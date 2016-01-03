using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixFourThree.BoxPacker.Model
{
    public class AhnentafelList
    {
        protected ArrayList Content;

        protected AhnentafelList()
        {
            Content = new ArrayList();
        }

        public bool IsEmpty()
        {
            if (GetCount() == 0)
                return true;
            return false;
        }

        public int GetCount()
        {
            return Content.Count;
        }

        public void Clear()
        {
            Content.Clear();
        }

        protected int GetParentIndex(int index)
        {
            if (index < 0 || index > GetCount() - 1)
                throw new System.InvalidOperationException("Invalid index.");
            int result = (int)Math.Floor(((double)index - 1) / 2);
            return result;
        }

        protected int GetLeftChildIndex(int index)
        {
            if (index < 0 || index > GetCount() - 1)
                throw new System.InvalidOperationException("Invalid index.");
            int result = (2 * index) + 1;
            if (result > GetCount() - 1)
                result = index; // return itself if no children
            return result;
        }

        protected int GetRightChildIndex(int index)
        {
            if (index < 0 || index > GetCount() - 1)
                throw new System.InvalidOperationException("Invalid index.");
            int result = (2 * index) + 2;
            if (result > GetCount() - 1)
                result = index; // return itself if no children
            return result;
        }

        public ArrayList GetContent()
        {
            return Content;
        }
    }

}
