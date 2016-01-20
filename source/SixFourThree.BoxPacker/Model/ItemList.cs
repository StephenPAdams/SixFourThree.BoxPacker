using System.Collections;
using System.Linq;

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
