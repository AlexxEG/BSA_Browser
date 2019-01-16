using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using SharpBSABA2;

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
        public bool ATI { get; private set; }

        public Filtering Filtering { get; private set; } = Filtering.None;
        public ListOptions ListOptions { get; private set; } = ListOptions.None;

        public string Destination { get; private set; }
        public string FilterString { get; private set; }

        public IReadOnlyCollection<string> Inputs { get; private set; }

        public Arguments(string[] args)
        {
            List<string> input = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg.StartsWith("/"))
                {
                    switch (arg.ToLower().Split(':')[0])
                    {
                        case "/?":
                            this.Help = true;
                            break;
                        case "/e":
                            this.Extract = true;
                            break;
                        case "/l":
                            this.List = true;

                            char[] options = arg.Split(':').Last().ToLower().ToCharArray();

                            if (options.Contains('a')) this.ListOptions = ListOptions.Archive;
                            if (options.Contains('f')) this.ListOptions = (this.ListOptions | ListOptions.FullPath);
                            if (options.Contains('s')) this.ListOptions = (this.ListOptions | ListOptions.FileSize);

                            break;
                        case "/ati":
                            this.ATI = true;
                            break;
                        case "/f":
                            this.Filtering = Filtering.Simple;
                            this.FilterString = args[++i];
                            break;
                        case "/regex":
                            this.Filtering = Filtering.Regex;
                            this.FilterString = args[++i];
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
    }

    class Program
    {
        static Arguments _arguments;
        static Regex _regex;
        static WildcardPattern _pattern;

        static void Main(string[] args)
        {
            _arguments = new Arguments(args);

            // Print help screen. Ignore other arguments
            if (args.Length == 0 || _arguments.Help)
            {
                PrintHelp();
                goto exit;
            }

            if (_arguments.Inputs.Count == 0)
            {
                Console.WriteLine("No input file(s) found");
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
                        goto exit;
                    }
                }
            }

            if (_arguments.List || (!_arguments.List && !_arguments.Extract && !_arguments.Help))
            {
                PrintFileList(_arguments.Inputs.ToList(), _arguments.ListOptions);
            }

            if (_arguments.Extract)
            {
                if (string.IsNullOrEmpty(_arguments.Destination) || !Directory.Exists(_arguments.Destination))
                {
                    Console.WriteLine($"Destination \'{_arguments.Destination}\' not found!");
                    goto exit;
                }

                ExtractFiles(_arguments.Inputs.ToList(), _arguments.ATI, _arguments.Destination);
            }

            exit:;

#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
        }

        static void ExtractFiles(List<string> files, bool ati, string destination)
        {
            files.ForEach(file =>
            {
                var archive = OpenArchive(file, ati);

                if (archive == null) { }
                else
                {
                    int count = 0;
                    int total = archive.Files.Count(x => Filter(x.FullPath));
                    int line = -1;
                    int prevLength = 0;

                    // Some Console properties might not be available in certain situations, 
                    // e.g. when redirecting stdout. To prevent crashing, setting the cursor position should only
                    // be done if there actually is a cursor to be set.
                    try
                    {
                        line = Console.CursorTop;
                    }
                    catch (IOException) { }

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

                        entry.Extract(destination, true);
                    }

                    Console.WriteLine();
                }
            });
        }

        static Archive OpenArchive(string file, bool ati)
        {
            Archive archive = null;

            try
            {
                string extension = Path.GetExtension(file);

                switch (extension.ToLower())
                {
                    case ".bsa":
                    case ".dat":
                        archive = new SharpBSABA2.BSAUtil.BSA(file);
                        break;
                    case ".ba2":
                        archive = new SharpBSABA2.BA2Util.BA2(file) { UseATIFourCC = ati };
                        break;
                    default:
                        throw new Exception($"Unrecognized archive file type ({extension}).");
                }

                archive.Files.Sort((a, b) => string.CompareOrdinal(a.LowerPath, b.LowerPath));
                return archive;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured:");
                Console.WriteLine(ex.Message);
                return null;
            }
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

        private static void PrintFileList(List<string> files, ListOptions options)
        {
            files.ForEach(file =>
            {
                if (files.Count > 1)
                    Console.WriteLine($"{Path.GetFileName(file)}:");

                var archive = OpenArchive(file, false);

                if (archive == null)
                {
                    Console.WriteLine("{0}Couldn\'t open archive.", files.Count > 1 ? "\t" : string.Empty);
                }
                else
                {
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

                        string indent = string.IsNullOrEmpty(prefix) && files.Count > 1 ? "\t" : string.Empty;
                        string filesizeString = filesize ? entry.RealSize + "\t\t" : string.Empty;

                        Console.WriteLine($"{indent}{filesizeString}{Path.Combine(prefix, entry.FullPath)}");
                    }
                }

                Console.WriteLine();
            });
        }

        static void PrintHelp()
        {
            Console.WriteLine("Extract or list files inside .bsa and .ba2 archives.");
            Console.WriteLine();
            Console.WriteLine("bsab [/e] [/l:[options]] [/ati] [/f [pattern]] [/regex [pattern]] [FILE] [DESTINATION]");
            Console.WriteLine();
            Console.WriteLine("  /? \t\t Display this help page");
            Console.WriteLine("  /e \t\t Extract all files");
            Console.WriteLine("  /l \t\t List all files");
            Console.WriteLine("    options \t  A \t Prepend each line with archive filename");
            Console.WriteLine("    \t      \t  F \t Prepend each line with full archive file path");
            Console.WriteLine("    \t      \t  S \t Display file size");
            Console.WriteLine("  /f \t\t Simple filtering. Wildcard supported");
            Console.WriteLine("  /regex \t Regex filtering");
            Console.WriteLine("  /ati \t\t Use ATI header for textures");
            Console.WriteLine();
        }
    }
}
