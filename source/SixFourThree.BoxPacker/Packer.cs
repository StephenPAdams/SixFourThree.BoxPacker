using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SixFourThree.BoxPacker.Exceptions;
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

        /// <summary>
        /// If true, items that are oversized will not throw an exception, but will rather have a custom box created just
        /// for them. By default this is false.
        /// </summary>
        protected Boolean CreateBoxesForOversizedItems { get; set; }

        public Packer()
        {
            Items = new ItemList();
            Boxes = new BoxList();
            CreateBoxesForOversizedItems = false;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public Packer(Boolean createBoxesForOverizedItems)
        {
            Items = new ItemList();
            Boxes = new BoxList();
            CreateBoxesForOversizedItems = createBoxesForOverizedItems;
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

            while (Items.GetCount() > 0)
            {
                var boxesToEvaluate = Boxes.ShallowCopy();
                var packedBoxesIteration = new PackedBoxList();

                while (!boxesToEvaluate.IsEmpty())
                {
                    var box = boxesToEvaluate.ExtractMin();
                    var packedBox = PackIntoBox(box, Items.ShallowCopy());

                    if (packedBox.GetItems().GetCount() > 0)
                    {
                        packedBoxesIteration.Insert(packedBox);

                        // Have we found a single box that contains everything?
                        if (packedBox.GetItems().GetCount() == Items.GetCount())
                            break;
                    }
                }

                // Check iteration was productive
                if (packedBoxesIteration.IsEmpty() && !CreateBoxesForOversizedItems)
                    throw new ItemTooLargeException(String.Format("Item {0} is too large to fit into any box.", Items.GetMax().Description));

                // 1. Create box, add to boxes list
                // 2. Run through loop to add that product to that box
                if (packedBoxesIteration.IsEmpty() && CreateBoxesForOversizedItems)
                {
                    // TODO: What should the empty box weight be?
                    var oversizedItem = Items.GetMax();
                    var box = new Box()
                              {
                                  Description = String.Format("Custom box for {0}", oversizedItem.Description),
                                  EmptyWeight = 0,
                                  InnerDepth = oversizedItem.Depth,
                                  InnerLength = oversizedItem.Length,
                                  InnerWidth = oversizedItem.Width,
                                  MaxWeight = oversizedItem.Weight,
                                  OuterDepth = oversizedItem.Depth,
                                  OuterLength = oversizedItem.Length,
                                  OuterWidth = oversizedItem.Width
                              };
                    Boxes.Insert(box);
                    _logger.Log(LogLevel.Debug, "Item {0} is too large to fit into any box, creating custom box for it.",
                        oversizedItem.Description);
                }
                else
                {
                    // Find best box of iteration, and remove packed items from unpacked list
                    var bestBox = packedBoxesIteration.GetMin();
                    var bestBoxItems = bestBox.GetItems().ShallowCopy();
                    var unpackedItems = Items.GetContent().Cast<Item>().ToList();

                    foreach (var packedItem in bestBoxItems.GetContent())
                    {
                        foreach (var unpackedItem in unpackedItems)
                        {
                            if (packedItem == unpackedItem)
                            {
                                unpackedItems.Remove(unpackedItem);
                                break;
                            }
                        }
                    }

                    var unpackedItemList = new ItemList();
                    foreach (var unpackedItem in unpackedItems)
                    {
                        unpackedItemList.Insert(unpackedItem);
                    }

                    Items = unpackedItemList;
                    packedBoxes.Insert(bestBox);
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

            var originalPackedBoxes = originalPackedBoxList.ShallowCopy().GetContent().Cast<PackedBox>();
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

      
            // Keep moving items from most overweight box to most underweight box
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
        /// <returns></returns>
        public PackedBox PackIntoBox(Box box, ItemList items)
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

            while (!items.IsEmpty())
            {
                var itemToPack = items.GetMax();

                if (itemToPack.Depth > remainingDepth || itemToPack.Weight > remainingWeight)
                {
                    items.ExtractMax();
                    continue;
                }

                _logger.Log(LogLevel.Debug, "Evaluating item {0}", itemToPack.Description);
                _logger.Log(LogLevel.Debug, "Remaining width: {0}, length: {1}, depth: {2}", remainingWidth, remainingLength, remainingDepth);
                _logger.Log(LogLevel.Debug, "LayerWidth: {0}, layerLength: {1}, layerDepth: {2}", layerWidth, layerLength, layerDepth);

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
                         remainingLength >= 2 * itemLength))
                    {
                        _logger.Log(LogLevel.Debug, "Fits (better) unrotated.");
                        remainingLength -= itemLength;
                        layerLength += itemLength;
                        layerWidth = Math.Max(itemWidth, layerWidth);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Debug, "Fits (better) rotated.");
                        remainingLength -= itemWidth;
                        layerLength += itemWidth;
                        layerWidth = Math.Max(itemLength, layerWidth);
                    }

                    // Greater than 0, items will always be less deep
                    layerDepth = Math.Max(layerDepth, itemToPack.Depth);

                    // Allow items to be stacked in place within the same footprint up to current layerdepth
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
                        _logger.Log(LogLevel.Debug, "No more fit in lengthwise, resetting for new row.");
                        remainingLength += layerLength;
                        remainingWidth -= layerWidth;
                        layerWidth = layerLength = 0;
                        continue;
                    }

                    if (remainingLength < Math.Min(itemWidth, itemLength) || layerDepth == 0)
                    {
                        _logger.Log(LogLevel.Debug, "Doesn't fit on layer even when empty.");
                        items.ExtractMax();
                        continue;
                    }

                    // TODO: TEST THIS
                    remainingWidth = (layerWidth > 0)
                        ? (Int32)Math.Min(Math.Floor(layerWidth * 1.1), box.InnerWidth)
                        : box.InnerWidth;

                    // TODO: TEST THIS
                    remainingLength = (layerLength > 0)
                        ? (Int32)Math.Min(Math.Floor(layerLength * 1.1), box.InnerLength)
                        : box.InnerLength;

                    remainingDepth -= layerDepth;

                    layerWidth = layerLength = layerDepth = 0;
                    _logger.Log(LogLevel.Debug, "Doesn't fit, so starting next vertical layer.");
                }
            }

            _logger.Log(LogLevel.Debug, "Done with this box.");
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
