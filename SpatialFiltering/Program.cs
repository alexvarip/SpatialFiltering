using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpatialFiltering
{
    class Program
    {
        public static string filepath = "";
        public static string keepAlive = "";

        static void Main(string[] args)
        {

            ConfigStartup(args);


            GetAssemblyInfo();


            var yuv = new YuvModel();
            var helpers = new Helpers(yuv);
            Filters filters;
            ConfigurationMethods config;
            CustomController controller;


            while (true)
            {

                if (keepAlive is "no")
                {
                    yuv = new YuvModel();
                    helpers = new Helpers(yuv);
                }

                filters = new Filters(yuv, helpers);
                config = new ConfigurationMethods(Console.ReadLine, Console.Write, filters, helpers);
                controller = new CustomController(Console.ReadLine, Console.Write, config);


                if (!filters.filter.TryGetValue(args[1], out Action filter))
                {
                    Console.WriteLine($"'{args[1]}' is not recognized as a filter.\n");
                    Help();

                    return;
                }


                controller.Build()
                          .ApplyFilter(filter)
                          .Out();


                GetNextAction();

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


        private static void GetAssemblyInfo()
        {
            Console.Clear();

            Console.Write($"{Assembly.GetAssembly(typeof(Program)).GetName().Name} ");
            Console.Write($"[Version {Assembly.GetAssembly(typeof(Program)).GetName().Version}]");
            Console.WriteLine("\n(c) 2021 Alex Varypatis. All rights reserved.\n");
        }


        private static void GetNextAction()
        {

            Console.Write("\n\n\n  Press any key if you wish to continue...Type 'exit' for exiting application.\n> ");
            
            if (Console.ReadLine().ToLower() is "exit")
                Environment.Exit(0);
            

            Console.Write("\n\n  Continue with the same file resolution?\n> ");

            if (Console.ReadLine() is "yes")
                keepAlive = "yes";
            else
            {
                Console.Clear();
                GetAssemblyInfo();
            }
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