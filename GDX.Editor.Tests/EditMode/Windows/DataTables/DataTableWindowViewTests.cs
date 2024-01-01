// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System.Collections;
using GDX.DataTables;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GDX.Editor.Windows.DataTables
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="DataTableWindowView" />.
    /// </summary>
    public class DataTableWindowViewTests
    {
        DataTableBase m_TestDataTable;
        DataTableMetaData m_TestTableMeta;
        DataTableWindow m_DataTableWindow;
        DataTableWindowView m_DataTableWindowView;
        DataTableWindowController m_DataTableWindowController;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            m_TestDataTable = ScriptableObject.CreateInstance<StableDataTable>();
            m_TestTableMeta = ScriptableObject.CreateInstance<DataTableMetaData>();
            m_TestDataTable.m_MetaData = m_TestTableMeta;

            m_DataTableWindow = DataTableWindowProvider.OpenAsset(m_TestDataTable);
            m_DataTableWindowView = m_DataTableWindow.GetView();
            m_DataTableWindowController = m_DataTableWindow.GetController();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            m_DataTableWindow.Close();
            Object.DestroyImmediate(m_TestTableMeta);
            Object.DestroyImmediate(m_TestDataTable);
            yield return null;
        }
    }
}
#endif // UNITY_2022_2_OR_NEWER