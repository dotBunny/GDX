// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Unity.Collections;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A <see cref="Unity.Collections.NativeArray{T}" /> backed simplified first-in, first-out (FIFO) data structure.
    /// </summary>
    /// <remarks>
    ///     The <see cref="NativeSimpleQueue{T}" /> is backed by a <see cref="Unity.Collections.NativeArray{T}" /> which requires
    ///     UnityEngine.CoreModule.dll.
    /// </remarks>
    /// <typeparam name="T">The type of <see cref="object" />s contained within.</typeparam>
    public struct NativeSimpleQueue<T> : IDisposable where T : struct
    {
        /// <summary>
        ///     The minimum growth amount of the internal <see cref="Array" />'s length.
        /// </summary>
        private const int MinimumGrow = 4;

        // ReSharper disable MemberCanBePrivate.Global

        /// <summary>
        ///     Internal <see cref="NativeArray{T}" /> of backed data for the <see cref="NativeSimpleQueue{T}" />.
        /// </summary>
        public NativeArray<T> Array;

        /// <summary>
        ///     Number of elements.
        /// </summary>
        public int Count;

        /// <summary>
        ///     Last valid element (tail) index in the queue.
        /// </summary>
        public int EndIndex;

        /// <summary>
        ///     First valid element (head) index in the queue.
        /// </summary>
        public int FirstIndex;

        // ReSharper enable MemberCanBePrivate.Global

        /// <summary>
        ///     Create a new <see cref="NativeSimpleQueue{T}" />.
        /// </summary>
        /// <param name="capacity">The maximum number of items allowed in the <see cref="NativeSimpleQueue{T}" /></param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public NativeSimpleQueue(int capacity, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            Array = new NativeArray<T>(capacity, allocator, nativeArrayOptions);
            FirstIndex = 0;
            EndIndex = 0;
            Count = 0;
        }

        /// <summary>
        ///     Clear the <see cref="NativeSimpleQueue{T}" />.
        /// </summary>
        public void Clear()
        {
            if (FirstIndex < EndIndex)
            {
                for (int i = FirstIndex; i < Count; i++)
                {
                    Array[i] = default;
                }
            }
            else
            {
                for (int i = FirstIndex; i < Array.Length - FirstIndex; i++)
                {
                    Array[i] = default;
                }

                for (int i = 0; i < EndIndex; i++)
                {
                    Array[i] = default;
                }
            }

            FirstIndex = 0;
            EndIndex = 0;
            Count = 0;
        }

        /// <summary>
        ///     Pop (remove and return) the first element from the <see cref="NativeSimpleQueue{T}" />.
        /// </summary>
        /// <returns>The first element.</returns>
        public T Dequeue()
        {
            T removed = Array[FirstIndex];
            Array[FirstIndex] = default;
            FirstIndex = (FirstIndex + 1) % Array.Length;
            Count--;
            return removed;
        }

        /// <summary>
        ///     Properly dispose of the <see cref="NativeSimpleQueue{T}" />.
        /// </summary>
        public void Dispose()
        {
            Array.Dispose();
            Array = default;
            FirstIndex = default;
            EndIndex = default;
            Count = default;
        }

        /// <summary>
        ///     Add an <paramref name="item" /> to the <see cref="NativeSimpleQueue{T}" /> at its end..
        /// </summary>
        /// <param name="item">The typed <see cref="object" /> to add.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public void Enqueue(T item, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            if (Count == Array.Length)
            {
                int newCapacity = Array.Length * 2;
                if (newCapacity < Array.Length + MinimumGrow)
                {
                    newCapacity = Array.Length + MinimumGrow;
                }

                SetCapacity(newCapacity, allocator, nativeArrayOptions);
            }

            Array[EndIndex] = item;
            EndIndex = (EndIndex + 1) % Array.Length;
            Count++;
        }

        /// <summary>
        ///     Retrieve the element at the provided <paramref name="index" />.
        /// </summary>
        /// <param name="index">The target <see cref="object" /> index.</param>
        /// <returns>The target element.</returns>
        public T GetElementAt(int index)
        {
            return Array[(FirstIndex + index) % Array.Length];
        }

        /// <summary>
        ///     Retrieve the first element from the <see cref="NativeSimpleQueue{T}" />.
        /// </summary>
        /// <returns>The first element.</returns>
        public T Peek()
        {
            return Array[FirstIndex];
        }

        /// <summary>
        ///     Shrink the capacity of the <see cref="NativeSimpleQueue{T}" /> to fit its contents.
        /// </summary>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public void TrimExcess(Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            int threshold = (int)(Array.Length * 0.9);
            if (Count < threshold)
            {
                SetCapacity(Count, allocator, nativeArrayOptions);
            }
        }

        /// <summary>
        ///     Resize the capacity of the <see cref="NativeSimpleQueue{T}" />.
        /// </summary>
        /// <param name="capacity">The desired capacity for the <see cref="NativeSimpleQueue{T}" /></param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        private void SetCapacity(int capacity, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            NativeArray<T> newArray = new NativeArray<T>(capacity, allocator, nativeArrayOptions);
            if (Count > 0)
            {
                if (FirstIndex < EndIndex)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        newArray[i] = Array[FirstIndex + i];
                    }
                }
                else
                {
                    for (int i = 0; i < Array.Length - FirstIndex; i++)
                    {
                        newArray[i] = Array[FirstIndex + i];
                    }

                    for (int i = 0; i < EndIndex; i++)
                    {
                        newArray[Array.Length - FirstIndex + i] = Array[i];
                    }
                }
            }

            Array.Dispose();
            Array = newArray;
            FirstIndex = 0;
            EndIndex = Count == capacity ? 0 : Count;
        }
    }
}