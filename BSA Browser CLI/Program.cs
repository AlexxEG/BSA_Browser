using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpBSABA2;

namespace BSA_Browser_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            // Print help screen. Ignore other arguments
            if (args.Contains("/?"))
            {
                PrintHelp();
            }
            else
            {
                var input = new List<string>();

                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];

                    if (!arg.StartsWith("/") && File.Exists(arg))
                        input.Add(arg);
                }

                if (input.Count == 0)
                {
                    Console.WriteLine("No input file(s) found");
                    goto exit;
                }

                if (args.Contains("/l"))
                {
                    PrintFileList(input);
                }

                if (args.Contains("/e"))
                {
                    // Last argument should be the destination
                    string destination = args[args.Length - 1];

                    // Make sure destination exists
                    if (!Directory.Exists(destination))
                    {
                        try
                        {
                            Directory.CreateDirectory(destination);
                        }
                        catch
                        {
                            Console.WriteLine("ERROR! Destination can\'t be found.");
                            goto exit;
                        }
                    }

                    ExtractFiles(input, args.Contains("/ati"), destination);
                }
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
                    int total = archive.Files.Count;
                    int line = Console.CursorTop;
                    int prevLength = 0;

                    archive.Files.ForEach(x =>
                    {
                        Console.SetCursorPosition(0, line);
                        string output = $"Extracting: {++count}/{total} - {x.FullPath}";
                        Console.Write(output.PadRight(prevLength));
                        prevLength = output.Length;

                        x.Extract(destination, true);
                    });

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

        private static void PrintFileList(List<string> files)
        {
            files.ForEach(file =>
            {
                if (files.Count > 1)
                    Console.WriteLine($"{Path.GetFileName(file)}:");

                var archive = OpenArchive(file, false);

                if (archive == null)
                {
                    Console.WriteLine("\tCouldn\'t open archive.");
                }
                else
                {
                    archive.Files.ForEach(x => Console.WriteLine((files.Count > 1 ? "\t" : "") + x.FullPath));
                }

                Console.WriteLine();
            });
        }

        static void PrintHelp()
        {
            Console.WriteLine("Extract or list files inside .bsa and .ba2 archives.");
            Console.WriteLine();
            Console.WriteLine("bsab [/e] [/l] FILE DESTINATION");
            Console.WriteLine();
            Console.WriteLine("  /e \t Extract all files");
            Console.WriteLine("  /l \t List all files");
            Console.WriteLine("  /ati \t Use ATI header for textures");
            Console.WriteLine();
        }
    }
}
