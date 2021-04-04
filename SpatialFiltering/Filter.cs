using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpatialFiltering
{
    public class Filter
    {

        private readonly Dictionary<string, Action> _filters;
        private readonly YuvModel _yuv;
        private readonly Helpers _helpers;

        public Filter(YuvModel yuv, Helpers helpers)
        {
            _yuv = yuv;
            _helpers = helpers;
            
            _filters = new Dictionary<string, Action>()
            {
                { "Median", () => { if (ApplyMedianFilter().IsCompletedSuccessfully)
                                        Console.Write("Task successfully completed.\n"); }
                },

                { "Average", () => { if (ApplyAverageFilter().IsCompletedSuccessfully)
                                        Console.Write("Task successfully completed.\n"); }
                },
                
                { "Laplace", () => { } }
            };

        }


        public IEnumerable<string> GetAvailableFilters()
        {
            return _filters.Keys;
        }



        public Action GetValue(string key)
        {
            key = char.ToUpper(key[0]) + key.Substring(1);

            if (!_filters.TryGetValue(key, out Action value)) { }

            return value;
        }



        public Task ApplyMedianFilter()
        {

            bool q = false;
            bool w = false;

            var (Left, Top) = Console.GetCursorPosition();

            if (_yuv.Dimensions is 1)
            {
                byte[] YbytesExtended = _helpers.Extend1DArray(_yuv.Ybytes, _yuv.YTotalBytes);


                _helpers.InformUser();


                Array.Resize(ref _yuv.YFiltered, _yuv.YResolution);


                Parallel.Invoke(
                                delegate ()
                                {

                                    int index = (_yuv.Mask - 1) / 2;

                                    while (index < YbytesExtended.Length - ((_yuv.Mask - 1) / 2))
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

            }

            else if (_yuv.Dimensions is 2)
            {

                _helpers.InformUser();


                Parallel.Invoke(
                    () => _yuv.Yplane = _helpers.ConvertTo2DExtendedArray(_yuv.Ybytes, _yuv.YHeight, _yuv.YWidth),
                    () => _yuv.Uplane = _helpers.ConvertTo2DArray(_yuv.Ubytes, _yuv.UHeight, _yuv.UWidth),
                    () => _yuv.Vplane = _helpers.ConvertTo2DArray(_yuv.Vbytes, _yuv.VHeight, _yuv.VWidth));


                _yuv.YFiltered2D = new byte[_yuv.YHeight, _yuv.YWidth];


                Parallel.Invoke(
                                delegate ()
                                {

                                    for (int i = ((_yuv.Mask - 1) / 2); i < _yuv.Yplane.GetLength(0) - ((_yuv.Mask - 1) / 2); i++)
                                    {
                                        for (int j = ((_yuv.Mask - 1) / 2); j < _yuv.Yplane.GetLength(1) - ((_yuv.Mask - 1) / 2); j++)
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

            }



            return Task.CompletedTask;
        }



        public Task ApplyAverageFilter()
        {
            bool q = false;
            bool w = false;


            if (_yuv.Dimensions is 1)
            {
                byte[] YbytesExtended = _helpers.Extend1DArray(_yuv.Ybytes, _yuv.YTotalBytes);


                _helpers.InformUser();


                Array.Resize(ref _yuv.YFiltered, _yuv.YResolution);


                Parallel.Invoke(
                                delegate ()
                                {

                                    int index = (_yuv.Mask - 1) / 2;

                                    while (index < YbytesExtended.Length - ((_yuv.Mask - 1) / 2))
                                    {
                                        q = true;

                                        FindAverage(YbytesExtended, index);
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

                                                // Console.SetCursorPosition(Left, Top);
                                                Console.Write(new string(' ', 3));
                                                //Console.SetCursorPosition(Left, Top);

                                                dots = -1;
                                                Thread.Sleep(500);
                                            }
                                        }

                                        if (w == true)
                                            break;
                                    }
                                }
                );


            }

            else if (_yuv.Dimensions is 2)
            {

                _helpers.InformUser();


                Parallel.Invoke(
                    () => _yuv.Yplane = _helpers.ConvertTo2DExtendedArray(_yuv.Ybytes, _yuv.YHeight, _yuv.YWidth),
                    () => _yuv.Uplane = _helpers.ConvertTo2DArray(_yuv.Ubytes, _yuv.UHeight, _yuv.UWidth),
                    () => _yuv.Vplane = _helpers.ConvertTo2DArray(_yuv.Vbytes, _yuv.VHeight, _yuv.VWidth));


                _yuv.YFiltered2D = new byte[_yuv.YHeight, _yuv.YWidth];


                Parallel.Invoke(
                                delegate ()
                                {

                                    for (int i = ((_yuv.Mask - 1) / 2); i < _yuv.Yplane.GetLength(0) - ((_yuv.Mask - 1) / 2); i++)
                                    {
                                        for (int j = ((_yuv.Mask - 1) / 2); j < _yuv.Yplane.GetLength(1) - ((_yuv.Mask - 1) / 2); j++)
                                        {
                                            FindAverage2D(i, j);
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

                                                //Console.SetCursorPosition(Left, Top);
                                                Console.Write(new string(' ', 3));
                                                //Console.SetCursorPosition(Left, Top);

                                                dots = -1;
                                                Thread.Sleep(500);
                                            }
                                        }

                                        if (w == true)
                                            break;
                                    }
                                }
                );

            }


                return Task.CompletedTask;
        }



        private void FindMedian(byte[] input, int index)
        {
            List<byte> temp = new();

            for (int i = 0; i < _yuv.Mask; i++)
            {
                temp.Add(input[index - ((_yuv.Mask - 1) / 2) + i]);
            }

            temp.Sort();
            _yuv.YFiltered[index - ((_yuv.Mask - 1) / 2)] = temp.ElementAt((_yuv.Mask - 1) / 2);
        }



        private void FindMedian2D(int i_index, int j_index)
        {
            List<byte> temp = new();

            // Find the correct block on given index and _mask
            for (int i = i_index - ((_yuv.Mask - 1) / 2); i <= i_index + ((_yuv.Mask - 1) / 2); i++)
            {
                for (int j = j_index - ((_yuv.Mask - 1) / 2); j <= j_index + ((_yuv.Mask - 1) / 2); j++)
                {
                    temp.Add(_yuv.Yplane[i, j]);
                }
            }

            temp.Sort();
            _yuv.YFiltered2D[i_index - ((_yuv.Mask - 1) / 2), j_index - ((_yuv.Mask - 1) / 2)] = temp.ElementAt((int)((Math.Pow(_yuv.Mask, 2) - 1) / 2));
        }



        private void FindAverage(byte[] input, int index)
        {
            int temp = 0;
            int i;

            for (i = 0; i < _yuv.Mask; i++)
                temp += input[index - ((_yuv.Mask - 1) / 2) + i];

            temp /= i;

            _yuv.YFiltered[index - ((_yuv.Mask - 1) / 2)] = (byte)temp;
        }



        private void FindAverage2D(int i_index, int j_index)
        {
            double temp = 0;

            // Find the correct block on given index and mask
            for (int i = i_index - ((_yuv.Mask - 1) / 2); i <= i_index + ((_yuv.Mask - 1) / 2); i++)
            {
                for (int j = j_index - ((_yuv.Mask - 1) / 2); j <= j_index + ((_yuv.Mask - 1) / 2); j++)
                {
                    temp += _yuv.Yplane[i, j];
                }
            }

            temp /= Math.Pow(_yuv.Mask, 2);

            _yuv.YFiltered2D[i_index - ((_yuv.Mask - 1) / 2), j_index - ((_yuv.Mask - 1) / 2)] = (byte)temp;
        }



    }
}
