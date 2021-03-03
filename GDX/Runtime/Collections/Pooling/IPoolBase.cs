// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace GDX.Collections.Pooling
{
    public interface IPoolBase<T>
    {
        /// <summary>
        /// Is the provided <paramref name="item"/> managed by this this <see cref="IPoolBase{T}"/>.
        /// </summary>
        /// <param name="item">The item in question </param>
        /// <returns></returns>
        public bool IsPooled(T item);
        public bool IsManaged(T item);
        public void ForceRemove(T item);
    }
}