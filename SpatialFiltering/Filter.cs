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
        public YuvModel _yuv;
        public readonly Helpers _helper;

        public Filter(YuvModel yuv, Helpers helper)
        {
            _yuv = yuv;
            _helper = helper;
            
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
                byte[] YbytesExtended = _helper.Extend1DArray(_yuv.Ybytes, _yuv.YTotalBytes);


                _helper.InformUser();


                Array.Resize(ref _yuv.YMedian, _yuv.YResolution);


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


                //_outputProvider("Task successfully completed.\n");
            }

            else if (_yuv.Dimensions is 2)
            {

                _helper.InformUser();


                Parallel.Invoke(
                    () => _yuv.Yplane = _helper.ConvertTo2DExtendedArray(_yuv.Ybytes, _yuv.YHeight, _yuv.YWidth),
                    () => _yuv.Uplane = _helper.ConvertTo2DArray(_yuv.Ubytes, _yuv.UHeight, _yuv.UWidth),
                    () => _yuv.Vplane = _helper.ConvertTo2DArray(_yuv.Vbytes, _yuv.VHeight, _yuv.VWidth));


                _yuv.YMedian2D = new byte[_yuv.YHeight, _yuv.YWidth];


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

            for (int i = 0; i < _yuv.Mask; i++)
            {
                temp.Add(input[index - ((_yuv.Mask - 1) / 2) + i]);
            }

            temp.Sort();
            _yuv.YMedian[index - ((_yuv.Mask - 1) / 2)] = temp.ElementAt((_yuv.Mask - 1) / 2);

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
            _yuv.YMedian2D[i_index - ((_yuv.Mask - 1) / 2), j_index - ((_yuv.Mask - 1) / 2)] = temp.ElementAt((int)((Math.Pow(_yuv.Mask, 2) - 1) / 2));
        }



    }
}
