using System;
using System.IO;
using System.Threading.Tasks;

namespace SpatialFiltering
{
    public class ConfigurationMethods
    {

        private readonly Func<string> _inputProvider;
        private readonly Action<string> _outputProvider;
        public readonly Filter _filters;
        private readonly Helpers _helper;
        private string _outfilepath = "";


        public ConfigurationMethods(Func<string> inputProvider, Action<string> outputProvider, Filter filters, Helpers helper)
        {
            _inputProvider = inputProvider;
            _outputProvider = outputProvider;
            _filters = filters;
            _helper = helper;
        }



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
            _outputProvider("\n └─────────────────────────┘ \n\n");


            _outputProvider("  <Y component resolution>");
            _outputProvider("\n  width: "); var (Left, Top) = Console.GetCursorPosition();
            
            var result = _inputProvider();
            if (int.TryParse(result, out int value))
                _filters._yuv.YWidth = value;

             _helper.UserExit(result);

            _outputProvider("  height: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _filters._yuv.YHeight = value;

            _helper.UserExit(result);

            _outputProvider("\n  <U component resolution>");
            _outputProvider("\n  width: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _filters._yuv.UWidth = value;

            _helper.UserExit(result);

            _outputProvider("  height: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _filters._yuv.UHeight = value;

            _helper.UserExit(result);

            _outputProvider("\n  <V component resolution>");
            _outputProvider("\n  width: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _filters._yuv.VWidth = value;

            _helper.UserExit(result);

            _outputProvider("  height: ");
            result = _inputProvider();
            if (int.TryParse(result, out value))
                _filters._yuv.VHeight = value;

            _helper.UserExit(result);

            _filters._yuv.YTotalBytes = _filters._yuv.YResolution;
            _filters._yuv.UTotalBytes = _filters._yuv.UResolution;
            _filters._yuv.VTotalBytes = _filters._yuv.VResolution;



            Array.Resize(ref _filters._yuv.Ybytes, _filters._yuv.YResolution);
            Array.Resize(ref _filters._yuv.Ubytes, _filters._yuv.UResolution);
            Array.Resize(ref _filters._yuv.Vbytes, _filters._yuv.VResolution);


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
            _outputProvider($"\n  Resolution: {_filters._yuv.YWidth} x {_filters._yuv.YHeight}");
            _outputProvider($"\n  Width: {_filters._yuv.YWidth} pixels");
            _outputProvider($"\n  Height: {_filters._yuv.YHeight} pixels");
            _outputProvider($"\n  Item Type: {Path.GetExtension(Program.filepath).ToUpper()} File");
            _outputProvider($"\n  Folder Path: {Path.GetDirectoryName(Program.filepath)}");
            _outputProvider($"\n  Date Created: {File.GetCreationTime(Program.filepath)}");
            _outputProvider($"\n  Date Modified: {File.GetLastAccessTime(Program.filepath)}");
            _outputProvider($"\n  Size: {_filters._yuv.YTotalBytes} KB");
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
                    while (_filters._yuv.YTotalBytes > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(_filters._yuv.Ybytes, numBytesRead, _filters._yuv.YTotalBytes);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        _filters._yuv.YTotalBytes -= n;
                    }
                    _filters._yuv.YTotalBytes = _filters._yuv.Ybytes.Length;


                    // Write u component into a byte array.
                    numBytesRead = 0;
                    while (_filters._yuv.UTotalBytes > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(_filters._yuv.Ubytes, numBytesRead, _filters._yuv.UTotalBytes);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        _filters._yuv.UTotalBytes -= n;
                    }
                    _filters._yuv.UTotalBytes = _filters._yuv.Ubytes.Length;


                    // Write v component into a byte array.
                    numBytesRead = 0;
                    while (_filters._yuv.VTotalBytes > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(_filters._yuv.Vbytes, numBytesRead, _filters._yuv.VTotalBytes);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        _filters._yuv.VTotalBytes -= n;
                    }
                    _filters._yuv.VTotalBytes = _filters._yuv.Vbytes.Length;


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
           
            _outputProvider("\n\n  Select array implementation method between 1 and 2 dimensions\n  > ");

            result = _inputProvider() ?? string.Empty;

            _helper.UserExit(result);


            // Get correct dimension value
            _ = int.TryParse(result, out int value);

            _filters._yuv.Dimensions = value;
            if (_filters._yuv.SystemMessage is not "")
            {
                var pos = Console.GetCursorPosition();
                Console.SetCursorPosition(pos.Left, pos.Top - 1);

                _outputProvider("\r" + new string(' ', Console.WindowWidth) + "\r");
                Console.SetCursorPosition(pos.Left + 2, pos.Top - 1);

                _outputProvider($"> {_filters._yuv.Dimensions}\n");
                _outputProvider(_filters._yuv.SystemMessage);
            }


            _filters._yuv.SystemMessage = "";



            _outputProvider("\n\n  Please select window/mask size  [Usage: [integer [default = 3]] \n  [Press Enter to continue]");
            Console.CursorVisible = false;

            Console.ReadKey();
            _outputProvider("\r" + new string(' ', Console.WindowWidth) + "\r");
            _outputProvider("  Your preference: ");
            Console.CursorVisible = true;


            result = _inputProvider();

            _helper.UserExit(result);


            // Get correct mask value
            _ = int.TryParse(result, out value);

            _filters._yuv.Mask = value;
            if (_filters._yuv.SystemMessage is not "")
            {
                var pos = Console.GetCursorPosition();
                Console.SetCursorPosition(pos.Left, pos.Top - 1);

                _outputProvider("\r" + new string(' ', Console.WindowWidth) + "\r");
                _outputProvider($"  Your preference: {_filters._yuv.Mask}\n");
                _outputProvider(_filters._yuv.SystemMessage);
            }


            _filters._yuv.SystemMessage = "";
        }


        public string CreateOutPath()
        {
            string path = $"{Environment.CurrentDirectory}/generated";
            int mask = _filters._yuv.Mask;
            int dimensions = _filters._yuv.Dimensions;


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory($"{Environment.CurrentDirectory}/generated");
                _outfilepath = $"{path}/BlowingBubbles_416x240_filtered_{mask}x{mask}_{dimensions}D.yuv";
            }
            else
                _outfilepath = $"{Environment.CurrentDirectory}/generated/BlowingBubbles_416x240_filtered_{mask}x{mask}_{dimensions}D.yuv";

            return _outfilepath;
        }




    }
}
