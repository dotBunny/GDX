// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using GDX.Editor.Windows.Tables;
using GDX.Tables;
using UnityEngine;
using UnityEngine.TestTools;

namespace GDX.Editor.Windows
{
#if UNITY_2022_2_OR_NEWER
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="TableWindowView" />.
    /// </summary>
    public class TableWindowViewTests
    {
        TableBase m_TestTable;
        TableWindow m_TableWindow;
        TableWindowView m_TableWindowView;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            m_TestTable = ScriptableObject.CreateInstance<StableTable>();
            m_TableWindow = TableWindowProvider.OpenAsset(m_TestTable);
            m_TableWindowView = m_TableWindow.GetView();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            m_TableWindow.Close();
            Object.DestroyImmediate(m_TestTable);
            yield return null;
        }
    }
#endif
}