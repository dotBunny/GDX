// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using GDX.DataTables;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GDX.Editor.Inspectors
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="StableDataTableInspector" />.
    /// </summary>
    public class StableDataTableInspectorTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void BuildExample_NoUnexpectedLogs()
        {
            StableDataTable table = null;
            DataTableMetaData meta = null;
            try
            {
                table = ScriptableObject.CreateInstance<StableDataTable>();
                meta = ScriptableObject.CreateInstance<DataTableMetaData>();

                table.metaData = meta;

                StableDataTableInspector.BuildExample(table);
            }
            finally
            {
                Object.DestroyImmediate(meta);
                Object.DestroyImmediate(table);
            }

            LogAssert.NoUnexpectedReceived();
        }
    }
}
#endif // UNITY_2022_2_OR_NEWER