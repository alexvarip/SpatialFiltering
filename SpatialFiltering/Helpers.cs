using System;

namespace SpatialFiltering
{
    public class Helpers
    {
        private readonly YuvModel _yuv;

        public Helpers(YuvModel yuv)
        {
            _yuv = yuv;
        }


        /// <summary>
        /// Extends an one dimensional array at the left-most and right-most indexes according to selected _yuv.Mask.
        /// </summary>
        public byte[] Extend1DArray(byte[] input, int totalbytes)
        {

            byte[] outputExtended = new byte[totalbytes + (_yuv.Mask - 1)];


            for (int i = 0; i < outputExtended.Length; i++)
            {
                outputExtended[i] = i < (_yuv.Mask - 1) / 2 ? input[(_yuv.Mask - 1) / 2]
                                : i > input.Length - 1 ? input[^1] : input[i];
            }

            return outputExtended;
        }



        /// <summary>
        /// Converts the one dimensional byte array passed, to the equivalent two dimensional 
        /// non extended array at first and then extends it according to the selected _yuv.Mask.
        /// </summary>
        public byte[,] ConvertTo2DExtendedArray(byte[] input, int height, int width)
        {

            int count = -1;
            byte[,] output = new byte[height, width];
            byte[,] outputExtended = new byte[height + (_yuv.Mask - 1), width + (_yuv.Mask - 1)];



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


            //                    _yuv.Mask(3) : i + 2 / j + 2
            //                    _yuv.Mask(5) : i + 4 / j + 4
            //                    _yuv.Mask(7) : i + 6 / j + 6
            //                    _yuv.Mask(9) : i + 8 / j + 8

            //                    { X,X,X,X,X,X,X,X,X,X }
            //                    { X,X,X,X,X,X,X,X,X,X }
            //                    { X,X,1,4,0,1,3,1,X,X }
            //                    { X,X,2,2,4,2,2,3,X,X }
            //                    { X,X,1,0,1,0,1,0,X,X }
            //                    { X,X,1,2,1,0,2,2,X,X }
            //                    { X,X,2,5,3,1,2,5,X,X }
            //                    { X,X,1,1,4,2,3,0,X,X }
            //                    { X,X,X,X,X,X,X,X,X,X }
            //                    { X,X,X,X,X,X,X,X,X,X }   _yuv.Mask:5x5

            #endregion



            // Extend border (with extension size according to _yuv.Mask) values outside with values at boundary

            for (int i = 0; i < outputExtended.GetLength(0); i++)
            {
                for (int j = 0; j < outputExtended.GetLength(1); j++)
                {

                    // Only for first extended rows
                    if (i < (_yuv.Mask - 1) / 2)
                    {
                        outputExtended[i, j] = j <= ((_yuv.Mask - 1) / 2) ? output[0, 0]
                                                           : j >= outputExtended.GetLength(1) - 1 - ((_yuv.Mask - 1) / 2) ? output[0, output.GetLength(1) - 1]
                                                           : output[0, j - ((_yuv.Mask - 1) / 2)];

                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    //Only for last extended rows
                    else if (i >= outputExtended.GetLength(0) - ((_yuv.Mask - 1) / 2))
                    {

                        outputExtended[i, j] = j <= ((_yuv.Mask - 1) / 2) ? output[output.GetLength(0) - 1, 0]
                                                        : j >= outputExtended.GetLength(1) - 1 - ((_yuv.Mask - 1) / 2)
                                                        ? output[output.GetLength(0) - 1, output.GetLength(1) - 1]
                                                        : output[output.GetLength(0) - 1, j - ((_yuv.Mask - 1) / 2)];

                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // Only for first extended columns
                    else if (j < (_yuv.Mask - 1) / 2)
                    {
                        outputExtended[i, j] = output[i - ((_yuv.Mask - 1) / 2), 0];

                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // Only for last extended columns
                    else if (j == outputExtended.GetLength(1) - 1)
                    {
                        outputExtended[i, j] = output[i - ((_yuv.Mask - 1) / 2), output.GetLength(1) - 1];

                        //Console.Write(outputExtended[i, j] + "  ");
                    }

                    // For every other element
                    else
                    {
                        if (i <= output.GetLength(0) && j <= output.GetLength(1))
                        {
                            outputExtended[i, j] = output[i - ((_yuv.Mask - 1) / 2), j - ((_yuv.Mask - 1) / 2)];
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
        public byte[,] ConvertTo2DArray(byte[] input, int height, int width)
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
        /// Informs user about a task status. 
        /// </summary>
        public void InformUser()
        {
            Console.Write("\n\n  Getting things ready...\n");
            Console.Write("  Filter is now being applied");
        }



        /// <summary>
        /// Exits application depending on the user at any user input point.
        /// </summary>
        public void UserExit(string result)
        {
            if (result is "exit")
                Environment.Exit(0);
        }


    }
}
