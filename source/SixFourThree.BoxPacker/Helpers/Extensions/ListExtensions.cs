using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixFourThree.BoxPacker.Helpers.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Adds the items to the list x number of times (if numberOfTimes greater than 1)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="itemsToAdd"></param>
        /// <param name="numberOfTimes"></param>
        public static void AddRangeIfNotNullMultipleTimes<T>(this IList<T> items, IList<T> itemsToAdd, Int32 numberOfTimes = 1)
        {
            if (numberOfTimes >= 1)
            {
                for (var counter = 0; counter < numberOfTimes; counter++)
                {
                    items.AddRangeIfNotNull(itemsToAdd);
                }
            }
        }

        /// <summary>
        /// Checks whether or not the list has items and isn't null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="itemsToAdd"></param>
        /// <returns></returns>
        public static void AddRangeIfNotNull<T>(this IList<T> items, IList<T> itemsToAdd)
        {
            if (items != null && itemsToAdd.HasItemsAndNotNull())
            {
                foreach (var itemToAdd in itemsToAdd)
                    items.Add(itemToAdd);
            }
        }

        /// <summary>
        /// Checks whether or not the list has items and isn't null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Boolean HasItemsAndNotNull<T>(this IList<T> items)
        {
            return items != null && items.Count > 0;
        }
    }
}
