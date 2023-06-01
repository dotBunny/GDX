// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System.Collections;
using GDX.Editor.Windows.DataTables;
using GDX.DataTables;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GDX.Editor.Windows
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="TableWindowView" />.
    /// </summary>
    public class TableWindowViewTests
    {
        DataTableObject m_TestDataTable;
        DataTableWindow m_DataTableWindow;
        TableWindowView m_TableWindowView;
        TableWindowController m_TableWindowController;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            m_TestDataTable = ScriptableObject.CreateInstance<StableDataTable>();
            m_DataTableWindow = TableWindowProvider.OpenAsset(m_TestDataTable);
            m_TableWindowView = m_DataTableWindow.GetView();
            m_TableWindowController = m_DataTableWindow.GetController();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            m_DataTableWindow.Close();
            Object.DestroyImmediate(m_TestDataTable);
            yield return null;
        }
    }
}
#endif // UNITY_2022_2_OR_NEWER