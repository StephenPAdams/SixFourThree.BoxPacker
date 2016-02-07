using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixFourThree.BoxPacker.Helpers;

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

        public Double WidthInInches => ConversionHelper.ConvertMillimetersToInches(Width);

        /// <summary>
        /// Length in mm
        /// </summary>
        public Int32 Length { get; set; }

        public Double LengthInInches => ConversionHelper.ConvertMillimetersToInches(Length);

        /// <summary>
        /// Depth in mm
        /// </summary>
        public Int32 Depth { get; set; }

        public Double DepthInInches => ConversionHelper.ConvertMillimetersToInches(Depth);

        /// <summary>
        /// Weight in g
        /// </summary>
        public Int32 Weight { get; set; }

        public Double WeightInPounds => ConversionHelper.ConvertPoundsToGrams(Weight);

        /// <summary>
        /// Volume in mm^3
        /// </summary>
        public Int32 Volume { get { return Length * Depth * Width; } }

        public Double VolumeInInches => LengthInInches * DepthInInches * WidthInInches;

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
