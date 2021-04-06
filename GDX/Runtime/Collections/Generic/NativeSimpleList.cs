// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Unity.Collections;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A <see cref="Unity.Collections.NativeArray{T}" /> backed <see cref="SimpleList{T}" />.
    /// </summary>
    /// <remarks>
    ///     The <see cref="NativeSimpleList{T}" /> is backed by a <see cref="Unity.Collections.NativeArray{T}" /> which
    ///     requires
    ///     UnityEngine.CoreModule.dll.
    /// </remarks>
    /// <typeparam name="T">The type of <see cref="object" />s contained within.</typeparam>
    public struct NativeSimpleList<T> : IDisposable where T : struct
    {
        /// <summary>
        ///     Internal array of backed data for the <see cref="SimpleList{T}" />.
        /// </summary>
        public NativeArray<T> Array;

        /// <summary>
        ///     Number of elements.
        /// </summary>
        /// <remarks>CAUTION! Changing this will alter the understanding of the data.</remarks>
        public int Count;

        /// <summary>
        ///     Create a <see cref="SimpleList{T}" /> with an initial <paramref name="capacity" />.
        /// </summary>
        /// <param name="capacity">An initial sizing for the <see cref="Array" />.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public NativeSimpleList(int capacity, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            Array = new NativeArray<T>(capacity, allocator, nativeArrayOptions);
            Count = 0;
        }

        /// <summary>
        ///     Create a <see cref="NativeSimpleList{T}" /> providing an existing <paramref name="arrayToUse" />.
        /// </summary>
        /// <param name="arrayToUse">An existing array to use in the <see cref="NativeSimpleList{T}" />.</param>
        public NativeSimpleList(NativeArray<T> arrayToUse)
        {
            Array = arrayToUse;
            Count = 0;
        }

        /// <summary>
        ///     Create a <see cref="NativeSimpleList{T}" /> providing an existing <paramref name="arrayToUse" /> and setting the
        ///     <see cref="Count" />.
        /// </summary>
        /// <param name="arrayToUse">An existing array to use in the <see cref="NativeSimpleList{T}" />.</param>
        /// <param name="count">An existing element count.</param>
        public NativeSimpleList(NativeArray<T> arrayToUse, int count)
        {
            Array = arrayToUse;
            Count = count;
        }

        /// <summary>
        ///     Add an item to the <see cref="NativeSimpleList{T}" /> without checking the <see cref="Array" /> size.
        /// </summary>
        /// <param name="item">A typed <see cref="object" /> to add.</param>
        public void AddUnchecked(T item)
        {
            Array[Count] = item;
            ++Count;
        }

        /// <summary>
        ///     Add an item to the <see cref="NativeSimpleList{T}" />, checking if <see cref="Array" /> needs to be resized.
        /// </summary>
        /// <param name="item">A typed <see cref="object" /> to add.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public void AddWithExpandCheck(T item, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            int arrayLength = Array.Length;

            if (Count == arrayLength)
            {
                arrayLength = arrayLength == 0 ? 1 : arrayLength;
                NativeArray<T> newArray = new NativeArray<T>(arrayLength * 2, allocator, nativeArrayOptions);
                Array.CopyTo(newArray);

                Array.Dispose();

                Array = newArray;
            }

            Array[Count] = item;
            ++Count;
        }

        /// <summary>
        ///     Clear out the <see cref="Array" /> in <see cref="NativeSimpleList{T}" /> and sets the <see cref="Count" /> to 0.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                Array[i] = default;
            }

            Count = 0;
        }

        /// <summary>
        ///     Properly dispose of the <see cref="NativeSimpleList{T}" />.
        /// </summary>
        public void Dispose()
        {
            if (!Array.IsCreated)
            {
                return;
            }

            Array.Dispose();
            Array = default;
        }

        /// <summary>
        ///     Insert an item into the <see cref="NativeSimpleList{T}" />, checking if <see cref="Array" /> needs to be resized.
        /// </summary>
        /// <param name="item">A typed <see cref="object" /> to insert.</param>
        /// <param name="index">The index in <see cref="Array" /> to add the <paramref name="item" /> at.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public void Insert(T item, int index, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            if (Count == Array.Length)
            {
                NativeArray<T> newArray = new NativeArray<T>(Count * 2, allocator, nativeArrayOptions);
                Array.CopyTo(newArray);

                Array.Dispose();

                Array = newArray;
            }

            for (int i = index; i < Count - 1; i++)
            {
                Array[i + 1] = Array[i];
            }

            Array[index] = item;
            ++Count;
        }

        /// <summary>
        ///     Remove an item from the <see cref="NativeSimpleList{T}" /> at a specific <paramref name="index" />.
        /// </summary>
        /// <param name="index">The target index.</param>
        public void RemoveAt(int index)
        {
            int newLength = Count - 1;

            Array[index] = default;

            for (int i = index; i < newLength; i++)
            {
                Array[i] = Array[i + 1];
            }

            Count = newLength;
        }

        /// <summary>
        ///     Remove the last element in the <see cref="NativeSimpleList{T}" />.
        /// </summary>
        public void RemoveFromBack()
        {
            int newLength = Count - 1;
            Array[newLength] = default;
            Count = newLength;
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