using System.Collections.Generic;

namespace Mosh
{
    /// <summary>
    /// A queue suitable for a parser, where you can get a lookahead sign
    /// </summary>
    public class PQueue<T> : Queue<T>
    {
        public T PeekNext()
        {
            return PeekN(1);
        }

        public T PeekN(int n)
        {
            var tempArray = new T[Count];
            CopyTo(tempArray, 0);

            return tempArray[n];
        }

        public void EnqueueBeginning(T item)
        {
            var tempArray = new T[Count + 1];
            tempArray[0] = item;

            CopyTo(tempArray, 1);
            Clear();

            foreach (var tempItem in tempArray)
            {
                Enqueue(tempItem);
            }
        }
    }
}