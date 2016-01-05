using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SixFourThree.BoxPacker.Helpers.Extensions;
using SixFourThree.BoxPacker.Interfaces;
using SixFourThree.BoxPacker.Model;

namespace SixFourThree.BoxPacker
{
    public class Packer : IPacker
    {
        private static Logger _logger;
        protected ItemList Items { get; set; }
        protected BoxList Boxes { get; set; }

        public Packer()
        {
            Items = new ItemList();
            Boxes = new BoxList();
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Adds an individual item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        public void AddItem(Item item, int quantity)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException("quantity");

            for (var quantityCounter = 0; quantityCounter < quantity; quantityCounter++)
            {
                Items.Insert(item);
            }

            _logger.Log(LogLevel.Info, String.Format("Added {0} x {1}", quantity, item.Description));
        }

        /// <summary>
        /// Add list of items
        /// </summary>
        /// <param name="items"></param>
        public void AddItems(ItemList items)
        {
            if (items == null)
                throw new ArgumentNullException();

            var tmpItems = items.GetContent().Cast<Item>();
            foreach (var item in tmpItems)
                AddItem(item, 1);
        }

        /// <summary>
        /// Add list of items
        /// </summary>
        /// <param name="items"></param>
        public void AddItems(IList<Item> items)
        {
            if (items == null)
                throw new ArgumentNullException();

            foreach (var item in items)
                AddItem(item, 1);
        }

        /// <summary>
        /// Add a pre-prepared box
        /// </summary>
        /// <param name="box"></param>
        public void AddBox(Box box)
        {
            if (box == null)
                throw new ArgumentNullException("box");

            Boxes.Insert(box);

            _logger.Log(LogLevel.Info, String.Format("Added box {0}", box.Description));
        }

        /// <summary>
        /// Add a pre-prepared set of boxes all at once
        /// </summary>
        /// <param name="boxes"></param>
        public void AddBoxes(BoxList boxes)
        {
            if (boxes == null)
                throw new ArgumentNullException();
            
            var tmpBoxes = boxes.GetContent().Cast<Box>();
            foreach (var box in tmpBoxes)
                AddBox(box);
        }

        /// <summary>
        /// Add a pre-prepared set of boxes all at once
        /// </summary>
        /// <param name="boxes"></param>
        public void AddBoxes(IList<Box> boxes)
        {
            if (boxes == null)
                throw new ArgumentNullException();

            foreach (var box in boxes)
                AddBox(box);
        }

        /// <summary>
        /// Pack items into boxes
        /// </summary>
        /// <returns></returns>
        public PackedBoxList Pack()
        { 
            var packedBoxes = PackByVolume();

            //If we have multiple boxes, try and optimise/even-out weight distribution 
            if (packedBoxes.GetCount() > 1)
                packedBoxes = RedistributeWeight(packedBoxes);

            _logger.Log(LogLevel.Info, String.Format("Packing completed, {0} boxes", packedBoxes.GetCount()));

            return packedBoxes;
        }

        /// <summary>
        /// Pack items into boxes using the principle of largest volume item first
        /// </summary>
        /// <returns></returns>
        public PackedBoxList PackByVolume()
        {     
            var packedBoxes = new PackedBoxList();
            ItemList nonProcessedItems = new ItemList();

            foreach (var item in Items.GetContent().Cast<Item>().ToList())
            {
                nonProcessedItems.Insert(item);
            }

            while (nonProcessedItems.GetCount() > 0)
            {
                var boxesToEvaluate = Boxes.Copy();
                var packedBoxesIteration = new PackedBoxList();

                while (!boxesToEvaluate.IsEmpty())
                {
                    var box = boxesToEvaluate.ExtractMin();
                    var packedBox = PackIntoBox(box, nonProcessedItems.Copy(), out nonProcessedItems);

                    if (packedBox.GetItems().GetCount() > 0)
                    {
                        packedBoxesIteration.Insert(packedBox);

                        // Have we found a single box that contains everything?
                        if (packedBox.GetItems().GetCount() == Items.GetCount())
                            break;
                    }
                }

                // Check iteration was productive 
                if (packedBoxesIteration.IsEmpty())
                {      
                    // OKAY, product was either TOO BIG or we RAN OUT OF BOXES
                    // If product is TOO BIG, we should have an option to allow for a custom box size to accommodate it
                    // If we RAN OUT OF BOXES, we should have an appropriate exception

                    // So we don't have enough boxes, let's just knock it off the heap
                    // We should probably log these somewhere to bubble them up to the library user
                    nonProcessedItems.ExtractMax();
                    /*
                    throw new Exception(String.Format("Item {0} is too large to fit into any box.",
                        Items.GetMax().Description));*/
                }
                else
                {
                    var tmpPackedBoxes = packedBoxesIteration.GetContent().Cast<PackedBox>().ToList();
                    packedBoxes.InsertAll(tmpPackedBoxes);
                }
            }

            return packedBoxes;
        }

        /// <summary>
        /// Given a solution set of packed boxes, repack them to achieve optimum weight distribution
        /// </summary>
        /// <param name="originalPackedBoxList"></param>
        /// <returns></returns>
        public PackedBoxList RedistributeWeight(PackedBoxList originalPackedBoxList)
        {
            var targetWeight = originalPackedBoxList.GetMeanWeight();

            _logger.Log(LogLevel.Debug, "Repacking for weight distribution, weight variance {0}, target weight {1}", originalPackedBoxList.GetWeightVariance(), targetWeight);

            var packedBoxes = new PackedBoxList();
            var overWeightBoxes = new List<PackedBox>();
            var underWeightBoxes = new List<PackedBox>();

            var originalPackedBoxes = originalPackedBoxList.Copy().GetContent().Cast<PackedBox>();
            foreach (var originalPackedBox in originalPackedBoxes)
            {
                var boxWeight = originalPackedBox.GetWeight();

                if (boxWeight > targetWeight)
                    overWeightBoxes.Add(originalPackedBox);
                else if (boxWeight < targetWeight)
                    underWeightBoxes.Add(originalPackedBox);
                else
                    packedBoxes.Insert(originalPackedBox); // Target weight, so we'll keep these
            }

      
            //Keep moving items from most overweight box to most underweight box
            var tryRepack = false;
            do { 
                _logger.Log(LogLevel.Debug, "Boxes under/over target: {0}/{1}", underWeightBoxes.Count, overWeightBoxes.Count);

                for (var underWeightBoxIndex = 0; underWeightBoxIndex < underWeightBoxes.Count; underWeightBoxIndex++)
                {
                    var underWeightBox = underWeightBoxes[underWeightBoxIndex];

                    for (var overWeightBoxIndex = 0; overWeightBoxIndex < overWeightBoxes.Count; overWeightBoxIndex++)
                    {
                        var overWeightBox = overWeightBoxes[overWeightBoxIndex];
                        var overWeightBoxItems = overWeightBox.GetItems().GetContent().Cast<Item>().ToList();

                        foreach (var overWeightBoxItem in overWeightBoxItems)
                        {
                            // If over target weight, just continue as it would hinder rather than help weight distribution
                            var overTargetWeight = (underWeightBox.GetWeight() + overWeightBoxItem.Weight) > targetWeight;
                            if (overTargetWeight)
                                continue;

                            var newItemsForLighterBox = underWeightBox.GetItems().ShallowCopy();
                            newItemsForLighterBox.Insert(overWeightBoxItem);

                            // We may need a bigger box
                            var newLighterBoxPacker = new Packer();
                            newLighterBoxPacker.AddBoxes(Boxes);
                            newLighterBoxPacker.AddItems(newItemsForLighterBox);
                            var newLighterBox = newLighterBoxPacker.PackByVolume().ExtractMin();

                            // New item fits!
                            if (newLighterBox.GetItems().GetCount() == newItemsForLighterBox.GetCount())
                            {
                                // Remove from overWeightBoxItems as it is now packed in a different box
                                overWeightBoxItems.Remove(overWeightBoxItem);

                                // We may be able to use a smaller box
                                var newHeavierBoxPacker = new Packer();
                                newHeavierBoxPacker.AddBoxes(Boxes);
                                newHeavierBoxPacker.AddItems(overWeightBoxItems);

                                var newHeavierBoxes = newHeavierBoxPacker.PackByVolume();

                                // Found an edge case in packing algorithm that *increased* box count
                                if (newHeavierBoxes.GetCount() > 1)
                                {
                                    _logger.Log(LogLevel.Info, "[REDISTRIBUTING WEIGHT] Abandoning redistribution, because new packing is less effciient than original");
                                    return originalPackedBoxList;
                                }

                                // TODO: INDEX BASED ARRAY INSERTION FOR BOTH
                                overWeightBoxes[overWeightBoxIndex] = newHeavierBoxes.ExtractMin();
                                underWeightBoxes[underWeightBoxIndex] = newLighterBox;

                                // We did some work, so see if we can do even better
                                tryRepack = true;
                                overWeightBoxes.Sort(originalPackedBoxList.ReverseCompareTo);
                                underWeightBoxes.Sort(originalPackedBoxList.ReverseCompareTo);

                                // The devil, but ported from PHP
                                goto MOVINGON;
                            }
                        }
                    }
                }

                MOVINGON: _logger.Log(LogLevel.Info, "Trying to repack");
            } while (tryRepack);

            packedBoxes.InsertAll(overWeightBoxes);
            packedBoxes.InsertAll(underWeightBoxes);

            return packedBoxes;
        }

        /// <summary>
        /// Packs as many boxes as possible into the given box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="items"></param>
        /// <param name="nonAddedItems"></param>
        /// <returns></returns>
        public PackedBox PackIntoBox(Box box, ItemList items, out ItemList nonAddedItems)
        {
            _logger.Log(LogLevel.Debug, "[EVALUATING BOX] {0}", box.Description);

            var packedItems = new ItemList();
            var remainingDepth = box.InnerDepth;
            var remainingWeight = box.MaxWeight - box.EmptyWeight;
            var remainingWidth = box.InnerWidth;
            var remainingLength = box.InnerLength;

            var layerWidth = 0;
            var layerLength = 0;
            var layerDepth = 0;

            nonAddedItems = new ItemList();

            while (!items.IsEmpty())
            {
                var itemToPack = items.GetMax();

                if (itemToPack.Depth > remainingDepth || itemToPack.Weight > remainingWeight)
                {
                    nonAddedItems.Insert(items.ExtractMax());
                    continue;
                }

                _logger.Log(LogLevel.Debug, "evaluating item {0}", itemToPack.Description);
                _logger.Log(LogLevel.Debug, "remaining width: {0}, length: {1}, depth: {2}", remainingWidth,
                    remainingLength, remainingDepth);
                _logger.Log(LogLevel.Debug, "layerWidth: {0}, layerLength: {1}, layerDepth: {2}", layerWidth,
                    layerLength, layerDepth);

                var itemWidth = itemToPack.Width;
                var itemLength = itemToPack.Length;

                var fitsSameGap = Math.Min(remainingWidth - itemWidth, remainingLength - itemLength);
                var fitsRotatedGap = Math.Min(remainingWidth - itemLength, remainingLength - itemWidth);

                if (fitsSameGap >= 0 || fitsRotatedGap >= 0)
                {
                    packedItems.Insert(items.ExtractMax());
                    remainingWeight -= itemToPack.Weight;

                    if (fitsRotatedGap < 0 ||
                        (fitsSameGap >= 0 && fitsSameGap <= fitsRotatedGap) ||
                        (itemWidth <= remainingWidth && !items.IsEmpty() && items.GetMax() == itemToPack &&
                         remainingLength >= 2*itemLength))
                    {
                        _logger.Log(LogLevel.Debug, "fits (better) unrotated");
                        remainingLength -= itemLength;
                        layerLength += itemLength;
                        layerWidth = Math.Max(itemWidth, layerWidth);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Debug, "fits (better) rotated");
                        remainingLength -= itemWidth;
                        layerLength += itemWidth;
                        layerWidth = Math.Max(itemLength, layerWidth);
                    }

                    //greater than 0, items will always be less deep
                    layerDepth = Math.Max(layerDepth, itemToPack.Depth);

                    //allow items to be stacked in place within the same footprint up to current layerdepth
                    var maxStackDepth = layerDepth - itemToPack.Depth;
                    while (!items.IsEmpty())
                    {
                        var potentialStackItem = items.GetMax();
                        if (potentialStackItem.Depth <= maxStackDepth &&
                            potentialStackItem.Weight <= remainingWeight &&
                            potentialStackItem.Width <= itemToPack.Width &&
                            potentialStackItem.Length <= itemToPack.Length)
                        {
                            remainingWeight -= potentialStackItem.Weight;
                            maxStackDepth -= potentialStackItem.Depth;
                            packedItems.Insert(items.ExtractMax());
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (remainingWidth >= Math.Min(itemWidth, itemLength) && layerDepth > 0 && layerWidth > 0 &&
                        layerLength > 0)
                    {
                        _logger.Log(LogLevel.Debug, "No more fit in lengthwise, resetting for new row");
                        remainingLength += layerLength;
                        remainingWidth -= layerWidth;
                        layerWidth = layerLength = 0;
                        continue;
                    }

                    if (remainingLength < Math.Min(itemWidth, itemLength) || layerDepth == 0)
                    {
                        _logger.Log(LogLevel.Debug, "doesn't fit on layer even when empty");
                        items.ExtractMax();
                        continue;
                    }

                    // TODO: TEST THIS
                    remainingWidth = (layerWidth == 0)
                        ? (Int32) Math.Min(Math.Floor(layerWidth*1.1), box.InnerWidth)
                        : box.InnerWidth;

                    // TODO: TEST THIS
                    remainingLength = (layerLength == 0)
                        ? (Int32)Math.Min(Math.Floor(layerLength*1.1), box.InnerLength)
                        : box.InnerLength;

                    remainingDepth -= layerDepth;

                    layerWidth = layerLength = layerDepth = 0;
                    _logger.Log(LogLevel.Debug, "doesn't fit, so starting next vertical layer");
                }
            }
            
            _logger.Log(LogLevel.Debug, "done with this box");
            return new PackedBox(box, packedItems, remainingWidth, remainingLength, remainingDepth, remainingWeight);
        }

        public BoxList GetBoxes()
        {
            return Boxes;
        }

        public ItemList GetItems()
        {
            return Items;
        }
    }
}
