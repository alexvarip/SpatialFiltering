using System;

namespace SpatialFiltering
{
    public class CustomController
    {

        private readonly Func<string> _inputProvider;
        private readonly Action<string> _outputProvider;
        private readonly ConfigurationMethods _config;
        private string _outfilepath = "";



        /// <summary>
        /// Custom controller constructor using Dependecy Injection for handling console input/output and given yuv model.  
        /// </summary>
        public CustomController(Func<string> inputProvider, Action<string> outputProvider, ConfigurationMethods config)
        {
            _inputProvider = inputProvider;
            _outputProvider = outputProvider;
            _config = config;
        }



        /// <summary>
        /// Reads from a .yuv file and gets all the essential information about it.
        /// </summary>
        public CustomController Build()
        {

            if (Program.keepInstancesAlive is "yes")
            {
                _config.UserAction();

                return this;
            }

            _config.GetInformation();

            if(_config.ReadFile().IsCompletedSuccessfully)
                _config.UserAction();
            
            
            return this;
        }


        
        /// <summary>
        /// Applies the selected from the user spatial Filter with the specified window/mask size with either one or two dimensional implementations.
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

            _outfilepath = _config.CreateFilePath();

            _config.WriteToFile();

            _outputProvider($"\n\n  Your file is ready to use at the following path:\n  {_outfilepath}");


            return this;
        }
       
        
    }
}