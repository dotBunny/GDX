// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading.Tasks;

namespace GDX.Developer
{
    public class WaitWhile
    {
        readonly Func<bool> m_Check;
        public WaitWhile(Func<bool> conditional)
        {
            m_Check = conditional;
        }

        public bool Wait()
        {
            return m_Check();
        }

        public async Task WaitAsync()
        {
            await Task.Run(() =>
            {
                if (m_Check())
                {
                    Task.Delay(1).Wait();
                }
            });
        }

        public IEnumerator While()
        {
            while (m_Check())
            {
                yield return null;
            }
        }
    }
}