using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixFourThree.BoxPacker.Helpers
{
    public static class ConversionHelper
    {        /// <summary>
             /// Converts millimeters to inches
             /// </summary>
             /// <param name="millimeters"></param>
             /// <returns></returns>
        public static Double ConvertMillimetersToInches(Double millimeters)
        {
            return millimeters * .03937008;
        }

        /// <summary>
        /// Converts millimeters to inches
        /// </summary>
        /// <param name="millimeters"></param>
        /// <returns></returns>
        public static Double ConvertMillimetersToInches(Int32 millimeters)
        {
            return ConvertMillimetersToInches((Double)millimeters);
        }

        /// <summary>
        /// Converts inches to millimeters
        /// </summary>
        /// <param name="inches"></param>
        /// <returns></returns>
        public static Double ConvertInchesToMillimeters(Double inches)
        {
            return inches * 25.4;
        }

        /// <summary>
        /// Converts inches to millimeters
        /// </summary>
        /// <param name="inches"></param>
        /// <returns></returns>
        public static Double ConvertInchesToMillimeters(Int32 inches)
        {
            return ConvertInchesToMillimeters((Double)inches);
        }

        /// <summary>
        /// Converts grams to pounds
        /// </summary>
        /// <param name="grams"></param>
        /// <returns></returns>
        public static Double ConvertGramsToPounds(Double grams)
        {
            return grams * .00220462;
        }

        /// <summary>
        /// Converts pounds to grams
        /// </summary>
        /// <param name="pounds"></param>
        /// <returns></returns>
        public static Double ConvertPoundsToGrams(Double pounds)
        {
            return pounds * 453.59237;
        }
    }
}
