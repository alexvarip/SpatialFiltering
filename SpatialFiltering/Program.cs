using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpatialFiltering
{
    class Program
    {
        public static string filepath = "";
        public static string keepInstancesAlive = "";

        static void Main(string[] args)
        {
            var yuv = new YuvModel();
            var helpers = new Helpers(yuv);
            var filter = new Filter(yuv, helpers);
            ConfigurationMethods config;
            CustomController controller;


            ConfigStartup(args, filter);
            GetAssemblyInfo();


            while (true)
            {

                if (keepInstancesAlive is "no")
                    filter = new Filter(new YuvModel(), new Helpers(yuv));


                config = new ConfigurationMethods(Console.ReadLine, Console.Write, filter, helpers);
                controller = new CustomController(Console.ReadLine, Console.Write, config);


                var value = filter.GetValue(args[1]);

                if (value is null)
                {
                    Console.WriteLine($"'{args[1]}' is not recognized as a filter.\n");
                    Help(filter);

                    return;
                }


                controller.Build()
                          .ApplyFilter(value)
                          .Out();


                GetNextAction();

            }

        }


        private static void ConfigStartup(string[] args, Filter filter)
        {
            if (args[0] is "-h" || args[0] is "--help")
            {
                Help(filter);
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


        private static void GetNextAction()
        {

            Console.Write("\n\n\n  Press any key if you wish to continue...Type 'exit' for exiting application.\n> ");
            
            if (Console.ReadLine().ToLower() is "exit")
                Environment.Exit(0);
            

            Console.Write("\n\n  Continue with the same file resolution?\n> ");

            if (Console.ReadLine() is "yes")
                keepInstancesAlive = "yes";
            else
            {
                Console.Clear();
                GetAssemblyInfo();
            }
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