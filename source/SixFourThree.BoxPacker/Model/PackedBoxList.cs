using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixFourThree.BoxPacker.Model
{
    public class PackedBoxList : MinHeap<PackedBox>
    {
        /// <summary>
        /// Average (mean) weight of boxes
        /// </summary>
        protected Double? MeanWeight { get; set; }

        /// <summary>
        /// Variance in weight between boxes
        /// </summary>
        protected Double? WeightVariance { get; set; }

        public int ReverseCompareTo(PackedBox packedBoxA, PackedBox packedBoxB)
        {
            var choice = packedBoxB.GetItems().GetCount() - packedBoxA.GetItems().GetCount();

            if (choice == 0)
                choice = packedBoxA.GetBox().InnerVolume - packedBoxB.GetBox().InnerVolume;

            return choice;
        }

        /// <summary>
        /// Calculate teh average (mean) weight of the boxes
        /// </summary>
        /// <returns></returns>
        public Double GetMeanWeight()
        {
            if (MeanWeight.HasValue)
                return MeanWeight.Value;
            
            var boxes = GetContent().Cast<PackedBox>();
            foreach (var box in boxes)
            {
                MeanWeight += box.GetWeight();
            }

            if (MeanWeight.HasValue && GetCount() > 0)
            {
                MeanWeight = MeanWeight.Value/GetCount();

                return MeanWeight.Value;
            }

            return 0;
        }

        public Double GetWeightVariance()
        {
            if (WeightVariance.HasValue)
                return WeightVariance.Value;

            var mean = GetMeanWeight();
            
            var boxes = GetContent().Cast<PackedBox>();
            foreach (var box in boxes)
            {
                WeightVariance += Math.Pow(box.GetWeight() - mean, 2);
            }

            if (WeightVariance.HasValue && GetCount() > 0)
            {
                WeightVariance = WeightVariance.Value/GetCount();

                return WeightVariance.Value;
            }

            return 0;
        }

        public void InsertAll(IList<PackedBox> packedBoxes)
        {
            foreach (var packedBox in packedBoxes)
            {
                Insert(packedBox);
            }
        }

        public PackedBoxList ShallowCopy()
        {
            PackedBoxList othercopy = (PackedBoxList)MemberwiseClone();
            return othercopy;
        }
    }
}
