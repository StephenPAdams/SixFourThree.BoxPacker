using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixFourThree.BoxPacker.Model
{
    public class Box : IComparable<Box>
    {
        /// <summary>
        /// Reference number (i.e. SKU, description, etc.)
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Outer width in mm
        /// </summary>
        public Int32 OuterWidth { get; set; }

        /// <summary>
        /// Outer length in mm
        /// </summary>
        public Int32 OuterLength { get; set; }

        /// <summary>
        /// Outer depth in mm
        /// </summary>
        public Int32 OuterDepth { get; set; }

        /// <summary>
        /// Empty weight in g
        /// </summary>
        public Int32 EmptyWeight { get; set; }

        /// <summary>
        /// Inner width in mm
        /// </summary>
        public Int32 InnerWidth { get; set; }

        /// <summary>
        /// Inner length in mm
        /// </summary>
        public Int32 InnerLength { get; set; }

        /// <summary>
        /// Inner depth in mm
        /// </summary>
        public Int32 InnerDepth { get; set; }

        /// <summary>
        /// Inner volume in mm^3
        /// </summary>
        public Int32 InnerVolume { get { return InnerWidth * InnerLength * InnerDepth; } }

        /// <summary>
        /// Max weight the package can hold in g
        /// </summary>
        public Int32 MaxWeight { get; set; }

        public int CompareTo(Box other)
        {
            if (other.InnerVolume > InnerVolume)
                return -1;
            
            if (other.InnerVolume < InnerVolume) 
                return 1;
            
            return 0;
        }
    }
}
