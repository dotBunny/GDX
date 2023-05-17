// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Collections
{
    /// <summary>
    /// A struct purely created to allow for serialization of multi-dimensional arrays.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public  struct ArrayHolder<T>
    {
        public T[] TArray;

        public ref T this[int index]
        {
            get
            {
                return ref TArray[index];
            }
        }
    }
}