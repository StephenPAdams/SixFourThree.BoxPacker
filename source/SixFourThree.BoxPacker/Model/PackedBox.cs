using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixFourThree.BoxPacker.Model
{
    public class PackedBox : IComparable<PackedBox>
    {
        public PackedBox(Box box, ItemList items, Int32 remainingWidth, Int32 remainingLength, Int32 remainingDepth, Int32 remainingWeight)
        {
            Box = box;
            GeneratedId = Guid.NewGuid().ToString();
            Items = items;
            RemainingWidth = remainingWidth;
            RemainingLength = remainingLength;
            RemainingDepth = remainingDepth;
            RemainingWeight = remainingWeight;
        }

        protected Box Box { get; set; }
        
        /// <summary>
        /// Generated id for the packed box
        /// </summary>
        public String GeneratedId { get; }

        protected ItemList Items { get; set; }

        /// <summary>
        /// Total weight in g
        /// </summary>
        protected Int32? Weight { get; set; }

        /// <summary>
        /// Remaining width in mm inside box for another item
        /// </summary>
        protected Int32 RemainingWidth { get; set; }

        /// <summary>
        /// Remaining length in mm inside box for another item
        /// </summary>
        protected Int32 RemainingLength { get; set; }

        /// <summary>
        /// Remaining depth in mm inside box for another item
        /// </summary>
        protected Int32 RemainingDepth { get; set; }
        
        /// <summary>
        /// Remaining weight in g inside box for another item
        /// </summary>
        protected Int32 RemainingWeight { get; set; }

        /// <summary>
        /// Calculates the current weight of the items in the box in grams
        /// </summary>
        /// <returns></returns>
        public Int32 GetWeight()
        {
            if (Weight.HasValue)
                return Weight.Value;

            Weight = Box.EmptyWeight;

            var items = Items.GetContent().Cast<Item>();

            foreach (var item in items)
            {
                Weight += item.Weight;
            }

            return Weight.Value;
        }

        public Double GetVolumeUtilisation()
        {
            var items = Items.GetContent().Cast<Item>();
            var itemVolume = items.Sum(item => item.Volume);

            return Math.Round((Double)itemVolume / Box.InnerVolume * 100, 1);
        }

        public int CompareTo(PackedBox other)
        {
            var choice = Items.GetCount() - other.Items.GetCount();

            if (choice == 0)
                choice = other.Box.InnerVolume - Box.InnerVolume;

            return choice;
        }

        public ItemList GetItems()
        {
            return Items;
        }

        public Box GetBox()
        {
            return Box;
        }
    }
}
