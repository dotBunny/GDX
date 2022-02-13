// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

// ReSharper disable UnusedMember.Global

namespace GDX.Collections
{
    /// <summary>
    ///     An array where indices are allocated from and stored in an in-place linked list.
    ///     Allocating or deallocating a single int from this array is very fast, as is single datum lookup,
    ///     but neither the allocated indices nor the free indices can be reliably iterated without an external data structure.
    ///     This structure can be adapted to an arbitrary of external, parallel arrays.
    /// </summary>
    public struct FreeList
    {
        // ReSharper disable MemberCanBePrivate.Global

        /// <summary>
        ///     Data storage for allocated indices as well as the in-place free-list.
        /// </summary>
        public int[] Indices;

        /// <summary>
        ///     The next available index of the free-list.
        /// </summary>
        public int CurrentFreeIndex;

        /// <summary>
        ///     The total number of currently-allocated indices.
        /// </summary>
        public int Count;

        // ReSharper enable MemberCanBePrivate.Global

        /// <summary>
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the array.</param>
        public FreeList(int initialCapacity)
        {
            Indices = new int[initialCapacity];
            CurrentFreeIndex = 0;
            Count = 0;

            for (int i = 0; i < initialCapacity; i++)
            {
                Indices[i] = i + 1;
            }
        }

        /// <summary>
        ///     Removes all allocated data and rebuilds the free-list.
        /// </summary>
        public void Clear()
        {
            int length = Indices.Length;
            for (int i = 0; i < length; i++)
            {
                Indices[i] = i + 1;
            }

            Count = 0;
            CurrentFreeIndex = 0;
        }

        /// <summary>
        ///     Allocates an index from the free-list and stores an integer there, expanding the array if necessary.
        /// </summary>
        /// <param name="data">The integer value to store at the allocated index.</param>
        /// <param name="allocatedIndex">The index allocated from the free-list.</param>
        /// <param name="expandBy">How much the array should expand by when out of space.</param>
        /// <returns>True if the array expanded.</returns>
        public bool AddWithExpandCheck(int data, out int allocatedIndex, int expandBy)
        {
            int currentIndex = CurrentFreeIndex;
            int oldLength = Indices.Length;
            int expandedLength = oldLength + expandBy;

            bool needsExpansion = currentIndex >= oldLength;
            if (needsExpansion)
            {
                int[] newIndices = new int[expandedLength];
                Array.Copy(Indices, 0, newIndices, 0, oldLength);
                Indices = newIndices;

                for (int i = oldLength; i < expandedLength; i++)
                {
                    Indices[i] = i + 1;
                }
            }

            CurrentFreeIndex = Indices[currentIndex];
            Indices[currentIndex] = data;
            ++Count;

            allocatedIndex = currentIndex;

            return needsExpansion;
        }

        /// <summary>
        ///     Allocates an index from the free-list and stores an integer there, expanding the array by twice the current size if
        ///     necessary.
        /// </summary>
        /// <param name="data">The integer value to store at the allocated index.</param>
        /// <param name="allocatedIndex">The index allocated from the free-list.</param>
        /// <returns>True if the array expanded.</returns>
        public bool AddWithExpandCheck(int data, out int allocatedIndex)
        {
            int currentIndex = CurrentFreeIndex;
            int oldLength = Indices.Length;
            int expandedLength = oldLength * 2;

            bool needsExpansion = currentIndex >= oldLength;
            if (needsExpansion)
            {
                int[] newIndices = new int[expandedLength];
                Array.Copy(Indices, 0, newIndices, 0, oldLength);
                Indices = newIndices;

                for (int i = oldLength; i < expandedLength; i++)
                {
                    Indices[i] = i + 1;
                }
            }

            CurrentFreeIndex = Indices[currentIndex];
            Indices[currentIndex] = data;
            ++Count;

            allocatedIndex = currentIndex;

            return needsExpansion;
        }

        /// <summary>
        ///     Allocates an index from the free-list and stores an integer there, without checking for expansion.
        /// </summary>
        /// <param name="data">The integer value to store at the allocated index.</param>
        /// <returns>The index allocated from the free-list.</returns>
        public int AddUnchecked(int data)
        {
            int currentIndex = CurrentFreeIndex;

            CurrentFreeIndex = Indices[currentIndex];
            Indices[currentIndex] = data;
            ++Count;

            return currentIndex;
        }

        /// <summary>
        ///     Deallocates the given index and adds it to the free-list.
        /// </summary>
        /// <param name="index">The index to add to the free-list.</param>
        public void RemoveAt(int index)
        {
            Indices[index] = CurrentFreeIndex;
            CurrentFreeIndex = index;
            --Count;
        }

        /// <summary>
        ///     Retrieves the value stored at the given index and deallocates the index, adding it to the free-list.
        /// </summary>
        /// <param name="index">The index to add to the free-list.</param>
        /// <returns>The value stored at the given index.</returns>
        public int GetAndRemoveAt(int index)
        {
            int data = Indices[index];
            Indices[index] = CurrentFreeIndex;
            CurrentFreeIndex = index;
            --Count;

            return data;
        }
    }
}