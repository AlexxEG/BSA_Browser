using BSA_Browser.Properties;
using SharpBSABA2;
using SharpBSABA2.BA2Util;
using SharpBSABA2.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSA_Browser.Tools
{
    public partial class CompareEntryWindow : Form
    {
        public const int BufferSize = 4096 * 10;

        protected struct ArchiveEntryCompare
        {
            public bool Diff;
            public ArchiveEntry Entry;
            public List<CompareProperty> ComparedProperties;

            public ArchiveEntryCompare(ArchiveEntry entry, bool diff, List<CompareProperty> comparedProperties)
            {
                this.Diff = diff;
                this.Entry = entry;
                this.ComparedProperties = comparedProperties;
            }
        }

        protected struct CompareProperty
        {
            public string Name;
            public string Left;
            public string Right;
            public string CustomGroup;

            private CompareProperty(string name, string customGroup)
            {
                this.Name = name;
                this.Left = this.Right = null;
                this.CustomGroup = customGroup;
            }

            public CompareProperty(string name, string left, string right, string customGroup = "") : this(name, customGroup)
            {
                this.Left = left;
                this.Right = right;
            }
            public CompareProperty(string name, uint left, uint right, string customGroup = "") : this(name, customGroup)
            {
                this.Left = left.ToString();
                this.Right = right.ToString();
            }
            public CompareProperty(string name, bool left, bool right, string customGroup = "") : this(name, customGroup)
            {
                this.Left = left ? bool.TrueString : bool.FalseString;
                this.Right = right ? bool.TrueString : bool.FalseString;
            }
            public CompareProperty(string name, uint left, uint right, Type enumType, string customGroup = "") : this(name, customGroup)
            {
                this.Left = Enum.GetName(enumType, left);
                this.Right = Enum.GetName(enumType, right);
            }
        }

        /// <summary>
        /// Gets the <see cref="ArchiveEntry"/> to compare entries to.
        /// </summary>
        public ArchiveEntry Entry { get; private set; }

        /// <summary>
        /// Gets the list of <see cref="ArchiveEntry"/> to compare to <see cref="Entry"/>.
        /// </summary>
        public List<ArchiveEntry> Entries { get; private set; }

        public CompareEntryWindow()
        {
            InitializeComponent();
        }

        private void CompareEntryWindow_Load(object sender, EventArgs e)
        {
            Settings.Default.WindowStates.Restore(this, false);
        }

        private void CompareEntryWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowStates.Save(this);
        }

        private void lvEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvEntries.SelectedIndices.Count < 1)
                return;

            lvCompare.Items.Clear();

            var item = lvEntries.Items[lvEntries.SelectedIndices[0]];
            var properties = item.Tag as List<CompareProperty>;

            if (properties == null || properties.Count == 0)
                return;

            for (int i = lvCompare.Groups.Count - 1; i > 0; i--)
                lvCompare.Groups.RemoveAt(i);

            foreach (var group in properties.Select(x => x.CustomGroup).Distinct())
            {
                if (string.IsNullOrEmpty(group))
                    continue;

                lvCompare.Groups.Add(group, group);
            }

            foreach (var prop in properties)
            {
                var newItem = new ListViewItem(prop.Name);
                if (string.IsNullOrEmpty(prop.CustomGroup))
                    newItem.Group = lvCompare.Groups[0];
                else
                    newItem.Group = lvCompare.Groups[prop.CustomGroup];
                newItem.BackColor = lvCompare.Items.Count % 2 == 0 ? Color.White : Color.Gainsboro;
                newItem.SubItems.Add(prop.Left);
                newItem.SubItems.Add(prop.Right);
                lvCompare.Items.Add(newItem);
            }
        }

        public void SetSource(ArchiveEntry entry)
        {
            this.Entry = entry;

            label1.Text = this.Entry.FullPath;
        }

        public void SetEntries(List<ArchiveEntry> entries)
        {
            this.Entries = entries;

            lvEntries.Items.Clear();

            foreach (var entry in this.Entries)
            {
                lvEntries.Items.Add(entry.FullPathOriginal, entry.FileName, -1);
                lvEntries.Items[lvEntries.Items.Count - 1].SubItems.Add("-");
            }

            _ = this.DoCompare();
        }

        private async Task DoCompare()
        {
            var entries = this.Entries.ToList();
            byte[] sourceBuffer = this.Entry.GetRawDataStream().ToArray();

            while (entries.Count > 0)
            {
                var tasks = entries.Take(5).Select(x => Diff(sourceBuffer, x)).ToList();

                while (tasks.Any())
                {
                    var finishedTask = await Task.WhenAny(tasks);
                    tasks.Remove(finishedTask);
                    entries.Remove(finishedTask.Result.Entry);

                    lvEntries.Invoke(new Action(() =>
                    {
                        var item = lvEntries.Items[finishedTask.Result.Entry.FullPathOriginal];

                        if (finishedTask.IsFaulted)
                        {
                            item.SubItems[1].Text = "Error";
                            item.ForeColor = Color.LightGray;
                        }
                        else
                        {
                            if (finishedTask.Result.Diff)
                            {
                                item.SubItems[1].Text = "Diff";
                                item.ForeColor = Color.Red;
                            }
                            else if (!finishedTask.Result.Diff)
                            {
                                item.SubItems[1].Text = "Same";
                                item.ForeColor = Color.Green;
                            }
                            item.Tag = finishedTask.Result.ComparedProperties;
                        }
                    }));
                }

                await Task.Delay(100);
            }
        }

        /// <summary>
        /// Compares bytes in '<paramref name="entry"/>' with <paramref name="sourceBuffer"/>.
        /// </summary>
        private async Task<ArchiveEntryCompare> Diff(byte[] sourceBuffer, ArchiveEntry entry)
        {
            return await Task.Run(() =>
            {
                if (this.Entry == entry)
                {
                    return new ArchiveEntryCompare(entry, false, CompareProperties(this.Entry, entry));
                }

                bool diff = false;

                using (var compareMS = entry.GetRawDataStream())
                {
                    byte[] compareBuffer;
                    int chunk = 0;

                    while ((compareBuffer = compareMS.ReadBytes(BufferSize)).Length > 0)
                    {
                        for (int i = 0; i < compareBuffer.Length; i++)
                        {
                            if (sourceBuffer[i + (BufferSize * chunk)] != compareBuffer[i])
                            {
                                diff = true;
                                break;
                            }
                        }
                        if (diff) break; // break again if diff
                        chunk++;
                    }
                }

                return new ArchiveEntryCompare(entry, diff, CompareProperties(this.Entry, entry));
            });
        }

        private List<CompareProperty> CompareProperties(ArchiveEntry a, ArchiveEntry b)
        {
            var compareProperties = new List<CompareProperty>();

            compareProperties.Add(new CompareProperty("File Size",
                        string.Format("{0:n0} bytes", a.Size),
                        string.Format("{0:n0} bytes", b.Size)));

            // These properties only gets compared if they're same type, since some properties only appear on those types
            if (a.GetType() == b.GetType())
            {
                switch (a)
                {
                    case BA2TextureEntry aTex:
                        var texCompare = (BA2TextureEntry)b;

                        compareProperties.Add(new CompareProperty("DXIGI_FORMAT",
                            aTex.format,
                            texCompare.format,
                            typeof(DXGI_FORMAT_FULL),
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Format",
                            aTex.format,
                            texCompare.format,
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Width",
                            aTex.width,
                            texCompare.width,
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Height",
                            aTex.height,
                            texCompare.height,
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Chunks",
                            aTex.numChunks,
                            texCompare.numChunks,
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Mipmap Levels",
                            aTex.numMips,
                            texCompare.numMips,
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Cubemap",
                            aTex.isCubemap == 1,
                            texCompare.isCubemap == 1,
                            "Texture"));
                        break;
                    case BA2GNFEntry gnfTex:
                        var gnfCompare = (BA2GNFEntry)b;

                        compareProperties.Add(new CompareProperty("DXGI_FORMAT",
                            gnfTex.format,
                            gnfCompare.format,
                            typeof(DXGI_FORMAT_FULL),
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Format",
                            gnfTex.format,
                            gnfCompare.format,
                            "Texture"));

                        compareProperties.Add(new CompareProperty("numFormat",
                            gnfTex.numFormat,
                            gnfCompare.numFormat,
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Width",
                            gnfTex.width,
                            gnfCompare.width,
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Height",
                            gnfTex.height,
                            gnfCompare.height,
                            "Texture"));

                        compareProperties.Add(new CompareProperty("Chunks",
                            gnfTex.numChunks,
                            gnfCompare.numChunks,
                            "Texture"));
                        break;
                }
            }

            return compareProperties;
        }
    }
}
