using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixFourThree.BoxPacker.Exceptions
{
    public class ItemTooLargeException : Exception
    {
        public ItemTooLargeException() { }
        public ItemTooLargeException(String message) : base(message) { }
        public ItemTooLargeException(String message, Exception inner) : base(message, inner) { }
    }
}
