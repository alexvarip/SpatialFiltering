using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpatialFiltering
{
    public class CustomController
    {

        private readonly Func<string> _inputProvider;
        private readonly Action<string> _outputProvider;
        private readonly ConfigurationMethods _config;
        public readonly Dictionary<string, Action> filters;
        private string _outfilepath = "";



        /// <summary>
        /// Custom controller constructor using Dependecy Injection for handling console input/output and given model.  
        /// </summary>
        public CustomController(Func<string> inputProvider, Action<string> outputProvider, ConfigurationMethods config)
        {
            _inputProvider = inputProvider;
            _outputProvider = outputProvider;
            _config = config;

            filters = new Dictionary<string, Action>()
            {
                { "median", () => { if (_config.ApplyMedianFilter().IsCompletedSuccessfully) 
                                        _outputProvider("Task successfully completed.\n"); }
                },

                { "average", () => { if (_config.ApplyAverageFilter().IsCompletedSuccessfully)
                                        _outputProvider("Task successfully completed.\n"); }
                }};
        }



        /// <summary>
        /// Reads from a specified .yuv file and gets all the essential information about it.
        /// </summary>
        public CustomController Build()
        {
            _config.GetInformation();

            if(_config.ReadYuvComponents().IsCompletedSuccessfully)
                _config.UserAction();
            
            return this;
        }


        
        /// <summary>
        /// Applies the selected from the user Filter with the specified window/mask size with the one or two dimensional implementations.
        /// </summary>
        public CustomController ApplyFilter(Action action)
        {
            action.Invoke();

            return this;
        }

        

        /// <summary>
        /// Writes the y, u, v byte arrays to a new .yuv file with one or two dimensional array implementation.
        /// </summary>
        public CustomController Out()
        {

            _outfilepath = _config.CreateOutPath();


            if (_config.value is 1)
            {
                // Write all component byte arrays to a new .yuv file with 1D array implementation.
                using (FileStream fsNew = new FileStream(_outfilepath, FileMode.Create, FileAccess.Write))
                {
                    for (int i = 0; i < _config._yuv.YMedian.Length; i++)
                    {
                        fsNew.WriteByte(_config._yuv.YMedian[i]);
                    }

                    for (int i = 0; i < _config._yuv.Ubytes.Length; i++)
                    {
                        fsNew.WriteByte(_config._yuv.Ubytes[i]);
                    }

                    for (int i = 0; i < _config._yuv.Vbytes.Length; i++)
                    {
                        fsNew.WriteByte(_config._yuv.Vbytes[i]);
                    }
                }
            }
            else if (_config.value is 2)
            {

                // Write all component byte arrays to a new .yuv file with 2D array implementation.
                using (FileStream fsNew = new FileStream(_outfilepath, FileMode.Create, FileAccess.Write))
                {
                    for (int i = 0; i < _config._yuv.YMedian2D.GetLength(0); i++)
                    {
                        for (int j = 0; j < _config._yuv.YMedian2D.GetLength(1); j++)
                        {
                            fsNew.WriteByte(_config._yuv.YMedian2D[i, j]);
                        }
                    }

                    for (int i = 0; i < _config._yuv.Uplane.GetLength(0); i++)
                    {
                        for (int j = 0; j < _config._yuv.Uplane.GetLength(1); j++)
                        {
                            fsNew.WriteByte(_config._yuv.Uplane[i, j]);
                        }
                    }

                    for (int i = 0; i < _config._yuv.Vplane.GetLength(0); i++)
                    {
                        for (int j = 0; j < _config._yuv.Vplane.GetLength(1); j++)
                        {
                            fsNew.WriteByte(_config._yuv.Vplane[i, j]);
                        }
                    }
                }
            }


            _outputProvider($"\n\n  Your file is ready to use at the following path:\n  {_outfilepath}");


            return this;
        }
       
        
    }
}