using SharpBSABA2;
using SharpBSABA2.BA2Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace BSA_Browser_CLI
{
    enum Filtering
    {
        None, Simple, Regex
    }

    [Flags]
    enum ListOptions
    {
        None = 0,
        Archive = 1,
        FullPath = 2,
        FileSize = 4
    }

    class Arguments
    {
        public bool Extract { get; private set; }
        public bool Help { get; private set; }
        public bool List { get; private set; }
        public bool Overwrite { get; private set; }
        public bool IgnoreErrors { get; private set; }
        public bool MatchTimeChanged { get; private set; }
        public bool NoHeaders { get; private set; }

        public Filtering Filtering { get; private set; } = Filtering.None;
        public ListOptions ListOptions { get; private set; } = ListOptions.None;

        public string Destination { get; private set; }
        public string FilterString { get; private set; }

        public Encoding Encoding { get; private set; } = Encoding.UTF7;

        public IReadOnlyCollection<string> Inputs { get; private set; }

        public Arguments(string[] args)
        {
            var input = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg.StartsWith("/") || arg.StartsWith("-") || arg.StartsWith("--"))
                {
                    switch (arg.ToLower().Split(':', '=')[0])
                    {
                        case "/?":
                        case "-h":
                        case "--help":
                            this.Help = true;
                            break;
                        case "/e":
                        case "-e":
                            this.Extract = true;
                            break;
                        case "/f":
                        case "-f":
                            this.Filtering = Filtering.Simple;
                            this.FilterString = args[++i];
                            break;
                        case "/i":
                        case "-i":
                            this.IgnoreErrors = true;
                            break;
                        case "/l":
                        case "-l":
                            this.List = true;

                            char[] options = arg.Split(':', '=').Last().ToLower().ToCharArray();

                            if (options.Contains('a')) this.ListOptions = ListOptions.Archive;
                            if (options.Contains('f')) this.ListOptions = (this.ListOptions | ListOptions.FullPath);
                            if (options.Contains('s')) this.ListOptions = (this.ListOptions | ListOptions.FileSize);

                            break;
                        case "/regex":
                        case "--regex":
                            this.Filtering = Filtering.Regex;
                            this.FilterString = args[++i];
                            break;
                        case "/enc":
                        case "--enc":
                        case "/encoding":
                        case "--encoding":
                            this.Encoding = this.ParseEncoding(args[++i]);
                            break;
                        case "/mtc":
                        case "--mtc":
                            this.MatchTimeChanged = true;
                            break;
                        case "/noheaders":
                        case "--noheaders":
                            this.NoHeaders = true;
                            break;
                        case "/o":
                        case "-o":
                        case "/overwrite":
                        case "--overwrite":
                            this.Overwrite = true;
                            break;
                        default:
                            throw new ArgumentException("Unrecognized argument: " + arg);
                    }
                }
                else
                {
                    if (i == args.Length - 1 && this.Extract) // Last item is destination when extracting
                    {
                        if (Directory.Exists(arg))
                            this.Destination = arg;
                        else
                            throw new DirectoryNotFoundException("Destination directory not found.");
                    }
                    else if (File.Exists(arg))
                    {
                        input.Add(arg);
                    }
                    else
                    {
                        throw new FileNotFoundException("File not found.", arg);
                    }
                }
            }

            this.Inputs = input.AsReadOnly();
        }

        private Encoding ParseEncoding(string encoding)
        {
            switch (encoding.ToLower())
            {
                case "utf7": return Encoding.UTF7;
                case "system": return Encoding.Default;
                case "ascii": return Encoding.ASCII;
                case "unicode": return Encoding.Unicode;
                case "utf32": return Encoding.UTF32;
                case "utf8": return Encoding.UTF8;
                default:
                    throw new ArgumentException("Unrecognized encoding: " + encoding);
            }
        }
    }

    class Program
    {
        private const int ERROR_INVALID_FUNCTION = 1;
        private const int ERROR_FILE_NOT_FOUND = 2;
        private const int ERROR_PATH_NOT_FOUND = 3;
        private const int ERROR_BAD_ARGUMENTS = 160;

        static Arguments _arguments;
        static Regex _regex;
        static WildcardPattern _pattern;

        static void Main(string[] args)
        {
            // Parse arguments. Go to exit if null, errors has occurred and been handled
            if ((_arguments = ParseArguments(args)) == null)
                goto exit;

            // Print help screen. Ignore other arguments
            if (args.Length == 0 || _arguments.Help)
            {
                PrintHelp();
                goto exit;
            }

            if (_arguments.Inputs.Count == 0)
            {
                Console.WriteLine("No input file(s) found");
                Environment.ExitCode = ERROR_FILE_NOT_FOUND;
                goto exit;
            }

            // Setup filtering
            if (_arguments.Filtering != Filtering.None)
            {
                if (_arguments.Filtering == Filtering.Simple)
                {
                    _pattern = new WildcardPattern(
                        $"*{WildcardPattern.Escape(_arguments.FilterString).Replace("`*", "*")}*",
                        WildcardOptions.Compiled | WildcardOptions.IgnoreCase);
                }
                else if (_arguments.Filtering == Filtering.Regex)
                {
                    try
                    {
                        _regex = new Regex(_arguments.FilterString, RegexOptions.Compiled | RegexOptions.Singleline);
                    }
                    catch
                    {
                        Console.WriteLine("Invalid regex filter string");
                        Environment.ExitCode = ERROR_BAD_ARGUMENTS;
                        goto exit;
                    }
                }
            }

            // Default to list if no other options have been given
            if (_arguments.List || (!_arguments.List && !_arguments.Extract && !_arguments.Help))
            {
                try
                {
                    PrintFileList(_arguments.Inputs.ToList(), _arguments.ListOptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occured opening archive:");
                    Console.WriteLine(ex.Message);
                    Environment.ExitCode = ERROR_INVALID_FUNCTION;
                }
            }

            if (_arguments.Extract)
            {
                try
                {
                    ExtractFiles(_arguments.Inputs.ToList(), _arguments.Destination, _arguments.Overwrite);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occured opening archive:");
                    Console.WriteLine(ex.Message);
                    Environment.ExitCode = ERROR_INVALID_FUNCTION;
                }
            }

        exit:;

#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
        }

        static void ExtractFiles(List<string> archives, string destination, bool overwrite)
        {
            archives.ForEach(archivePath =>
            {
                Archive archive = null;

                try
                {
                    archive = OpenArchive(archivePath);
                }
                catch (Exception ex)
                {
                    if (!_arguments.IgnoreErrors)
                        throw ex;
                    else
                        Console.WriteLine($"An error occured opening '{Path.GetFileName(archivePath)}'. Skipping...");
                }

                int count = 0;
                int total = archive.Files.Count(x => Filter(x.FullPath));
                int line = -1;
                int prevLength = 0;
                int skipped = 0;

                // Some Console properties might not be available in certain situations, 
                // e.g. when redirecting stdout. To prevent crashing, setting the cursor position should only
                // be done if there actually is a cursor to be set.
                try
                {
                    line = Console.CursorTop;
                }
                catch (IOException) { }

                HandleUnsupportedTextures(archive.Files);

                foreach (var entry in archive.Files)
                {
                    if (!Filter(entry.FullPath))
                        continue;

                    string output = $"Extracting: {++count}/{total} - {entry.FullPath}".PadRight(prevLength);

                    if (line > -1)
                    {
                        Console.SetCursorPosition(0, line);
                        Console.Write(output);
                    }
                    else
                    {
                        Console.WriteLine(output);
                    }
                    prevLength = output.Length;

                    try
                    {
                        if (!overwrite && File.Exists(Path.Combine(destination, entry.FullPath)))
                        {
                            skipped++;
                        }
                        else
                        {
                            entry.Extract(destination, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!_arguments.IgnoreErrors)
                            throw ex;
                        else
                            Console.WriteLine($"An error occured extracting '{entry.FullPath}'. Skipping...");
                    }
                }

                Console.WriteLine();

                if (skipped > 0)
                    Console.WriteLine($"Skipped {skipped} existing files");
            });
        }

        static void HandleUnsupportedTextures(List<ArchiveEntry> files)
        {
            if (files.All(x => (x as BA2TextureEntry)?.IsFormatSupported() != false))
                return;

            if (_arguments.NoHeaders)
            {
                foreach (var fe in files.Where(x => (x as BA2TextureEntry)?.IsFormatSupported() == false))
                {
                    (fe as BA2TextureEntry).GenerateTextureHeader = false;
                }
            }
            else
            {
                // Remove unsupported textures to skip them
                for (int i = files.Count; i-- > 0;)
                {
                    if ((files[i] as BA2TextureEntry)?.IsFormatSupported() == false)
                        files.RemoveAt(i);
                }
            }
        }

        static Archive OpenArchive(string file)
        {
            Archive archive = null;
            string extension = Path.GetExtension(file);

            switch (extension.ToLower())
            {
                case ".bsa":
                case ".dat":
                    archive = new SharpBSABA2.BSAUtil.BSA(file, _arguments.Encoding);
                    break;
                case ".ba2":
                    archive = new SharpBSABA2.BA2Util.BA2(file, _arguments.Encoding);
                    break;
                default:
                    throw new Exception($"Unrecognized archive file type ({extension}).");
            }

            archive.MatchLastWriteTime = _arguments.MatchTimeChanged;
            archive.Files.Sort((a, b) => string.CompareOrdinal(a.LowerPath, b.LowerPath));
            return archive;
        }

        static bool Filter(string input)
        {
            if (_arguments.Filtering == Filtering.Simple)
            {
                return _pattern.IsMatch(input);
            }
            else if (_arguments.Filtering == Filtering.Regex)
            {
                return _regex.IsMatch(input);
            }

            return true;
        }

        static Arguments ParseArguments(params string[] args)
        {
            try
            {
                return new Arguments(args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.ExitCode = ERROR_BAD_ARGUMENTS;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Input file not found: " + ex.FileName);
                Environment.ExitCode = ERROR_FILE_NOT_FOUND;
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.ExitCode = ERROR_PATH_NOT_FOUND;
            }

            return null;
        }

        static void PrintFileList(List<string> archives, ListOptions options)
        {
            archives.ForEach(archivePath =>
            {
                // If there are multiple archives print archive filename to differentiate
                if (archives.Count > 1)
                    Console.WriteLine($"{Path.GetFileName(archivePath)}:");

                Archive archive = null;

                try
                {
                    archive = OpenArchive(archivePath);
                }
                catch (Exception ex)
                {
                    if (!_arguments.IgnoreErrors)
                        throw ex;
                    else
                        Console.WriteLine($"An error occured opening '{Path.GetFileName(archivePath)}'. Skipping...");
                }

                bool filesize = false;
                string prefix = string.Empty;

                if (options.HasFlag(ListOptions.Archive))
                    prefix = Path.GetFileName(archive.FullPath);

                if (options.HasFlag(ListOptions.FullPath))
                    prefix = Path.GetFullPath(archive.FullPath);

                filesize = options.HasFlag(ListOptions.FileSize);

                foreach (var entry in archive.Files)
                {
                    if (!Filter(entry.FullPath))
                        continue;

                    string indent = string.IsNullOrEmpty(prefix) && archives.Count > 1 ? "\t" : string.Empty;
                    string filesizeString = filesize ? entry.RealSize + "\t\t" : string.Empty;

                    Console.WriteLine($"{indent}{filesizeString}{Path.Combine(prefix, entry.FullPath)}");
                }

                Console.WriteLine();
            });
        }

        static void PrintHelp()
        {
            Console.WriteLine("Extract or list files inside .bsa and .ba2 archives.");
            Console.WriteLine();
            Console.WriteLine("bsab [OPTIONS] FILE [FILE...] [DESTINATION]");
            Console.WriteLine();
            Console.WriteLine("  -h, --help             Display this help page");
            Console.WriteLine("  -i                     Ignore errors with opening archives or extracting files");
            Console.WriteLine("  -e                     Extract all files. Options:");
            Console.WriteLine("  -l:[OPTIONS]           List all files");
            Console.WriteLine("     options               A   Prepend each line with archive filename");
            Console.WriteLine("                           F   Prepend each line with full archive file path");
            Console.WriteLine("                           S   Display file size");
            Console.WriteLine("  -o, --overwrite        Overwrite existing files");
            Console.WriteLine("  -f FILTER              Simple filtering. Wildcard supported");
            Console.WriteLine("  --regex REGEX          Regex filtering");
            Console.WriteLine("  --encoding ENCODING    Set encoding to use");
            Console.WriteLine("     encodings             utf7     (Default)");
            Console.WriteLine("                           system   Use System's default encoding");
            Console.WriteLine("                           ascii");
            Console.WriteLine("                           unicode");
            Console.WriteLine("                           utf32");
            Console.WriteLine("                           utf8");
            Console.WriteLine("  --noheaders            Extract unsupported textures without DDS header instead of skipping");
            Console.WriteLine();
        }
    }
}
