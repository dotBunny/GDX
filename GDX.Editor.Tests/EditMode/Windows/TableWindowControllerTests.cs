// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER
using System.Collections;
using GDX.Editor.Windows.DataTables;
using GDX.DataTables;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GDX.Editor.Windows
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="DataTableWindowController" />.
    /// </summary>
    public class TableWindowControllerTests
    {
        DataTableBase m_TestDataTable;
        DataTableWindow m_DataTableWindow;
        DataTableWindowController m_DataTableWindowController;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            m_TestDataTable = ScriptableObject.CreateInstance<StableDataTable>();
            m_DataTableWindow = DataTableWindowProvider.OpenAsset(m_TestDataTable);
            m_DataTableWindowController = m_DataTableWindow.GetController();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            m_DataTableWindow.Close();
            Object.DestroyImmediate(m_TestDataTable);
            yield return null;
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddColumn_OneString_Valid()
        {
            m_DataTableWindowController.AddColumn("Test_String", Serializable.SerializableTypes.String);

            Assert.That(m_TestDataTable.GetColumnCount() == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddColumn_TenMixed_Valid()
        {
            m_DataTableWindowController.AddColumn("Test_String", Serializable.SerializableTypes.String);
            m_DataTableWindowController.AddColumn("Test_Bounds", Serializable.SerializableTypes.Bounds);
            m_DataTableWindowController.AddColumn("Test_Vector2", Serializable.SerializableTypes.Vector2);
            m_DataTableWindowController.AddColumn("Test_Bool", Serializable.SerializableTypes.Bool);
            m_DataTableWindowController.AddColumn("Test_Object", Serializable.SerializableTypes.Object);
            m_DataTableWindowController.AddColumn("Test_Quaternion", Serializable.SerializableTypes.Quaternion);
            m_DataTableWindowController.AddColumn("Test_SByte", Serializable.SerializableTypes.SByte);
            m_DataTableWindowController.AddColumn("Test_Double", Serializable.SerializableTypes.Double);
            m_DataTableWindowController.AddColumn("Test_Float", Serializable.SerializableTypes.Float);
            m_DataTableWindowController.AddColumn("Test_String", Serializable.SerializableTypes.String);

            Assert.That(m_TestDataTable.GetColumnCount() == 10);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddRow_One_Valid()
        {
            m_DataTableWindowController.AddColumn("Test_String", Serializable.SerializableTypes.String);
            m_DataTableWindowController.AddColumn("Test_Bounds", Serializable.SerializableTypes.Bounds);
            m_DataTableWindowController.AddColumn("Test_Vector2", Serializable.SerializableTypes.Vector2);

            m_DataTableWindowController.AddRow("Test_Row");
            RowDescription rowDescription = m_TestDataTable.GetRowDescription(0);

            Assert.That(m_TestDataTable.GetColumnCount() == 3);
            Assert.That(m_TestDataTable.GetRowCount() == 1);
            Assert.That(m_TestDataTable.GetRowName(rowDescription.Identifier) == "Test_Row");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddRemove_RemoveColumn_Count()
        {
            m_DataTableWindowController.AddColumn("A", Serializable.SerializableTypes.String);
            m_DataTableWindowController.AddColumn("B", Serializable.SerializableTypes.String);
            m_DataTableWindowController.AddColumn("C", Serializable.SerializableTypes.Bounds);
            m_DataTableWindowController.AddColumn("D", Serializable.SerializableTypes.String);
            m_DataTableWindowController.AddRowDefault();
            m_DataTableWindowController.AddRowDefault();
            m_DataTableWindowController.AddRowDefault();
            m_DataTableWindowController.AddColumn("E", Serializable.SerializableTypes.Bounds);
            m_DataTableWindowController.RemoveColumn(m_TestDataTable.GetColumnDescription(3).Identifier); // remove D
            m_DataTableWindowController.RemoveColumn(m_TestDataTable.GetColumnDescription(3).Identifier); // remove E
            m_DataTableWindowController.AddColumn("E", Serializable.SerializableTypes.Bounds);
            m_DataTableWindowController.AddColumn("D", Serializable.SerializableTypes.String);
            m_DataTableWindowController.RemoveColumn(m_TestDataTable.GetColumnDescription(3).Identifier); // remove E
            m_DataTableWindowController.AddColumn("E", Serializable.SerializableTypes.Bounds);
            m_DataTableWindowController.AddColumn("F", Serializable.SerializableTypes.String);
            m_DataTableWindowController.AddColumn("G", Serializable.SerializableTypes.String);
            m_DataTableWindowController.AddRowDefault();
            m_DataTableWindowController.AddRowDefault();
            m_DataTableWindowController.RemoveColumn(m_TestDataTable.GetColumnDescription(4).Identifier); // remove E

            Assert.That(m_TestDataTable.GetColumnCount() == 6, $"Expected 6, found {m_TestDataTable.GetColumnCount()}");
        }

    }
}
#endif // UNITY_2022_2_OR_NEWER