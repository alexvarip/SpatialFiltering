using System;
using System.IO;
using System.Reflection;

namespace SpatialFiltering
{
    class Program
    {
        public static string filepath = "";
        public static string keepInstancesAlive = "";
        public static string selectedFilter = "";
        private static Action value;

        static void Main(string[] args)
        {
            var yuv = new YuvModel();
            var helpers = new Helpers(yuv);
            var filter = new Filter(yuv, helpers);
            var config = new ConfigurationMethods(Console.ReadLine, Console.Write, yuv, helpers);
            var controller = new CustomController(Console.ReadLine, Console.Write, config);


            ConfigStartup(args, filter);


            while (true)
            {

                Console.Clear();

                GetAssemblyInfo();

                if (keepInstancesAlive is "no")
                {
                    LoadNewFile(filter);

                    GetAssemblyInfo();
                }

                controller.Build()
                          .ApplyFilter(value)
                          .Out();
               
                GetNextAction();

            }

        }


        private static void ConfigStartup(string[] args, Filter filter)
        {

            if (args.Length is 0)
            {
                Help(filter);
                Environment.Exit(0);
            }
            else if (args[0] is "-h" || args[0] is "--help")
            {
                Help(filter);
                Environment.Exit(0);
            }
            else if (args.Length is 4 && (args[0] is "-f" ||  args[0] is "--filter")
                                 && (args[2] is "-i" ||  args[2] is "--import"))
            {
                var input = args[3];
                var file = Path.GetFileName(input) ?? string.Empty;

                if (file is not "" && file.EndsWith(".yuv"))
                {
                    filepath = input;
                    selectedFilter = args[1];
                }
                else if (file is "")
                {
                    Console.WriteLine($"'{input}' is not a file.\n");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine($"'{file}' is not recognized as a known internal file type.\n");
                    Console.WriteLine("A file extension of type '.yuv' is expected.\n");
                    Environment.Exit(0);
                }                
            }
            else
            {
                Help(filter);
                Environment.Exit(0);
            }


            // Get correct value for a given key
            value = filter.GetValue(args[1]);

            if (value is null)
            {
                Console.WriteLine($"'{args[1]}' is not recognized as a filter.\n");
                Help(filter);

                Environment.Exit(0);
            }

        }


        private static void GetAssemblyInfo()
        {
            Console.Clear();

            Console.Write($"{Assembly.GetAssembly(typeof(Program)).GetName().Name} ");
            Console.Write($"[Version {Assembly.GetAssembly(typeof(Program)).GetName().Version}]");
            Console.WriteLine("\n(c) 2021 Alex Varypatis. All rights reserved.\n");
        }


        private static void LoadNewFile(Filter filter)
        {
            Console.Write("\nLoad file\n> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (input is "exit")
                Environment.Exit(0);

            var file = Path.GetFileName(input) ?? string.Empty;
            
            if (file is not "" && file.EndsWith(".yuv"))
                filepath = input;
            else if (file is "")
            {
                Console.WriteLine($"'{input}' is not a file.\n");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"'{file}' is not recognized as a known internal file type.\n");
                Console.WriteLine("A file extension of type '.yuv' is expected.\n");
                Environment.Exit(0);
            }

            Console.Write($"\nFilter ({string.Join(", ", filter.GetAvailableFilters())})\n> ");
            input = Console.ReadLine() ?? string.Empty;

            if (input is "")
            {
                Console.WriteLine($"[SYSTEM] Filter was successfully restored using the default [{filter.GetFilter(0)}]\n");
                value = filter.GetValue(filter.GetFilter(0));
                selectedFilter = filter.GetFilter(0).ToLower();
                Console.Write("\nPress any key to proceed...");
                Console.ReadLine();
                return;
            }

            value = null;
            value = filter.GetValue(input);

            if (value is null)
            {
                Console.WriteLine($"'{input}' is not recognized as a filter.\n");
                Help(filter);

                Environment.Exit(0);
            }

            selectedFilter = input.ToLower();
        } 


        private static void GetNextAction()
        {

            Console.Write("\n\n\n  Press any key if you wish to continue...Type 'exit' for exiting application.\n  > ");
            
            var (Left, Top) = Console.GetCursorPosition();


            if (Console.ReadLine().ToLower() is "exit")
                Environment.Exit(0);

            
            Console.SetCursorPosition(Left, Top - 1);
            Console.Write($"\r {new string(' ', Console.WindowWidth + 50)} \r");
            Console.SetCursorPosition(Left, Top - 2);

            Console.Clear();

            Console.Write("\nDo you want to continue with a new file? [Type 'exit' for exiting application.]\n> ");

            string answer = Console.ReadLine();

            if ((selectedFilter is "laplacian" && answer is "no") || answer is "exit")
                Environment.Exit(0);

            keepInstancesAlive = answer is "no" ? "yes" : "no";

        }


        private static void Help(Filter filter)
        {
            Console.WriteLine($"\nUsage: executable [options[-f]] [filter] [options[-i]] [path-to-file]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  -h | --help       Display help.");
            Console.WriteLine("  -f | --filter     Select a filter.");
            Console.WriteLine("  -i | --import     Import a file.");
            Console.WriteLine("\nfilter:");
            Console.WriteLine($"  {string.Join("\n  ", filter.GetAvailableFilters())}");
            Console.WriteLine("\npath-to-file:");
            Console.WriteLine("  The path to a .yuv file to import.");
        }


    }
}