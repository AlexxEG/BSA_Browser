using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BSA_Browser_CLI.Filtering;

namespace BSA_Browser_CLI
{
    [Flags]
    internal enum ExtractOptions
    {
        None = 0,
        /// <summary>
        /// Extract into destination with directory.
        /// </summary>
        Directory = 1,
        /// <summary>
        /// Extract into destination without directory.
        /// </summary>
        NoDirectory = 2
    }

    [Flags]
    internal enum ListOptions
    {
        None = 0,
        Archive = 1,
        FullPath = 2,
        Filename = 4,
        FileSize = 8,
        FileSizeFormat = 16
    }

    internal class Arguments
    {
        public bool Extract { get; private set; }
        public bool Help { get; private set; }
        public bool List { get; private set; }
        public bool Overwrite { get; private set; }
        public bool IgnoreErrors { get; private set; }
        public bool MatchTimeChanged { get; private set; }
        public bool NoHeaders { get; private set; }

        public ExtractOptions ExtractOptions { get; private set; } = ExtractOptions.Directory;
        public ListOptions ListOptions { get; private set; } = ListOptions.None;

        public string Destination { get; private set; }

        public Encoding Encoding { get; private set; } = Encoding.UTF7;

        public IReadOnlyCollection<string> Inputs { get; private set; }
        public IReadOnlyCollection<Filter> Filters { get; private set; }

        public Arguments(string[] args)
        {
            var input = new List<string>();
            var filters = new List<Filter>();

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
                            this.ExtractOptions = ParseExtractOptions(arg);
                            break;
                        case "/f":
                        case "-f":
                            filters.Add(new Filter(FilteringTypes.Simple, args[++i]));
                            break;
                        case "/i":
                        case "-i":
                            this.IgnoreErrors = true;
                            break;
                        case "/l":
                        case "-l":
                            this.List = true;
                            this.ListOptions = ParseListOptions(arg);
                            break;
                        case "/regex":
                        case "--regex":
                            filters.Add(new Filter(FilteringTypes.Regex, args[++i]));
                            break;
                        case "/enc":
                        case "--enc":
                        case "/encoding":
                        case "--encoding":
                            this.Encoding = this.ParseEncoding(args[++i]);
                            break;
                        case "--exclude":
                        case "/exclude":
                            filters.Add(new Filter(FilteringTypes.SimpleExclude, args[++i]));
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
            this.Filters = filters.AsReadOnly();
        }

        private ExtractOptions ParseExtractOptions(string arg)
        {
            const string AllowedOptions = "n";

            // Check if there is any sub options
            if (!arg.Contains(':') && !arg.Contains('='))
                return ExtractOptions.Directory;

            // Get chars after : or =
            var options = arg.Split(':', '=').Last().ToLower().ToCharArray();

            // Check that all sub options are valid
            if (options.Any(x => AllowedOptions.Contains(x) == false))
                throw new ArgumentException("Unknown -e sub options: " + new string(options) + "\nSee --help page for valid options.");

            var value = ExtractOptions.Directory;

            if (options.Contains('n'))
                value = ExtractOptions.NoDirectory;

            return value;
        }

        private ListOptions ParseListOptions(string arg)
        {
            const string AllowedOptions = "afnsx";

            // Check if there is any sub options
            if (!arg.Contains(':') && !arg.Contains('='))
                return ListOptions.None;

            // Get chars after : or =
            var options = arg.Split(':', '=').Last().ToLower().ToCharArray();

            // Check that all sub options are valid
            if (options.Any(x => AllowedOptions.Contains(x) == false))
                throw new ArgumentException("Unknown -l sub options: " + new string(options) + "\nSee --help page for valid options.");

            var value = ListOptions.None;

            foreach (char c in options)
            {
                switch (c)
                {
                    case 'a': value |= ListOptions.Archive; break;
                    case 'f': value |= ListOptions.FullPath; break;
                    case 'n': value |= ListOptions.Filename; break;
                    case 's': value |= ListOptions.FileSize; break;
                    case 'x': value |= ListOptions.FileSizeFormat; break;
                }
            }

            return value;
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
}
