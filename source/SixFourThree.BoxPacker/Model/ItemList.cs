using System;
using System.Collections;
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
            
            othercopy.Content = new ArrayList();

            var items = Content.Cast<Item>();
            othercopy.Content.AddRange(items.ToArray());
            
            return othercopy;
        }
    }
}
