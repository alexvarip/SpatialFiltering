using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialFiltering
{
    public class YuvModel
    {
        /// <summary>
        /// Total bytes of Y component
        /// </summary>
        public int YTotalBytes { get; set; } //= 240 * 416;


        /// <summary>
        /// Total bytes of U component
        /// </summary>
        public int UTotalBytes { get; set; } //= 120 * 208;


        /// <summary>
        /// Total bytes of V component
        /// </summary>
        public int VTotalBytes { get; set; } //= 120 * 208;


        /// <summary>
        /// For implementing read/wite operations for a .yuv file with one dimensional byte array
        /// </summary>
        public byte[] Ybytes, Ubytes, Vbytes;


        /// <summary>
        /// For implementing border extension from a given one dimensional byte array with two dimensional byte array
        /// </summary>
        public byte[,] Yplane, Uplane, Vplane;


        /// <summary>
        /// Array filled with the median values of the one dimensional byte array
        /// </summary>
        public byte[] YMedian;


        /// <summary>
        /// Array filled with the median values of the converted two dimensional byte array
        /// </summary>
        public byte[,] YMedian2D;


        public int YWidth { get; set; }
        public int YHeight { get; set; }
        public int UWidth { get; set; }
        public int UHeight { get; set; }
        public int VWidth { get; set; }
        public int VHeight { get; set; }


        /// <summary>
        /// Y component full resolution (width * height)
        /// </summary>
        public int YResolution
        {
            get { return YWidth * YHeight; }
        }


        /// <summary>
        /// U component full resolution (width * height)
        /// </summary>
        public int UResolution
        {
            get { return UWidth * UHeight; }
        }


        /// <summary>
        /// V component full resolution (width * height)
        /// </summary>
        public int VResolution
        {
            get { return VWidth * VHeight; }
        }



    }
}

