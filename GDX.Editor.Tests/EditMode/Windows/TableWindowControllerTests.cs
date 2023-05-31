// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER
using System.Collections;
using GDX.Editor.Windows.Tables;
using GDX.DataTables;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GDX.Editor.Windows
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="TableWindowController" />.
    /// </summary>
    public class TableWindowControllerTests
    {
        DataTableObject m_TestDataTable;
        TableWindow m_TableWindow;
        TableWindowController m_TableWindowController;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            m_TestDataTable = ScriptableObject.CreateInstance<StableDataTable>();
            m_TableWindow = TableWindowProvider.OpenAsset(m_TestDataTable);
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

        [Test]
        [Category(Core.TestCategory)]
        public void AddColumn_OneString_Valid()
        {
            m_TableWindowController.AddColumn("Test_String", Serializable.SerializableTypes.String);

            Assert.That(m_TestDataTable.GetColumnCount() == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddColumn_TenMixed_Valid()
        {
            m_TableWindowController.AddColumn("Test_String", Serializable.SerializableTypes.String);
            m_TableWindowController.AddColumn("Test_Bounds", Serializable.SerializableTypes.Bounds);
            m_TableWindowController.AddColumn("Test_Vector2", Serializable.SerializableTypes.Vector2);
            m_TableWindowController.AddColumn("Test_Bool", Serializable.SerializableTypes.Bool);
            m_TableWindowController.AddColumn("Test_Object", Serializable.SerializableTypes.Object);
            m_TableWindowController.AddColumn("Test_Quaternion", Serializable.SerializableTypes.Quaternion);
            m_TableWindowController.AddColumn("Test_SByte", Serializable.SerializableTypes.SByte);
            m_TableWindowController.AddColumn("Test_Double", Serializable.SerializableTypes.Double);
            m_TableWindowController.AddColumn("Test_Float", Serializable.SerializableTypes.Float);
            m_TableWindowController.AddColumn("Test_String", Serializable.SerializableTypes.String);

            Assert.That(m_TestDataTable.GetColumnCount() == 10);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddRow_One_Valid()
        {
            m_TableWindowController.AddColumn("Test_String", Serializable.SerializableTypes.String);
            m_TableWindowController.AddColumn("Test_Bounds", Serializable.SerializableTypes.Bounds);
            m_TableWindowController.AddColumn("Test_Vector2", Serializable.SerializableTypes.Vector2);

            m_TableWindowController.AddRow("Test_Row");
            DataTableObject.RowDescription rowDescription = m_TestDataTable.GetRowDescription(0);

            Assert.That(m_TestDataTable.GetColumnCount() == 3);
            Assert.That(m_TestDataTable.GetRowCount() == 1);
            Assert.That(m_TestDataTable.GetRowName(rowDescription.Identifier) == "Test_Row");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddRemove_RemoveColumn_Count()
        {
            m_TableWindowController.AddColumn("A", Serializable.SerializableTypes.String);
            m_TableWindowController.AddColumn("B", Serializable.SerializableTypes.String);
            m_TableWindowController.AddColumn("C", Serializable.SerializableTypes.Bounds);
            m_TableWindowController.AddColumn("D", Serializable.SerializableTypes.String);
            m_TableWindowController.AddRowDefault();
            m_TableWindowController.AddRowDefault();
            m_TableWindowController.AddRowDefault();
            m_TableWindowController.AddColumn("E", Serializable.SerializableTypes.Bounds);
            m_TableWindowController.RemoveColumn(m_TestDataTable.GetColumnDescription(3).Identifier); // remove D
            m_TableWindowController.RemoveColumn(m_TestDataTable.GetColumnDescription(3).Identifier); // remove E
            m_TableWindowController.AddColumn("E", Serializable.SerializableTypes.Bounds);
            m_TableWindowController.AddColumn("D", Serializable.SerializableTypes.String);
            m_TableWindowController.RemoveColumn(m_TestDataTable.GetColumnDescription(3).Identifier); // remove E
            m_TableWindowController.AddColumn("E", Serializable.SerializableTypes.Bounds);
            m_TableWindowController.AddColumn("F", Serializable.SerializableTypes.String);
            m_TableWindowController.AddColumn("G", Serializable.SerializableTypes.String);
            m_TableWindowController.AddRowDefault();
            m_TableWindowController.AddRowDefault();
            m_TableWindowController.RemoveColumn(m_TestDataTable.GetColumnDescription(4).Identifier); // remove E

            Assert.That(m_TestDataTable.GetColumnCount() == 6, $"Expected 6, found {m_TestDataTable.GetColumnCount()}");
        }

    }
}
#endif