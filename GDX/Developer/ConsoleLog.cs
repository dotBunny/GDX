// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using GDX.Logging;

namespace GDX.Developer
{
    public class ConsoleHistory
    {
        CircularBuffer<LogEntry> m_LogHistory = new CircularBuffer<LogEntry>(1000);

        public static LogEntry GetEntryAt(int index)
        {
            return s_Buffer[index];
        }

        public static int GetEntryCount()
        {
            return s_Buffer.Count;
        }
    }
}