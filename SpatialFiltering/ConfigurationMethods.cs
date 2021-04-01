﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpatialFiltering
{
    public class ConfigurationMethods
    {

        #region Private Variables

        private readonly Func<string> _inputProvider;
        private readonly Action<string> _outputProvider;
        public readonly YuvModel _yuv;
        private string _outfilepath = "";
        public int value = 0;
        public int mask = 3;

        #endregion



        #region Constructor
        public ConfigurationMethods(Func<string> inputProvider, Action<string> outputProvider, YuvModel yuv)
        {
            _inputProvider = inputProvider;
            _outputProvider = outputProvider;
            _yuv = yuv;
        }
        #endregion



        #region Private Methods

        public void GetInformation()
        {

            if (!File.Exists(Program.filepath))
            {
                Console.Clear();
                _outputProvider($"Could not find file '{Program.filepath}'.\n");
                Environment.Exit(-1);
            }


            _outputProvider("\n ┌─────────────────────────┐ ");
            _outputProvider("\n ▓   YUV File Components   ▓ ");
            _outputProvider("\n ▓          Y U V          ▓ ");
            _outputProvider("\n └─────────────────────────┘ ");

            _outputProvider("\n\n  <Y component resolution>");
            _outputProvider("\n  width: "); var (Left, Top) = Console.GetCursorPosition();
            
            var result = _inputProvider();
            if (int.TryParse(result, out value))
                _yuv.YWidth = value;

            
            //Console.SetCursorPosition(Left + 3, Top);
            //Console.Write(new string(' ', 3));
            

            UserExit(result);

            _outputProvider("  height: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _yuv.YHeight = value;

            UserExit(result);

            _outputProvider("\n  <U component resolution>");
            _outputProvider("\n  width: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _yuv.UWidth = value;

            UserExit(result);

            _outputProvider("  height: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _yuv.UHeight = value;

            UserExit(result);

            _outputProvider("\n  <V component resolution>");
            _outputProvider("\n  width: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _yuv.VWidth = value;

            UserExit(result);

            _outputProvider("  height: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _yuv.VHeight = value;

            UserExit(result);

            _yuv.YTotalBytes = _yuv.YResolution;
            _yuv.UTotalBytes = _yuv.UResolution;
            _yuv.VTotalBytes = _yuv.VResolution;



            Array.Resize(ref _yuv.Ybytes, _yuv.YResolution);
            Array.Resize(ref _yuv.Ubytes, _yuv.UResolution);
            Array.Resize(ref _yuv.Vbytes, _yuv.VResolution);


            FileProperties();
        }


        private void FileProperties()
        {
            _outputProvider("\n");
            _outputProvider("\n ┌──────────────────────┐ ");
            _outputProvider("\n ░   YUV File Details   ▓ ");
            _outputProvider("\n ▓        Y U V         ░ ");
            _outputProvider("\n └──────────────────────┘ ");
            _outputProvider("\n");
            _outputProvider($"\n  Name: {Path.GetFileName(Program.filepath)}");
            _outputProvider($"\n  Resolution: {_yuv.YWidth} x {_yuv.YHeight}");
            _outputProvider($"\n  Width: {_yuv.YWidth} pixels");
            _outputProvider($"\n  Height: {_yuv.YHeight} pixels");
            _outputProvider($"\n  Item Type: {Path.GetExtension(Program.filepath).ToUpper()} File");
            _outputProvider($"\n  Folder Path: {Path.GetDirectoryName(Program.filepath)}");
            _outputProvider($"\n  Date Created: {File.GetCreationTime(Program.filepath)}");
            _outputProvider($"\n  Date Modified: {File.GetLastAccessTime(Program.filepath)}");
            _outputProvider($"\n  Size: {_yuv.YTotalBytes} KB");
            _outputProvider($"\n  Owner: {Environment.UserName}");
            _outputProvider($"\n  Computer: {Environment.MachineName}");
        }


        public Task ReadYuvComponents()
        {
            try
            {

                using (FileStream fsSource = new(Program.filepath, FileMode.Open, FileAccess.Read))
                {

                    // Write y component into a byte array.
                    int numBytesRead = 0;
                    while (_yuv.YTotalBytes > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(_yuv.Ybytes, numBytesRead, _yuv.YTotalBytes);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        _yuv.YTotalBytes -= n;
                    }
                    _yuv.YTotalBytes = _yuv.Ybytes.Length;


                    // Write u component into a byte array.
                    numBytesRead = 0;
                    while (_yuv.UTotalBytes > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(_yuv.Ubytes, numBytesRead, _yuv.UTotalBytes);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        _yuv.UTotalBytes -= n;
                    }
                    _yuv.UTotalBytes = _yuv.Ubytes.Length;


                    // Write v component into a byte array.
                    numBytesRead = 0;
                    while (_yuv.VTotalBytes > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(_yuv.Vbytes, numBytesRead, _yuv.VTotalBytes);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        _yuv.VTotalBytes -= n;
                    }
                    _yuv.VTotalBytes = _yuv.Vbytes.Length;


                    return Task.CompletedTask;
                }
            }
            catch (FileLoadException ioEx)
            {
                Console.Clear();
                Console.WriteLine($"{ioEx.Message}");

                Environment.Exit(-1);

                return Task.FromException(ioEx);
            }

        }


        public void UserAction()
        {

            // ▒▓░ 

            _outputProvider("\n\n");
            _outputProvider("\n ┌───────────────────────────────┐ ");
            _outputProvider("\n ▒   YUV Implementation Method   ▒ ");
            _outputProvider("\n ▒            Y U V              ▒ ");
            _outputProvider("\n └───────────────────────────────┘ ");


            string result = "";
           
            _outputProvider("\n\n  Do you wish to implement the creation task using 1 or 2 dimensional arrays: ");

            result = _inputProvider();

            UserExit(result);

            if (!int.TryParse(result, out value))
                value = 2;
            else if (value > 2)
            {
                value = 2;
                _outputProvider($"\n  [SYSTEM] The creation task implementation was successfully restored using one of the correct values: {value}\n");
            }
                

            _outputProvider("\n\n  Please select window/mask size [default = 3] (Usage: [integer])\n  [Press Enter to continue]");
            //_outputProvider("\t\t\t\t\t\tUsage: [integer]\n\t\t\t\t\t\t  integer  \t A positive number specifies the mask.\n\n [Press Enter to continue]");

            Console.ReadKey();
            _outputProvider("\r" + new string(' ', Console.WindowWidth) + "\r");
            _outputProvider("  Your preference: ");

            result = _inputProvider();

            UserExit(result);

            if (!int.TryParse(result, out mask))
                mask = 3;

        }


        private void InformUser()
        {
            _outputProvider("\n\n  Getting things ready...\n");
            _outputProvider("  Filter is now being applied");
        }


        public string CreateOutPath()
        {
            string path = $"{Environment.CurrentDirectory}/generated";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory($"{Environment.CurrentDirectory}/generated");
                _outfilepath = $"{path}/BlowingBubbles_416x240_filtered_{mask}x{mask}_{value}D.yuv";
            }
            else
                _outfilepath = $"{Environment.CurrentDirectory}/generated/BlowingBubbles_416x240_filtered_{mask}x{mask}_{value}D.yuv";

            return _outfilepath;
        }


        public Task ApplyMedianFilter()
        {

            bool q = false;
            bool w = false;

            var (Left, Top) = Console.GetCursorPosition();

            if (value is 1)
            {
                byte[] YbytesExtended = Extend1DArray(_yuv.Ybytes, _yuv.YTotalBytes);


                InformUser();


                Array.Resize(ref _yuv.YMedian, _yuv.YResolution);


                Parallel.Invoke(
                                delegate ()
                                {

                                    int index = (mask - 1) / 2;

                                    while (index < YbytesExtended.Length - ((mask - 1) / 2))
                                    {
                                        q = true;

                                        FindMedian(YbytesExtended, index);
                                        index++;
                                    }
                                },

                                delegate ()
                                {
                                    while (true)
                                    {
                                        for (int dots = 0; dots < 3; ++dots)
                                        {
                                            Console.Write('.');
                                            Thread.Sleep(500);
                                            if (dots == 2)
                                            {
                                                if (q == true)
                                                {
                                                    w = true;
                                                    break;
                                                }

                                                Console.SetCursorPosition(Left, Top);
                                                Console.Write(new string(' ', 3));
                                                Console.SetCursorPosition(Left, Top);

                                                dots = -1;
                                                Thread.Sleep(500);
                                            }
                                        }

                                        if (w == true)
                                            break;
                                    }
                                }
                );


                //_outputProvider("Task successfully completed.\n");
            }

            else if (value is 2)
            {

                InformUser();


                Parallel.Invoke(
                    () => _yuv.Yplane = ConvertTo2DExtendedArray(_yuv.Ybytes, _yuv.YHeight, _yuv.YWidth),
                    () => _yuv.Uplane = ConvertTo2DArray(_yuv.Ubytes, _yuv.UHeight, _yuv.UWidth),
                    () => _yuv.Vplane = ConvertTo2DArray(_yuv.Vbytes, _yuv.VHeight, _yuv.VWidth));


                _yuv.YMedian2D = new byte[_yuv.YHeight, _yuv.YWidth];


                Parallel.Invoke(
                                delegate ()
                                {

                                    for (int i = ((mask - 1) / 2); i < _yuv.Yplane.GetLength(0) - ((mask - 1) / 2); i++)
                                    {
                                        for (int j = ((mask - 1) / 2); j < _yuv.Yplane.GetLength(1) - ((mask - 1) / 2); j++)
                                        {
                                            FindMedian2D(i, j);
                                        }
                                    }

                                    q = true;
                                },

                                delegate ()
                                {
                                    while (true)
                                    {
                                        for (int dots = 0; dots < 3; ++dots)
                                        {
                                            Console.Write('.');
                                            Thread.Sleep(500);
                                            if (dots == 2)
                                            {
                                                if (q == true)
                                                {
                                                    w = true;
                                                    break;
                                                }

                                                Console.SetCursorPosition(Left, Top);
                                                Console.Write(new string(' ', 3));
                                                Console.SetCursorPosition(Left, Top);

                                                dots = -1;
                                                Thread.Sleep(500);
                                            }
                                        }

                                        if (w == true)
                                            break;
                                    }
                                }
                );


                //_outputProvider("Task successfully completed.\n");
            }



            return Task.CompletedTask;
        }


        public Task ApplyAverageFilter()
        {

            return Task.CompletedTask;
        }


        private void FindMedian(byte[] input, int index)
        {
            List<byte> temp = new();

            for (int i = 0; i < mask; i++)
            {
                temp.Add(input[index - ((mask - 1) / 2) + i]);
            }

            temp.Sort();
            _yuv.YMedian[index - ((mask - 1) / 2)] = temp.ElementAt((mask - 1) / 2);

        }


        private void FindMedian2D(int i_index, int j_index)
        {
            List<byte> temp = new();

            // Find the correct block on given index and mask
            for (int i = i_index - ((mask - 1) / 2); i <= i_index + ((mask - 1) / 2); i++)
            {
                for (int j = j_index - ((mask - 1) / 2); j <= j_index + ((mask - 1) / 2); j++)
                {
                    temp.Add(_yuv.Yplane[i, j]);
                }
            }

            temp.Sort();
            _yuv.YMedian2D[i_index - ((mask - 1) / 2), j_index - ((mask - 1) / 2)] = temp.ElementAt((int)((Math.Pow(mask, 2) - 1) / 2));
        }


        #endregion



        #region Private Helper Methods

        /// <summary>
        /// Extends an one dimensional array at the left-most and right-most indexes according to selected mask.
        /// </summary>
        private byte[] Extend1DArray(byte[] input, int totalbytes)
        {

            byte[] outputExtended = new byte[totalbytes + (mask - 1)];


            for (int i = 0; i < outputExtended.Length; i++)
            {
                outputExtended[i] = i < (mask - 1) / 2 ? input[(mask - 1) / 2]
                                : i > input.Length - 1 ? input[^1] : input[i];
            }

            return outputExtended;
        }



        /// <summary>
        /// Converts the one dimensional byte array passed, to the equivalent two dimensional 
        /// non extended array at first and then extends it according to the selected mask.
        /// </summary>
        private byte[,] ConvertTo2DExtendedArray(byte[] input, int height, int width)
        {

            int count = -1;
            byte[,] output = new byte[height, width];
            byte[,] outputExtended = new byte[height + (mask - 1), width + (mask - 1)];



            // Copies the passed array to the new two dimensional non extended array   
            for (int i = 0; i < output.GetLength(0); i++)
            {
                for (int j = 0; j < output.GetLength(1); j++)
                {
                    count++;
                    if (count < input.Length)
                        output[i, j] = input[count];
                }
            }



            #region For testing purposes 

            //byte[,] output = new byte[,] {
            //                            { 1,4,0,1,3,1 },
            //                            { 2,2,4,2,2,3 },
            //                            { 1,0,1,0,1,0 },
            //                            { 1,2,1,0,2,2 },
            //                            { 2,5,3,1,2,5 },
            //                            { 1,1,4,2,3,0 }
            //    };

            //  extended array expected:  { 1,1,4,0,1,3,1,1 }
            //                            { 1,1,4,0,1,3,1,1 }
            //                            { 2,2,2,4,2,2,3,3 }
            //                            { 1,1,0,1,0,1,0,0 }
            //                            { 1,1,2,1,0,2,2,2 }
            //                            { 2,2,5,3,1,2,5,5 }
            //                            { 1,1,1,4,2,3,0,0 }
            //                            { 1,1,1,4,2,3,0,0 }


            //                    mask(3) : i + 2 / j + 2
            //                    mask(5) : i + 4 / j + 4
            //                    mask(7) : i + 6 / j + 6
            //                    mask(9) : i + 8 / j + 8

            //                    { X,X,X,X,X,X,X,X,X,X }
            //                    { X,X,X,X,X,X,X,X,X,X }
            //                    { X,X,1,4,0,1,3,1,X,X }
            //                    { X,X,2,2,4,2,2,3,X,X }
            //                    { X,X,1,0,1,0,1,0,X,X }
            //                    { X,X,1,2,1,0,2,2,X,X }
            //                    { X,X,2,5,3,1,2,5,X,X }
            //                    { X,X,1,1,4,2,3,0,X,X }
            //                    { X,X,X,X,X,X,X,X,X,X }
            //                    { X,X,X,X,X,X,X,X,X,X }   mask:5x5

            #endregion



            // Extend border (with extension size according to mask) values outside with values at boundary

            for (int i = 0; i < outputExtended.GetLength(0); i++)
            {
                for (int j = 0; j < outputExtended.GetLength(1); j++)
                {

                    // Only for first extended rows
                    if (i < (mask - 1) / 2)
                    {
                        outputExtended[i, j] = j <= ((mask - 1) / 2) ? output[0, 0]
                                                           : j >= outputExtended.GetLength(1) - 1 - ((mask - 1) / 2) ? output[0, output.GetLength(1) - 1]
                                                           : output[0, j - ((mask - 1) / 2)];

                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    //Only for last extended rows
                    else if (i >= outputExtended.GetLength(0) - ((mask - 1) / 2))
                    {

                        outputExtended[i, j] = j <= ((mask - 1) / 2) ? output[output.GetLength(0) - 1, 0]
                                                        : j >= outputExtended.GetLength(1) - 1 - ((mask - 1) / 2)
                                                        ? output[output.GetLength(0) - 1, output.GetLength(1) - 1]
                                                        : output[output.GetLength(0) - 1, j - ((mask - 1) / 2)];

                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // Only for first extended columns
                    else if (j < (mask - 1) / 2)
                    {
                        outputExtended[i, j] = output[i - ((mask - 1) / 2), 0];

                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // Only for last extended columns
                    else if (j == outputExtended.GetLength(1) - 1)
                    {
                        outputExtended[i, j] = output[i - ((mask - 1) / 2), output.GetLength(1) - 1];

                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // For every other element
                    else
                    {
                        if (i <= output.GetLength(0) && j <= output.GetLength(1))
                        {
                            outputExtended[i, j] = output[i - ((mask - 1) / 2), j - ((mask - 1) / 2)];
                            //Console.Write(outputExtended[i, j] + "  ");
                        }
                    }
                }

                // Array size is too big, thus can't fit any screen size,
                // every change of line is shown with a '+' sign to check boundaries values for debugging purposes
                //Console.WriteLine(" + ");

            }


            return outputExtended;
        }



        /// <summary>
        /// Converts the one dimensional byte array passed, to the equivalent two dimensional non extended array.
        /// </summary>
        private byte[,] ConvertTo2DArray(byte[] input, int height, int width)
        {

            int count = -1;
            byte[,] output = new byte[height, width];


            // Copies the passed array to the new two dimensional non extended array   
            for (int i = 0; i < output.GetLength(0); i++)
            {
                for (int j = 0; j < output.GetLength(1); j++)
                {
                    count++;
                    if (count < input.Length)
                        output[i, j] = input[count];
                }
            }


            return output;
        }



        /// <summary>
        /// Exits application depending on each of the user's input. 
        /// </summary>
        private static void UserExit(string result)
        {
            if (result is "exit")
                Environment.Exit(0);
        }

        #endregion


    }
}