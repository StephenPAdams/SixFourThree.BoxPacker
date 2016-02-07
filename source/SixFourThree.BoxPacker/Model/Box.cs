using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixFourThree.BoxPacker.Helpers;

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

        public Double OuterWidthInInches => ConversionHelper.ConvertMillimetersToInches(OuterWidth);

        /// <summary>
        /// Outer length in mm
        /// </summary>
        public Int32 OuterLength { get; set; }

        public Double OuterLengthInInches => ConversionHelper.ConvertMillimetersToInches(OuterLength);

        /// <summary>
        /// Outer depth in mm
        /// </summary>
        public Int32 OuterDepth { get; set; }

        public Double OuterDepthIInches => ConversionHelper.ConvertMillimetersToInches(OuterDepth);

        /// <summary>
        /// Empty weight in g
        /// </summary>
        public Int32 EmptyWeight { get; set; }

        public Double EmptyWeightInPounds => ConversionHelper.ConvertGramsToPounds(EmptyWeight);

        /// <summary>
        /// Inner width in mm
        /// </summary>
        public Int32 InnerWidth { get; set; }

        public Double InnerWidthInInches => ConversionHelper.ConvertMillimetersToInches(InnerWidth);

        /// <summary>
        /// Inner length in mm
        /// </summary>
        public Int32 InnerLength { get; set; }

        public Double InnerLengthInInches => ConversionHelper.ConvertMillimetersToInches(InnerLength);

        /// <summary>
        /// Inner depth in mm
        /// </summary>
        public Int32 InnerDepth { get; set; }

        public Double InnerDepthInInches => ConversionHelper.ConvertMillimetersToInches(InnerDepth);

        /// <summary>
        /// Inner volume in mm^3
        /// </summary>
        public Int32 InnerVolume { get { return InnerWidth * InnerLength * InnerDepth; } }

        public Double InnerVolumeInInches => InnerWidthInInches * InnerLengthInInches * InnerDepthInInches;

        /// <summary>
        /// Max weight the package can hold in g
        /// </summary>
        public Int32 MaxWeight { get; set; }

        public Double MaxWeightInPounds => ConversionHelper.ConvertGramsToPounds(MaxWeight);

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
