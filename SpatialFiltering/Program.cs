using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpatialFiltering
{
    class Program
    {
        public static string filepath = "";

        static void Main(string[] args)
        {

            ConfigStartup(args);


            YuvModel yuv = new();


            Console.Clear();

            Console.Write($"{Assembly.GetAssembly(typeof(Program)).GetName().Name} ");
            Console.Write($"[Version {Assembly.GetAssembly(typeof(Program)).GetName().Version}]");
            Console.WriteLine("\n(c) 2021 Alex Varypatis. All rights reserved.\n");



            while (true)
            {

                var config = new ConfigurationMethods(Console.ReadLine, Console.Write, yuv);
                var controller = new CustomController(Console.ReadLine, Console.Write, config);


                if (!controller.filters.TryGetValue(args[1], out Action filter))
                {
                    Console.WriteLine($"'{args[1]}' is not recognized as a filter.\n");
                    Help();
                    Environment.Exit(-1);
                }


                controller.Build()
                          .ApplyFilter(filter)
                          .Out();


                GetAction();

            }

        }


        private static void ConfigStartup(string[] args)
        {
            if (args[0] is "-h" || args[0] is "--help")
            {
                Help();
                Environment.Exit(0);
            }
            else if (args.Length is 4 && (args[0] is "-f" ||  args[0] is "--filter")
                                 && (args[2] is "-i" ||  args[2] is "--import"))
            {

                if (args[3].Length >= 5 && args[3][^3..] is "yuv")
                    filepath = args[3];
                else
                {
                    Console.WriteLine($"'{args[3]}' is not recognized as an file extension,\noperable program or batch file.");
                    Console.WriteLine("\nA file extension of type 'yuv' is expected.\n");
                    Environment.Exit(0);
                }                
            }
            else
            {
                Help();
                Environment.Exit(0);
            }
        }


        private static void GetAction()
        {
            Console.Write("\n\n\nPress any key if you wish to continue...Type 'exit' for exiting application.\n> ");

            if (Console.ReadLine().ToLower() is "exit")
                Environment.Exit(0);

            Console.Clear();
        }


        private static void Help()
        {
            Console.WriteLine($"\nUsage: executable [options[-f]] [filter] [options[-i]] [path-to-file]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  -h | --help       Display help.");
            Console.WriteLine("  -f | --filter     Select a filter.");
            Console.WriteLine("  -i | --import     Import a file.");
            Console.WriteLine("\nfilter:");
            Console.WriteLine("  Median\n  Average");
            Console.WriteLine("\npath-to-file:");
            Console.WriteLine("  The path to a .yuv file to import.");
        }


    }
}