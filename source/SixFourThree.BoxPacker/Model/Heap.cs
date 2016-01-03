using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixFourThree.BoxPacker.Model
{
    public class Heap<T> : AhnentafelList where T : IComparable<T>
    {
        public enum HeapType
        {
            MinHeap,
            MaxHeap
        };

        private readonly HeapType _heapType;

        public Heap(HeapType type)
        {
            _heapType = type;
        }

        public HeapType Type
        {
            get { return _heapType; }
        }

        public T GetBest()
        {
            if (IsEmpty())
                throw new InvalidOperationException("Heap is empty.");

            return (T)Content[0];
        }

        public T ExtractBest()
        {
            T result = GetBest();
            DeleteBest();
            return result;
        }

        public void DeleteBest()
        {
            if (IsEmpty())
                throw new InvalidOperationException("Heap is empty.");

            SwitchItems(0, GetCount() - 1);
            Content.RemoveAt(GetCount() - 1);

            if (!IsEmpty())
                BubbleDown(0);
        }

        public virtual int Insert(T item)
        {
            int index = Content.Add(item);
            index = BubbleUp(index);
            return index;
        }

        protected bool IsFirstBigger(int first, int second)
        {
            return (((IComparable<T>)Content[first]).CompareTo(((T)Content[second])) > 0);
        }

        private int BubbleUp(int index)
        {
            if (index == 0)
                return 0;
            int parent = GetParentIndex(index);
            // while parent is smaller and item not on root already
            while ((_heapType == HeapType.MinHeap && index != 0 && IsFirstBigger(parent, index))
                || (_heapType == HeapType.MaxHeap && index != 0 && IsFirstBigger(index, parent)))
            {
                SwitchItems(index, parent);
                index = parent;
                parent = GetParentIndex(parent);
            }
            return index;
        }

        private int BubbleDown(int index)
        {
            int leftChild, rightChild, targetChild;
            bool finished = false;
            do
            {
                leftChild = GetLeftChildIndex(index);
                rightChild = GetRightChildIndex(index);
                // if left child is bigger then right child
                if (leftChild == index && rightChild == index) // when no children, get child will return element itself
                {
                    finished = true; // bubbled down to the end
                }
                else // bubble further
                {
                    if ((_heapType == HeapType.MinHeap && IsFirstBigger(leftChild, rightChild)) ||
                        (_heapType == HeapType.MaxHeap && IsFirstBigger(rightChild, leftChild)))
                        targetChild = rightChild;
                    else
                        targetChild = leftChild;
                    // if smaller item at index is bigger than smaller child
                    if ((_heapType == HeapType.MinHeap && IsFirstBigger(index, targetChild))
                        || (_heapType == HeapType.MaxHeap && IsFirstBigger(targetChild, index)))
                    {
                        SwitchItems(targetChild, index);
                        index = targetChild;
                    }
                    else
                        finished = true;
                }
            }
            while (!finished);
            return index;
        }

        private void SwitchItems(int index1, int index2)
        {
            T temp = (T)Content[index1];
            Content[index1] = Content[index2];
            Content[index2] = temp;
        }
    }

    public class MinHeap<T> : Heap<T>  where T : IComparable<T>
    {
        public MinHeap() : base(HeapType.MinHeap) {}

        public T GetMin() { return GetBest(); }
        public T ExtractMin() { return ExtractBest(); }
        public void DeleteMin() { DeleteBest(); }
    }

    public class MaxHeap<T> : Heap<T> where T : IComparable<T>
    {
        public MaxHeap()
            : base(HeapType.MaxHeap)
        {
        }

        public T GetMax() { return base.GetBest(); }
        public T ExtractMax() { return base.ExtractBest(); }
        public void DeleteMax() { base.DeleteBest(); }
    }

    public class MidHeap<T> : MinHeap<T> where T : IComparable<T>
    {
        private readonly MaxHeap<T> _maxHeap;

        public MidHeap()
        {
            _maxHeap = new MaxHeap<T>();
        }

        public new bool IsEmpty()
        {
            return (base.IsEmpty() && _maxHeap.IsEmpty());
        }

        public new int GetCount()
        {
            return (base.GetCount() + _maxHeap.GetCount());
        }

        public new void Clear()
        {
            base.Clear();
            _maxHeap.Clear();
        }

        public override int Insert(T item)
        {
            int value;
            if (base.IsEmpty() || GetMin().CompareTo(item) < 0)
                value = base.Insert(item);
            else
                value = _maxHeap.Insert(item);
            if (_maxHeap.GetCount() > base.GetCount() + 1)
                base.Insert(_maxHeap.ExtractMax());
            else if (base.GetCount() > _maxHeap.GetCount() + 1)
                _maxHeap.Insert(ExtractMin());

            return value;
        }

        public T GetMid() { return CorrectHeap().GetBest(); }
        public T ExtractMid() { return CorrectHeap().ExtractBest(); }
        public void DeleteMid() { CorrectHeap().DeleteBest(); }

        private Heap<T> CorrectHeap()
        {
            if (base.GetCount() > _maxHeap.GetCount())
                return this;
            return _maxHeap;
        }
    }

}
