// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System;
using System.Collections;
using GDX.Editor.Windows.Tables;
using GDX.DataTables;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GDX.Editor.Windows
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="TableWindowView" />.
    /// </summary>
    public class TableWindowViewTests
    {
        DataTableObject m_TestDataTable;
        TableWindow m_TableWindow;
        TableWindowView m_TableWindowView;
        TableWindowController m_TableWindowController;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            m_TestDataTable = ScriptableObject.CreateInstance<StableDataTable>();
            m_TableWindow = TableWindowProvider.OpenAsset(m_TestDataTable);
            m_TableWindowView = m_TableWindow.GetView();
            m_TableWindowController = m_TableWindow.GetController();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            m_TableWindow.Close();
            Object.DestroyImmediate(m_TestDataTable);
            yield return null;
        }
    }
}
#endif