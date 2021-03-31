using System;
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

                var controller = new CustomController(Console.ReadLine, Console.Write, yuv);

                    
                controller.Build()
                          .ApplyFilter()
                          .Out();


                GetAction();

            }

        }


        private static void GetAction()
        {
            Console.Write("\n\nPress any key if you wish to continue...Type 'exit' for exiting application.\n> ");

            if (Console.ReadLine().ToLower() is "exit")
                Environment.Exit(0);

            Console.Clear();
        }


        private static void ConfigStartup(string[] args)
        {
            if (args.Length is 2 && (args[0] is "-i" ||  args[0] is "--import"))
            {
                string extension = "";

                if (args[1].Length >= 5 && args[1][^3..] is "yuv")
                {
                    extension = args[1][^3..];
                    filepath = args[1];
                }             
                else
                {
                    Console.WriteLine($"'{args[1]}' is not recognized as an file extension,\noperable program or batch file.");
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


        private static void Help()
        {
            Console.WriteLine($"\nUsage: executable [options] [path-to-file]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  -i | --import   Import a file.");
            Console.WriteLine("\npath-to-file:");
            Console.WriteLine("  The path to a .yuv file to import.");
        }


    }
}