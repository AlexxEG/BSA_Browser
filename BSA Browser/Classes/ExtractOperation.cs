using SharpBSABA2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace BSA_Browser.Classes
{
    public enum ProgressType : int
    {
        StateChange,
        ProgressPercentageUpdate,
        Error
    }

    public class ArchiveEntryException : Exception
    {
        public ArchiveEntry ArchiveEntry { get; private set; }
        public Exception Exception { get; private set; }

        public ArchiveEntryException(ArchiveEntry entry, Exception exception) : base()
        {
            this.ArchiveEntry = entry;
            this.Exception = exception;
        }
    }

    #region Event Args

    public class StateChangeEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public int Count { get; private set; }
        public int FilesTotal { get; private set; }

        public StateChangeEventArgs(string filename, int count, int filesTotal) : base()
        {
            this.FileName = filename;
            this.Count = count;
            this.FilesTotal = filesTotal;
        }
    }

    public class ProgressPercentageUpdateEventArgs : EventArgs
    {
        public int ProgressPercentage { get; private set; }
        public TimeSpan RemainingEstimate { get; private set; }

        public ProgressPercentageUpdateEventArgs(int progressPercentage, TimeSpan remainingEstimate) : base()
        {
            this.ProgressPercentage = progressPercentage;
            this.RemainingEstimate = remainingEstimate;
        }
    }

    public class CompletedEventArgs : EventArgs
    {
        private List<ArchiveEntryException> _exceptions;

        public int Successful { get; private set; }
        public IReadOnlyList<ArchiveEntryException> Exceptions { get => _exceptions.AsReadOnly(); }

        public CompletedEventArgs(int successful, List<ArchiveEntryException> exceptions) : base()
        {
            this.Successful = successful;
            _exceptions = exceptions;
        }
    }

    public class ErrorEventArgs : EventArgs
    {
        public ArchiveEntry ArchiveEntry { get; private set; }
        public Exception Exception { get; private set; }

        public ErrorEventArgs(ArchiveEntry entry, Exception exception) : base()
        {
            this.ArchiveEntry = entry;
            this.Exception = exception;
        }
    }

    #endregion

    public class ExtractOperation
    {
        private long _speedBytes = 0;
        private List<ArchiveEntry> _files;
        private BackgroundWorker _worker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };
        private Thread _monitorSpeedThread;

        public bool UseFolderPath { get; private set; }
        public long SpeedBytes { get => _speedBytes; }
        public string Folder { get; private set; }
        public TimeSpan EstimateTimeRemaining { get; private set; }
        public IReadOnlyList<ArchiveEntry> Files
        {
            get
            {
                return _files.AsReadOnly();
            }
        }

        public delegate void StateChangeEventHandler(ExtractOperation sender, StateChangeEventArgs e);
        public delegate void ProgressPercentageUpdateEventHandler(ExtractOperation sender, ProgressPercentageUpdateEventArgs e);
        public delegate void CompletedEventHandler(ExtractOperation sender, CompletedEventArgs e);
        public delegate void ErrorEventHandler(ExtractOperation sender, ErrorEventArgs e);

        public event StateChangeEventHandler StateChange;
        public event ProgressPercentageUpdateEventHandler ProgressPercentageUpdate;
        public event CompletedEventHandler Completed;
        public event ErrorEventHandler Error;

        public ExtractOperation(string folder, IList<ArchiveEntry> files, bool useFolderPath)
        {
            Folder = folder;
            _files = (List<ArchiveEntry>)files;
            UseFolderPath = useFolderPath;

            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        public void Start()
        {
            _worker.RunWorkerAsync();
        }

        public void Cancel()
        {
            _worker.CancelAsync();
        }

        private void StartMonitorSpeed()
        {
            _monitorSpeedThread = new Thread(new ThreadStart(delegate
            {
                long prevBytesWritten = 0;
                var speeds = new List<long>(5);
                var sw = new Stopwatch();
                sw.Start();

                while (true)
                {
                    if (sw.ElapsedMilliseconds < 1000)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    long newSpeed = Files.Sum(x => (long)x.BytesWritten) - prevBytesWritten;
                    prevBytesWritten += newSpeed;

                    // Find average speed based on last 10 speeds
                    if (speeds.Count >= speeds.Capacity) speeds.RemoveAt(speeds.Count - 1);
                    speeds.Insert(0, newSpeed);
                    Interlocked.Exchange(ref _speedBytes, speeds.Sum() / speeds.Count);

                    sw.Restart();
                }
            }));
            _monitorSpeedThread.Start();
        }

        private void StopMonitorSpeed()
        {
            _monitorSpeedThread.Abort();
            _monitorSpeedThread = null;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var extracted = new Dictionary<string, int>();
            var exceptions = new List<ArchiveEntryException>();
            int prevProgress = 0, count = 0;
            DateTime startExtraction = DateTime.Now;

            StartMonitorSpeed();

            foreach (var entry in this.Files)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                // Update current file and count
                _worker.ReportProgress((int)ProgressType.StateChange, new StateChangeEventArgs(entry.FileName, count, this.Files.Count));

                try
                {
                    if (this.UseFolderPath)
                    {
                        entry.Extract(this.Folder, this.UseFolderPath);
                    }
                    else
                    {
                        // Keep track of what files have been extracted previously and add number to files with identical filenames
                        if (extracted.ContainsKey(entry.FileName))
                        {
                            string filename = Path.GetFileNameWithoutExtension(entry.FileName);
                            string extension = Path.GetExtension(entry.FileName);

                            entry.Extract(this.Folder,
                                this.UseFolderPath,
                                $"{filename} ({++extracted[entry.FileName]}){extension}");
                        }
                        else
                        {
                            entry.Extract(this.Folder, this.UseFolderPath);
                            extracted.Add(entry.FileName, 0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // ToDo: Set BytesWritten to 0 or remove from arguments.Files
                    exceptions.Add(new ArchiveEntryException(entry, ex));
                    _worker.ReportProgress((int)ProgressType.Error, new ErrorEventArgs(entry, ex));
                }

                count++;
                int progress = (int)Math.Round(((double)count / this.Files.Count) * 100);
                if (progress > prevProgress)
                {
                    /* ToDo:
                     * Use averages instead, and maybe only a limited number of the last ones.
                     * These two options should make the speed a bit more stable, if that's what we want.
                     */
                    this.EstimateTimeRemaining = TimeSpan.FromMilliseconds((DateTime.Now - startExtraction).TotalMilliseconds / count * (this.Files.Count - count));
                    prevProgress = progress;
                    _worker.ReportProgress((int)ProgressType.ProgressPercentageUpdate,
                                           new ProgressPercentageUpdateEventArgs(progress, this.EstimateTimeRemaining));
                }
            }

            if (!e.Cancel)
                e.Result = new CompletedEventArgs(count - exceptions.Count, exceptions);

            StopMonitorSpeed();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch ((ProgressType)e.ProgressPercentage)
            {
                case ProgressType.StateChange:
                    StateChange?.Invoke(this, (StateChangeEventArgs)e.UserState);
                    break;
                case ProgressType.ProgressPercentageUpdate:
                    ProgressPercentageUpdate?.Invoke(this, (ProgressPercentageUpdateEventArgs)e.UserState);
                    break;
                case ProgressType.Error:
                    Error?.Invoke(this, (ErrorEventArgs)e.UserState);
                    break;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                Completed?.Invoke(this, null);
            else
                Completed?.Invoke(this, (CompletedEventArgs)e.Result);
        }
    }
}
