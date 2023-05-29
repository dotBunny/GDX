// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System;
using System.Collections;
using GDX.Editor.Windows.Tables;
using GDX.Tables;
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
        TableBase m_TestTable;
        TableWindow m_TableWindow;
        TableWindowView m_TableWindowView;
        TableWindowController m_TableWindowController;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            m_TestTable = ScriptableObject.CreateInstance<StableTable>();
            m_TableWindow = TableWindowProvider.OpenAsset(m_TestTable);
            m_TableWindowView = m_TableWindow.GetView();
            m_TableWindowController = m_TableWindow.GetController();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            m_TableWindow.Close();
            Object.DestroyImmediate(m_TestTable);
            yield return null;
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ColumnHeader_Name_InternalIndexMatches()
        {
            m_TableWindowController.AddColumn("A", Serializable.SerializableTypes.String);
            m_TableWindowController.AddColumn("B", Serializable.SerializableTypes.String);
            m_TableWindowController.AddColumn("C", Serializable.SerializableTypes.Bounds);
            m_TableWindowController.AddColumn("D", Serializable.SerializableTypes.String);


            TableBase.ColumnDescription columnDescription = m_TestTable.GetColumnDescription(3);
            VisualElement columnCHeader = m_TableWindowView.GetColumnContainer()[0][3];
            TableBase.ColumnDescription columnCDescription = m_TestTable.GetColumnDescription(columnCHeader.name.Split('_', StringSplitOptions.RemoveEmptyEntries)[1]);

            Assert.That(columnDescription.InternalIndex == columnCDescription.InternalIndex);
        }
    }
}
#endif