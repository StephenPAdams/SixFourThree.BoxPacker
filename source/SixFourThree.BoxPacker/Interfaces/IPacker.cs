using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixFourThree.BoxPacker.Model;

namespace SixFourThree.BoxPacker.Interfaces
{
    public interface IPacker
    {
        /// <summary>
        /// Adds an individual item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        void AddItem(Item item, Int32 quantity);

        /// <summary>
        /// Add list of items
        /// </summary>
        /// <param name="items"></param>
        void AddItems(ItemList items);

        /// <summary>
        /// Add list of items
        /// </summary>
        /// <param name="items"></param>
        void AddItems(IList<Item> items);

        /// <summary>
        /// Add a pre-prepared box
        /// </summary>
        /// <param name="box"></param>
        void AddBox(Box box);

        /// <summary>
        /// Add a pre-prepared set of boxes all at once
        /// </summary>
        /// <param name="boxes"></param>
        void AddBoxes(BoxList boxes);

        /// <summary>
        /// Add a pre-prepared set of boxes all at once
        /// </summary>
        /// <param name="boxes"></param>
        void AddBoxes(IList<Box> boxes);

        /// <summary>
        /// Pack items into boxes
        /// </summary>
        /// <returns></returns>
        PackedBoxList Pack();

        /// <summary>
        /// Pack items into boxes using the principle of largest volume item first
        /// </summary>
        /// <returns></returns>
        PackedBoxList PackByVolume();

        /// <summary>
        /// Given a solution set of packed boxes, repack them to achieve optimum weight distribution
        /// </summary>
        /// <param name="originalPackedBoxList"></param>
        /// <returns></returns>
        PackedBoxList RedistributeWeight(PackedBoxList originalPackedBoxList);

        /// <summary>
        /// Packs as many boxes as possible into the given box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        PackedBox PackIntoBox(Box box, ItemList items);
    }
}
