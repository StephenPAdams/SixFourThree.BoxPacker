using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C5;

namespace SixFourThree.BoxPacker.Model
{
    public class ItemList : MaxHeap<Item>
    {
        public ItemList ShallowCopy()
        {
            ItemList othercopy = (ItemList)MemberwiseClone();
            return othercopy;
        }
    }
}
