using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixFourThree.BoxPacker.Model
{
    public class Item : IComparable<Item>
    {
        /// <summary>
        /// Identifier for the item
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Reference number (i.e. SKU, description, etc.)
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Width in mm
        /// </summary>
        public Int32 Width { get; set; }

        /// <summary>
        /// Length in mm
        /// </summary>
        public Int32 Length { get; set; }

        /// <summary>
        /// Depth in mm
        /// </summary>
        public Int32 Depth { get; set; }

        /// <summary>
        /// Weight in g
        /// </summary>
        public Int32 Weight { get; set; }

        /// <summary>
        /// Volume in mm^3
        /// </summary>
        public Int32 Volume { get { return Length * Depth * Width; } }

        public int CompareTo(Item other)
        {
            if (Volume > other.Volume)
                return 1;

            if (Volume < other.Volume)
                return -1;

            return 0;
        }
    }
}
