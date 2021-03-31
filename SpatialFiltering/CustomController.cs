using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpatialFiltering
{
    public class CustomController
    {
        private readonly Func<string> _inputProvider;
        private readonly Action<string> _outputProvider;
        private readonly YuvModel _yuv;
        private int value = 0;
        private int mask = 3;
#if DEBUG
        private string filepath = @"C:\Users\alexv\Downloads\HW1\HW1\BlowingBubbles_416x240.yuv";
        private string outpath1 = @"C:\Users\alexv\Downloads\HW1\HW1\BlowingBubbles_416x240_filtered_1D.yuv"; 
        private string outpath2 = @"C:\Users\alexv\Downloads\HW1\HW1\BlowingBubbles_416x240_filtered_2D.yuv";
#endif



        /// <summary>
        /// Custom controller using Dependecy Injection for handling input/output and given model.  
        /// </summary>
        public CustomController(Func<string> inputProvider, Action<string> outputProvider, YuvModel yuv)
        {
            _inputProvider = inputProvider;
            _outputProvider = outputProvider;
            _yuv = yuv;
        }


        /// <summary>
        /// Gets all the essential information and reads from a specified file.
        /// </summary>
        public CustomController Build()
        {
            
            GetInformation();

            if(ReadYuvComponents().IsCompletedSuccessfully)
                UserAction();
            
            return this;
        }

       

        /// <summary>
        /// Applies Median Filtering with selected window/mask size for the one or two dimensional implementations respectively.
        /// </summary>
        public CustomController ApplyMedianFilter()
        {
            bool q = false;
            bool w = false;


            if (value is 1)
            {

                InformUser();

                Array.Resize(ref _yuv.YMedian, _yuv.YResolution);

                int index = 1;

                Parallel.Invoke(
                                delegate () 
                                {
                                    while (index < _yuv.Ybytes.Length - 1)
                                    {
                                        q = true;

                                        FindMedian(index);
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
                                            Thread.Sleep(1000);
                                            if (dots == 2)
                                            {
                                                if (q == true)
                                                {
                                                    w = true;
                                                    break;
                                                }

                                                Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                                                dots = -1;
                                                Thread.Sleep(1000);
                                            }
                                        }

                                        if (w == true)
                                            break;
                                    }
                                }
                );


                _outputProvider("Task successfully completed.\n");

            }
            else if (value is 2)
            {

                InformUser();


                Parallel.Invoke(
                    () => _yuv.Yplane = ConvertTo2DExtendedArray(_yuv.Ybytes, _yuv.YWidth, _yuv.YHeight),
                    () => _yuv.Uplane = ConvertTo2DArray(_yuv.Ubytes, _yuv.UWidth, _yuv.UHeight),
                    () => _yuv.Vplane = ConvertTo2DArray(_yuv.Vbytes, _yuv.VWidth, _yuv.VHeight));


                _yuv.YMedian2D = new byte[_yuv.YWidth, _yuv.YHeight];


                Parallel.Invoke(
                                delegate ()
                                {
                                    q = true;

                                    for (int i = 1; i < _yuv.Yplane.GetLength(0) - 1; i++)
                                    {
                                        for (int j = 1; j < _yuv.Yplane.GetLength(1) - 1; j++)
                                        {
                                            FindMedian2D(i, j);
                                        }
                                    }
                                },

                                 delegate ()
                                 {
                                     while (true)
                                     {
                                         for (int dots = 0; dots < 3; ++dots)
                                         {
                                             Console.Write('.');
                                             Thread.Sleep(1000);
                                             if (dots == 2)
                                             {
                                                 if (q == true)
                                                 {
                                                     w = true;
                                                     break;
                                                 }

                                                 Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                                                 dots = -1;
                                                 Thread.Sleep(1000);
                                             }
                                         }

                                         if (w == true)
                                             break;
                                     }
                                 }

                    );


                _outputProvider("Task successfully completed.\n");

            }


            return this;
        }


        /// <summary>
        /// Writes y, u, v byte arrays to a new .yuv file with one or two dimensional array implementation.
        /// </summary>
        public CustomController Out()
        {
            if (value is 1)
            {
                // Write all component byte arrays to a new .yuv file with 1D array implementation.
                using (FileStream fsNew = new FileStream(outpath1, FileMode.Create, FileAccess.Write))
                {
                    for (int i = 0; i < _yuv.YMedian.Length; i++)
                    {
                        fsNew.WriteByte(_yuv.YMedian[i]);
                    }

                    for (int i = 0; i < _yuv.Ubytes.Length; i++)
                    {
                        fsNew.WriteByte(_yuv.Ubytes[i]);
                    }

                    for (int i = 0; i < _yuv.Vbytes.Length; i++)
                    {
                        fsNew.WriteByte(_yuv.Vbytes[i]);
                    }
                }
            }
            else if (value is 2)
            {

                // Write all component byte arrays to a new .yuv file with 2D array implementation.
                using (FileStream fsNew = new FileStream(outpath2, FileMode.Create, FileAccess.Write))
                {
                    for (int i = 0; i < _yuv.YMedian2D.GetLength(0); i++)
                    {
                        for (int j = 0; j < _yuv.YMedian2D.GetLength(1); j++)
                        {
                            fsNew.WriteByte(_yuv.YMedian2D[i, j]);
                        }
                    }

                    for (int i = 0; i < _yuv.Uplane.GetLength(0); i++)
                    {
                        for (int j = 0; j < _yuv.Uplane.GetLength(1); j++)
                        {
                            fsNew.WriteByte(_yuv.Uplane[i, j]);
                        }
                    }

                    for (int i = 0; i < _yuv.Vplane.GetLength(0); i++)
                    {
                        for (int j = 0; j < _yuv.Vplane.GetLength(1); j++)
                        {
                            fsNew.WriteByte(_yuv.Vplane[i, j]);
                        }
                    }
                }
            }



            //int count = 0;
            ////////////////////////////////
            //while (count < _yuv.Ybytes.Length)
            //{
            //    for (int i = 0; i < _yuv.Yplane.GetLength(0); i++)
            //    {
            //        for (int j = 0; j < _yuv.Yplane.GetLength(1); j++)
            //        {
            //            if (_yuv.Yplane[i, j] == _yuv.Ybytes[count])
            //            {
            //                Console.WriteLine( $"[{i},{j}] = {count} ");
            //            }
            //            count++;
            //        }
            //        Console.WriteLine();
            //    }
            //}

          

            return this;
        }




        private void GetInformation()
        {

            _outputProvider("\n ┌─────────────────────────┐ ");
            _outputProvider("\n ▓   YUV File Components   ▓ ");
            _outputProvider("\n ▓          Y U V          ▓ ");
            _outputProvider("\n └─────────────────────────┘ ");

            _outputProvider("\n\n  <Y component resolution>");
            _outputProvider("\n  width: ");
            var result = _inputProvider();
            if (int.TryParse(result, out value))
                _yuv.YWidth = value;

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


            Array.Resize(ref _yuv.Ybytes, _yuv.YResolution + 2);
            Array.Resize(ref _yuv.Ubytes, _yuv.UResolution + 2);
            Array.Resize(ref _yuv.Vbytes, _yuv.VResolution + 2);


            FileProperties();
        }

        
        private void FileProperties()
        {
            Path.GetFileName(@"C:\Users\alexv\Downloads\HW1\HW1");

            _outputProvider("\n");
            _outputProvider("\n ┌──────────────────────┐ ");
            _outputProvider("\n ░   YUV File Details   ▓ ");
            _outputProvider("\n ▓        Y U V         ░ ");
            _outputProvider("\n └──────────────────────┘ ");
            _outputProvider("\n");
            _outputProvider($"\n  Resolution: {_yuv.YWidth} x {_yuv.YHeight}");
            _outputProvider($"\n  Width: {_yuv.YWidth} pixels");
            _outputProvider($"\n  Height: {_yuv.YHeight} pixels");
            _outputProvider($"\n  Item Type: {Path.GetExtension(filepath).ToUpper()} File");
            _outputProvider($"\n  Folder Path: {Path.GetDirectoryName(filepath)}");
            _outputProvider($"\n  Date Created: {File.GetCreationTime(filepath)}");
            _outputProvider($"\n  Date Modified: {File.GetLastAccessTime(filepath)}");
            _outputProvider($"\n  Size: {_yuv.YTotalBytes} KB");
            _outputProvider($"\n  Owner: {Environment.UserName}");
            _outputProvider($"\n  Computer: {Environment.MachineName}");
        }


        private Task ReadYuvComponents()
        {
            try
            {

                using (FileStream fsSource = new(filepath, FileMode.Open, FileAccess.Read))
                {

                    // Write y component into a byte array.
                    int numBytesRead = 1;
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

                    // Extend array at the left-most and right-most elements
                    _yuv.Ybytes[0] = _yuv.Ybytes[1];
                    _yuv.Ybytes[_yuv.YTotalBytes - 1] = _yuv.Ybytes[_yuv.YTotalBytes - 2];


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
            catch (FileNotFoundException ioEx)
            {
                Console.Clear();
                Console.WriteLine($"{ioEx.Message}");
                
                return Task.FromException(ioEx);
            }

        }


        private void UserAction()
        {

            // ▒▓░ 

            _outputProvider("\n\n");    
            _outputProvider("\n ┌───────────────────────────────┐ ");
            _outputProvider("\n ▒   YUV Implementation Method   ▒ ");
            _outputProvider("\n ▒            Y U V              ▒ ");
            _outputProvider("\n └───────────────────────────────┘ ");

            _outputProvider("\n\n  Do you wish to implement the creation task using 1 or 2 dimensional arrays: ");

            var result = _inputProvider();
            _ = int.TryParse(result, out value);

            if (value is 1)
                _outputProvider("\n  Please specify window size (i.e. 3):\n\n  Usage: [integer]\n  Window: 3 [default]");
            else if (value is 2)
                _outputProvider("\n  Please specify mask size (i.e. 3x3):\n  Mask: 3 [default]");


            Console.ReadKey();
            _outputProvider("\r" + new string(' ', Console.WindowWidth) + "\r");
            _outputProvider("  Your preference: ");
            
            result = _inputProvider();
            _ = int.TryParse(result, out mask);
            
        }


        private void FindMedian(int index)
        {
            List<byte> temp = new();

            for (int i = 0; i < mask; i++)
            {
                if (index is 1)
                    temp.Add(_yuv.Ybytes[index - 1 + i]);
                else
                    temp.Add(_yuv.Ybytes[index - ((mask - 1) / 2) + i]);
            }

            temp.Sort();
            _yuv.YMedian[index - 1] = temp.ElementAt(((mask - 1) / 2));

        }


        private void FindMedian2D(int i_index, int j_index)
        {
            List<byte> temp = new();


            // Find the correct 3x3 block on given index value
            for (int i = i_index - 1; i <= i_index + 1; i++)
            {
                for (int j = j_index - 1; j <= j_index + 1; j++)
                {
                    temp.Add(_yuv.Yplane[i, j]);
                }
            }

            temp.Sort();
            _yuv.YMedian2D[i_index - 1, j_index - 1] = temp.ElementAt((int)((Math.Pow(mask, 2) - 1) / 2));
        }







        /// <summary>
        /// Converts the one dimensional extended byte array passed, to the equivalent two dimensional 
        /// non extended array at first and then extends it correctly.
        /// </summary>
        private static byte[,] ConvertTo2DExtendedArray(byte[] input, int width, int height)
        {

            int count = 0;
            byte[,] output = new byte[width, height];
            byte[,] outputExtended = new byte[width + 2, height + 2];



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

            //byte[,] a = new byte[,] {
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

            #endregion



            // Extend border values outside with values at boundary
            //Console.WriteLine("extended:");


            for (int i = 0; i < outputExtended.GetLength(0); i++)
            {
                for (int j = 0; j < outputExtended.GetLength(1); j++)
                {

                    // Only for first row
                    if (i is 0)
                    {
                        outputExtended[i, j] = j is 0  ? output[i, j] 
                                                       : j == outputExtended.GetLength(1) - 1  ? output[i, j - 2] 
                                                       : output[i, j - 1];


                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // Only for last row
                    else if (i == outputExtended.GetLength(0) - 1)
                    {

                        outputExtended[i, j] = j is 0  ? output[i - 2, j] 
                                                       : j == outputExtended.GetLength(1) - 1 ? output[i - 2, j - 2]
                                                       : output[i - 2, j - 1];


                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // Only for first column
                    else if (j is 0)
                    {
                        outputExtended[i, j] = output[i - 1, j];
                        
                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // Only for last column
                    else if (j == outputExtended.GetLength(1) - 1)
                    {
                        outputExtended[i, j] = output[i -1 , j - 2];

                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // For every other element
                    else 
                    {
                        if (i <= output.GetLength(0) && j <= output.GetLength(1))
                        {
                            outputExtended[i, j] = output[i - 1, j - 1];
                            //Console.Write(outputExtended[i, j] + "  "); 
                        }
                        
                        
                    }
                }

                // Array size is too big, thus can't fit any screen size,
                // every change of line is show with a '+' sign for debugging purposes
                //Console.WriteLine(" + ");

            }


          
            return outputExtended;
        }



        /// <summary>
        /// Converts the one dimensional byte array passed, to the equivalent two dimensional non extended array.
        /// </summary>
        private static byte[,] ConvertTo2DArray(byte[] input, int width, int height)
        {

            int count = -1;
            byte[,] output = new byte[width, height];
            

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



        private void InformUser()
        {
            _outputProvider("\n\n  Getting things ready...\n");
            _outputProvider($"  Filter is now being applied");
        }


    } 
}