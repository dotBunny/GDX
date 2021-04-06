// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A <see cref="System.Collections.Generic.List{T}" />-like data structure.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="object" />s contained within.</typeparam>
    [VisualScriptingCollection]
    public struct SimpleList<T>
    {
        /// <summary>
        ///     Internal array of backed data for the <see cref="SimpleList{T}" />.
        /// </summary>
        public T[] Array;

        /// <summary>
        ///     The current number of occupied elements in the <see cref="CircularBuffer{T}" />.
        /// </summary>
        /// <remarks>CAUTION! Changing this will alter the understanding of the data.</remarks>
        public int Count;

        /// <summary>
        ///     Create a <see cref="SimpleList{T}" /> with an initial <paramref name="capacity" />.
        /// </summary>
        /// <param name="capacity">An initial sizing for the <see cref="Array" />.</param>
        public SimpleList(int capacity)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            Array = new T[capacity];
            Count = 0;
        }

        /// <summary>
        ///     Create a <see cref="SimpleList{T}" /> providing an existing <paramref name="arrayToUse" />.
        /// </summary>
        /// <param name="arrayToUse">An existing array to use in the <see cref="SimpleList{T}" />.</param>
        public SimpleList(T[] arrayToUse)
        {
            Array = arrayToUse;
            Count = 0;
        }

        /// <summary>
        ///     Create a <see cref="SimpleList{T}" /> providing an existing <paramref name="arrayToUse" /> and setting the
        ///     <see cref="Count" />.
        /// </summary>
        /// <param name="arrayToUse">An existing array to use in the <see cref="SimpleList{T}" />.</param>
        /// <param name="count">An existing element count.</param>
        public SimpleList(T[] arrayToUse, int count)
        {
            Array = arrayToUse;
            Count = count;
        }

        /// <summary>
        ///     Add an item to the <see cref="SimpleList{T}" /> without checking the <see cref="Array" /> size.
        /// </summary>
        /// <param name="item">A typed <see cref="object" /> to add.</param>
        public void AddUnchecked(T item)
        {
            Array[Count] = item;
            ++Count;
        }

        /// <summary>
        ///     Add an item to the <see cref="SimpleList{T}" />, checking if <see cref="Array" /> needs to be resized.
        /// </summary>
        /// <param name="item">A typed <see cref="object" /> to add.</param>
        public void AddWithExpandCheck(T item)
        {
            int arrayLength = Array.Length;

            if (Count == arrayLength)
            {
                arrayLength = arrayLength == 0 ? 1 : arrayLength;
                System.Array.Resize(ref Array, arrayLength * 2);
            }

            Array[Count] = item;
            ++Count;
        }

        /// <summary>
        ///     Add an item to the <see cref="SimpleList{T}" />, checking if <see cref="Array" /> needs to be resized.
        /// </summary>
        /// <param name="item">A typed <see cref="object" /> to add.</param>
        /// <param name="howMuchToExpand">How much to expand the array by.</param>
        public void AddWithExpandCheck(T item, int howMuchToExpand)
        {
            if (Count == Array.Length)
            {
                System.Array.Resize(ref Array, Array.Length + howMuchToExpand);
            }

            Array[Count] = item;
            ++Count;
        }

        /// <summary>
        ///     Clear out the <see cref="Array" /> in <see cref="SimpleList{T}" /> and sets the <see cref="Count" /> to 0.
        /// </summary>
        public void Clear()
        {
            System.Array.Clear(Array, 0, Count);
            Count = 0;
        }

        /// <summary>
        ///     Insert an item into the <see cref="SimpleList{T}" />, checking if <see cref="Array" /> needs to be resized.
        /// </summary>
        /// <param name="item">A typed <see cref="object" /> to insert.</param>
        /// <param name="index">The index in <see cref="Array" /> to add the <paramref name="item" /> at.</param>
        public void Insert(T item, int index)
        {
            int arrayLength = Array.Length;
            if (Count == arrayLength)
            {
                arrayLength = arrayLength == 0 ? 1 : arrayLength;
                System.Array.Resize(ref Array, arrayLength * 2);
            }

            System.Array.Copy(Array, index, Array, index + 1, Count - index);
            Array[index] = item;
            ++Count;
        }

        /// <summary>
        ///     Remove an item from the <see cref="SimpleList{T}" /> at a specific <paramref name="index" />.
        /// </summary>
        /// <param name="index">The target index.</param>
        public void RemoveAt(int index)
        {
            int newLength = Count - 1;

            Array[index] = default;

            if (index < Array.Length)
            {
                System.Array.Copy(Array, index + 1, Array, index, newLength - index);
            }

            Count = newLength;
        }

        /// <summary>
        ///     Remove the last element in the <see cref="SimpleList{T}" />.
        /// </summary>
        public void RemoveFromBack()
        {
            int newLength = Count - 1;
            Array[newLength] = default;
            Count = newLength;
        }

        /// <summary>
        ///     Resizes the <see cref="Array" />, ensuring there are the provided number of empty spots in it.
        /// </summary>
        /// <param name="numberToReserve">Number of desired empty spots.</param>
        public void Reserve(int numberToReserve)
        {
            int combinedLength = Count + numberToReserve;

            if (combinedLength > Array.Length)
            {
                System.Array.Resize(ref Array, combinedLength);
            }
        }

        /// <summary>
        ///     Reverse the order of <see cref="Array"/>.
        /// </summary>
        public void Reverse()
        {
            T temporaryStorage;

            int lastIndex = Count - 1;
            int middleIndex = Count / 2;

            for (int currentElementIndex = 0; currentElementIndex < middleIndex; currentElementIndex++)
            {
                // Store the swap value
                temporaryStorage = Array[currentElementIndex];
                int swapElementIndex = lastIndex - currentElementIndex;

                // Swap values
                Array[currentElementIndex] = Array[swapElementIndex];
                Array[swapElementIndex] = temporaryStorage;
            }
        }
    }
}