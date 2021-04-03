
namespace SpatialFiltering
{
    public class YuvModel
    {

        public int YWidth { get; set; }
        public int YHeight { get; set; }
        public int UWidth { get; set; }
        public int UHeight { get; set; }
        public int VWidth { get; set; }
        public int VHeight { get; set; }


        /// <summary>
        /// Total bytes of Y component.
        /// </summary>
        public int YTotalBytes { get; set; } //= 240 * 416;


        /// <summary>
        /// Total bytes of U component.
        /// </summary>
        public int UTotalBytes { get; set; } //= 120 * 208;


        /// <summary>
        /// Total bytes of V component.
        /// </summary>
        public int VTotalBytes { get; set; } //= 120 * 208;


        /// <summary>
        /// For implementing read/wite operations for a .yuv file with one dimensional byte array.
        /// </summary>
        public byte[] Ybytes, Ubytes, Vbytes;


        /// <summary>
        /// For implementing border extension from a given one dimensional byte array to two dimensional byte array.
        /// </summary>
        public byte[,] Yplane, Uplane, Vplane;


        /// <summary>
        /// One dimensional byte array filled with median values.
        /// </summary>
        public byte[] YMedian;


        /// <summary>
        /// Two dimensional array filled with the median values of a converted from one dimensional to two dimensional byte array.
        /// </summary>
        public byte[,] YMedian2D;


        /// <summary>
        /// Y component full resolution (width * height)
        /// </summary>
        public int YResolution { get { return YWidth * YHeight; } }


        /// <summary>
        /// U component full resolution (width * height)
        /// </summary>
        public int UResolution { get { return UWidth * UHeight; } }


        /// <summary>
        /// V component full resolution (width * height)
        /// </summary>
        public int VResolution { get { return VWidth * VHeight; } }


        
        private string _systemMessage = "";
        private int _mask;
        private int _dimensions;


        /// <summary>
        /// Display corresponding error message for a property, if any.
        /// </summary>
        public string SystemMessage
        {
            get 
            {
                return _systemMessage;
            }
            set 
            {
                _systemMessage = value;  
            }
        }



        public int Mask
        {
            get { return _mask; }
            set
            {
                if (value is not 1 && (value % 2 is 1))
                    _mask = value;
                else
                {
                    _mask = 3;
                    _systemMessage = $"  [SYSTEM] The mask size was successfully restored using the default value\n";
                }
                    
            }
        }



        public int Dimensions
        {
            get { return _dimensions; }
            set
            {
                if (value is 1 || value is 2)
                    _dimensions = value;
                else
                {
                    _dimensions = 2;
                    _systemMessage = $"  [SYSTEM] The dimension value was successfully restored using default\n";
                }
            }
        }



    }
}

