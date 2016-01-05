using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C5;

namespace SixFourThree.BoxPacker.Model
{
    public class BoxList : MinHeap<Box>
    {
        public BoxList ShallowCopy()
        {
            BoxList othercopy = (BoxList)MemberwiseClone();

            othercopy.Content = new ArrayList();

            var items = Content.Cast<Box>();
            othercopy.Content.AddRange(items.ToArray());

            return othercopy;
        }
    }
}
