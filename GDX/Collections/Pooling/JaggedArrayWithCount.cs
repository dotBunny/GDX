// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Collections.Pooling
{
    public struct JaggedArrayWithCount<T>
    {
        public T[][] Pool;
        public int Count;
    }
}