// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

// ReSharper disable MemberCanBePrivate.Global

// TODO: Change to struct + unit tests

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A sized buffer which loops back over itself as elements are used.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="object" />s contained within.</typeparam>
    [VisualScriptingCompatible(1)]
    public struct CircularBuffer<T>
    {
        /// <summary>
        ///     Internal array of backed data for the <see cref="CircularBuffer{T}" />.
        /// </summary>
        public readonly T[] Array;

        /// <summary>
        ///     The cached array length for <see cref="Array" />.
        /// </summary>
        public readonly int Capacity;

        /// <summary>
        ///     The current size of occupied elements in the <see cref="CircularBuffer{T}" />.
        /// </summary>
        /// <remarks>CAUTION! Changing this will alter the understanding of the data.</remarks>
        public int Count;

        /// <summary>
        ///     The index of the last item in <see cref="Array" />.
        /// </summary>
        /// <remarks>CAUTION! Changing this will alter the understanding of the data.</remarks>
        public int EndIndex;

        /// <summary>
        ///     The index of the first item in <see cref="Array" />.
        /// </summary>
        /// <remarks>CAUTION! Changing this will alter the understanding of the data.</remarks>
        public int StartIndex;

        /// <summary>
        ///     Create a <see cref="CircularBuffer{T}" /> with a <paramref name="capacity" />.
        /// </summary>
        /// <param name="capacity">The maximum number of items allowed in the <see cref="CircularBuffer{T}" /></param>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        public CircularBuffer(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("Circular buffer cannot have negative or zero capacity.", nameof(capacity));
            }

            Array = new T[capacity];
            Capacity = capacity;

            Count = 0;

            StartIndex = 0;
            EndIndex = Count == capacity ? 0 : Count;
        }

        /// <summary>
        ///     Create a <see cref="CircularBuffer{T}" /> with a <paramref name="capacity" />, filling with
        ///     <paramref name="targetItems"/>.
        /// </summary>
        /// <param name="capacity">The maximum number of items allowed in the <see cref="CircularBuffer{T}" /></param>
        /// <param name="targetItems">An array of values to fill the <see cref="CircularBuffer{T}" /> with.</param>
        /// <exception cref="ArgumentException">
        ///     Invalid number of entries provided to the <see cref="CircularBuffer{T}" />
        ///     constructor.
        /// </exception>
        /// <exception cref="ArgumentNullException">No items were provided to the <see cref="CircularBuffer{T}" /> constructor.</exception>
        public CircularBuffer(int capacity, T[] targetItems)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("Circular buffer cannot have negative or zero capacity.", nameof(capacity));
            }

            if (targetItems == null)
            {
                throw new ArgumentNullException(nameof(targetItems));
            }

            if (targetItems.Length > capacity)
            {
                throw new ArgumentException("Too many items to fit circular buffer", nameof(targetItems));
            }

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            Array = new T[capacity];
            Capacity = capacity;

            System.Array.Copy(targetItems, Array, targetItems.Length);
            Count = targetItems.Length;

            StartIndex = 0;
            EndIndex = Count == capacity ? 0 : Count;
        }

        /// <summary>
        ///     Access item at <paramref name="pseudoIndex" />.
        /// </summary>
        /// <param name="pseudoIndex"></param>
        /// <exception cref="IndexOutOfRangeException">Provided index is out of buffers range.</exception>
        public T this[int pseudoIndex]
        {
            get
            {
                return Array[
                    StartIndex +
                    (pseudoIndex < Capacity - StartIndex ? pseudoIndex : pseudoIndex - Capacity)];
            }
            set
            {
                Array[
                    StartIndex +
                    (pseudoIndex < Capacity - StartIndex ? pseudoIndex : pseudoIndex - Capacity)] = value;
            }
        }

        /// <summary>
        ///     Add an <paramref name="item" /> to the <see cref="Array" />.
        /// </summary>
        /// <param name="item">The typed <see cref="object" /> to add.</param>
        public void Add(T item)
        {
            PushBack(item);
        }

        /// <summary>
        ///     Clear all values of the <see cref="Array" />.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Array.Length; i++)
            {
                Array[i] = default;
            }

            StartIndex = 0;
            EndIndex = 0;
        }

        /// <summary>
        ///     Get the last item in the <see cref="Array" />.
        /// </summary>
        /// <returns>The last typed object in <see cref="Array" />.</returns>
        public T GetBack()
        {
            return Array[(EndIndex != 0 ? EndIndex : Capacity) - 1];
        }

        /// <summary>
        ///     Get the first item in the <see cref="Array" />.
        /// </summary>
        /// <returns>The first typed object in <see cref="Array" />.</returns>
        public T GetFront()
        {
            return Array[StartIndex];
        }


        /// <summary>
        ///     Does the <see cref="Array" /> have any items in it?
        /// </summary>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty()
        {
            return Count == 0;
        }

        /// <summary>
        ///     Is the <see cref="Array" /> at capacity?
        /// </summary>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFull()
        {
            return Count == Capacity;
        }

        /// <summary>
        ///     Remove an item from the end of the <see cref="Array" />.
        /// </summary>
        public void PopBack()
        {
            if (EndIndex == 0)
            {
                EndIndex = Capacity;
            }

            EndIndex--;
            Array[EndIndex] = default;
            --Count;
        }

        /// <summary>
        ///     Remove an item from the start of the <see cref="Array" />.
        /// </summary>
        public void PopFront()
        {
            Array[StartIndex] = default;
            if (++StartIndex == Capacity)
            {
                StartIndex = 0;
            }

            --Count;
        }

        /// <summary>
        ///     Add an item to the end of the <see cref="Array" />.
        /// </summary>
        /// <param name="targetItem">The item to add to the end of <see cref="Array" />.</param>
        public void PushBack(T targetItem)
        {
            if (Count == Capacity)
            {
                Array[EndIndex] = targetItem;
                if (++EndIndex == Capacity)
                {
                    EndIndex = 0;
                }

                StartIndex = EndIndex;
            }
            else
            {
                Array[EndIndex] = targetItem;
                if (++EndIndex == Capacity)
                {
                    EndIndex = 0;
                }

                ++Count;
            }
        }

        /// <summary>
        ///     Add an item to the start of the <see cref="Array" />.
        /// </summary>
        /// <param name="targetItem">The item to add to the start of <see cref="Array" />.</param>
        public void PushFront(T targetItem)
        {
            if (Count == Capacity)
            {
                if (StartIndex == 0)
                {
                    StartIndex = Capacity;
                }

                StartIndex--;
                EndIndex = StartIndex;
                Array[StartIndex] = targetItem;
            }
            else
            {
                if (StartIndex == 0)
                {
                    StartIndex = Capacity;
                }

                StartIndex--;
                Array[StartIndex] = targetItem;
                ++Count;
            }
        }

        /// <summary>
        ///     Copy <see cref="Array" /> to an array of the same type.
        /// </summary>
        /// <returns>A copied version of the <see cref="Array" /> as an array.</returns>
        public T[] ToArray()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            T[] newArray = new T[Count];
            int newArrayOffset = 0;
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            ArraySegment<T>[] segments = { ArrayOne(), ArrayTwo() };
            foreach (ArraySegment<T> segment in segments)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                System.Array.Copy(segment.Array, segment.Offset, newArray, newArrayOffset, segment.Count);
                newArrayOffset += segment.Count;
            }

            return newArray;
        }

        /// <summary>
        ///     Get the first array segment of the desired ordered array in the <see cref="Array" />.
        /// </summary>
        /// <remarks>
        ///     This is done because the arrays actual order may wrap around from the end of <see cref="Array" />, to the start.
        /// </remarks>
        /// <returns>An <see cref="ArraySegment{T}" /> representing the first part of the ordered data in <see cref="Array" />.</returns>
        private ArraySegment<T> ArrayOne()
        {
            if (Count == 0)
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                return new ArraySegment<T>(new T[0]);
            }

            return StartIndex < EndIndex
                ? new ArraySegment<T>(Array, StartIndex, EndIndex - StartIndex)
                : new ArraySegment<T>(Array, StartIndex, Array.Length - StartIndex);
        }

        /// <summary>
        ///     Get the second array segment of the desired ordered array in the <see cref="Array" />.
        /// </summary>
        /// <remarks>
        ///     This is done because the arrays actual order may wrap around from the end of <see cref="Array" />, to the start.
        /// </remarks>
        /// <returns>An <see cref="ArraySegment{T}" /> representing the end part of the ordered data in <see cref="Array" />.</returns>
        private ArraySegment<T> ArrayTwo()
        {
            if (Count == 0)
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                return new ArraySegment<T>(new T[0]);
            }

            return StartIndex < EndIndex
                ? new ArraySegment<T>(Array, EndIndex, 0)
                : new ArraySegment<T>(Array, 0, EndIndex);
        }
    }
}