// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;
using UnityEngine;

namespace GDX.Tables
{
    public class StableTableTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void StableTable_CreateEmpty_RowCountsZero()
        {
            StableTable table = ScriptableObject.CreateInstance<StableTable>();

            bool evaluate = table.GetColumnCount() == 0 && table.GetRowCount() == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void StableTable_CreateEmpty_NullDescriptionArrays()
        {
            StableTable table = ScriptableObject.CreateInstance<StableTable>();

            bool evaluate = table.GetAllColumnDescriptions() == null && table.GetAllRowDescriptions() == null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void StableTable_CellAccess_ValidColumnNames()
        {
            StableTable table = ScriptableObject.CreateInstance<StableTable>();

            int[] allColumnIDs = new int[Serializable.SerializableTypesCount];

            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                allColumnIDs[i] = table.AddColumn((Serializable.SerializableTypes)i, ((Serializable.SerializableTypes)i).ToString());
            }

            bool areAllNamesEqualToSetNames = true;
            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                int columnID = allColumnIDs[i];
                string iToString = ((Serializable.SerializableTypes)i).ToString();
                areAllNamesEqualToSetNames &= (table.GetColumnNameRef(columnID) == iToString) && (table.GetColumnName(columnID) == iToString);
            }

            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                int columnID = allColumnIDs[i];
                string iToStringModified = ((Serializable.SerializableTypes)i).ToString() + "mod";
                table.SetColumnName(iToStringModified, columnID);
            }

            bool areSetNamesSet = true;
            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                int columnID = allColumnIDs[i];
                string iToStringModified = ((Serializable.SerializableTypes)i).ToString() + "mod";
                areSetNamesSet &= table.GetColumnName(columnID) == iToStringModified && table.GetColumnNameRef(columnID) == iToStringModified;
            }

            Assert.IsTrue(areAllNamesEqualToSetNames && areSetNamesSet);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void StableTable_CellAccess_ValidRowNames()
        {
            StableTable table = ScriptableObject.CreateInstance<StableTable>();

            int[] allRowIDs = new int[10];

            for (int i = 0; i < 10; i++)
            {
                allRowIDs[i] = table.AddRow(i.ToString());
            }

            bool areAllNamesEqualToSetNames = true;
            for (int i = 0; i < 10; i++)
            {
                int rowID = allRowIDs[i];
                string iToString = i.ToString();
                areAllNamesEqualToSetNames &= (table.GetRowNameRef(rowID) == iToString) && (table.GetRowName(rowID) == iToString);
            }

            for (int i = 0; i < 10; i++)
            {
                int rowID = allRowIDs[i];
                string iToStringModified = i.ToString() + "mod";
                table.SetRowName(iToStringModified, rowID);
            }

            bool areSetNamesSet = true;
            for (int i = 0; i < 10; i++)
            {
                int rowID = allRowIDs[i];
                string iToStringModified = i.ToString() + "mod";
                areSetNamesSet &= table.GetRowName(rowID) == iToStringModified && table.GetRowNameRef(rowID) == iToStringModified;
            }

            Assert.IsTrue(areAllNamesEqualToSetNames && areSetNamesSet);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void StableTable_CellAccess_ValidCellValues()
        {
            StableTable table = ScriptableObject.CreateInstance<StableTable>();

            int[] allColumnIDs = new int[Serializable.SerializableTypesCount];

            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                allColumnIDs[i] = table.AddColumn((Serializable.SerializableTypes)i, ((Serializable.SerializableTypes)i).ToString());
            }

            int[] allRowIDs = new int[10];

            for (int i = 0; i < 10; i++)
            {
                allRowIDs[i] = table.AddRow(i.ToString());
            }

            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                Serializable.SerializableTypes currentType = (Serializable.SerializableTypes)i;
                for (int j = 0; j < 10; j++)
                {
                    switch (currentType)
                    {
                        case Serializable.SerializableTypes.Invalid:
                            break;
                        case Serializable.SerializableTypes.String:
                            table.SetString(allRowIDs[j], allColumnIDs[i], i.ToString() + j.ToString());
                            break;
                        case Serializable.SerializableTypes.Char:
                            table.SetChar(allRowIDs[j], allColumnIDs[i], (char)(i + j));
                            break;
                        case Serializable.SerializableTypes.Bool:
                            table.SetBool(allRowIDs[j], allColumnIDs[i], i % 2 == 0 ? true : false);
                            break;
                        case Serializable.SerializableTypes.SByte:
                            table.SetSByte(allRowIDs[j], allColumnIDs[i], (sbyte)(i + j));
                            break;
                        case Serializable.SerializableTypes.Byte:
                            table.SetByte(allRowIDs[j], allColumnIDs[i], (byte)(i + j));
                            break;
                        case Serializable.SerializableTypes.Short:
                            table.SetShort(allRowIDs[j], allColumnIDs[i], (short)(i + j));
                            break;
                        case Serializable.SerializableTypes.UShort:
                            table.SetUShort(allRowIDs[j], allColumnIDs[i], (ushort)(i + j));
                            break;
                        case Serializable.SerializableTypes.Int:
                            table.SetInt(allRowIDs[j], allColumnIDs[i], (int)(i + j));
                            break;
                        case Serializable.SerializableTypes.UInt:
                            table.SetUInt(allRowIDs[j], allColumnIDs[i], (uint)(i + j));
                            break;
                        case Serializable.SerializableTypes.Long:
                            table.SetLong(allRowIDs[j], allColumnIDs[i], (long)(i + j));
                            break;
                        case Serializable.SerializableTypes.ULong:
                            table.SetULong(allRowIDs[j], allColumnIDs[i], (ulong)(i + j));
                            break;
                        case Serializable.SerializableTypes.Float:
                            table.SetFloat(allRowIDs[j], allColumnIDs[i], (float)(i + j));
                            break;
                        case Serializable.SerializableTypes.Double:
                            table.SetDouble(allRowIDs[j], allColumnIDs[i], (double)(i + j));
                            break;
                        case Serializable.SerializableTypes.Vector2:
                            table.SetVector2(allRowIDs[j], allColumnIDs[i], new Vector2(i, j));
                            break;
                        case Serializable.SerializableTypes.Vector3:
                            table.SetVector3(allRowIDs[j], allColumnIDs[i], new Vector3(i, j, i + j));
                            break;
                        case Serializable.SerializableTypes.Vector4:
                            table.SetVector4(allRowIDs[j], allColumnIDs[i], new Vector4(i, j, i + j, i - j));
                            break;
                        case Serializable.SerializableTypes.Vector2Int:
                            table.SetVector2Int(allRowIDs[j], allColumnIDs[i], new Vector2Int(i, j));
                            break;
                        case Serializable.SerializableTypes.Vector3Int:
                            table.SetVector3Int(allRowIDs[j], allColumnIDs[i], new Vector3Int(i, j, i + j));
                            break;
                        case Serializable.SerializableTypes.Quaternion:
                            table.SetQuaternion(allRowIDs[j], allColumnIDs[i], new Quaternion(i, j, i + j, i - j));
                            break;
                        case Serializable.SerializableTypes.Rect:
                            table.SetRect(allRowIDs[j], allColumnIDs[i], new Rect(i, j, i + j, i - j));
                            break;
                        case Serializable.SerializableTypes.RectInt:
                            table.SetRectInt(allRowIDs[j], allColumnIDs[i], new RectInt(i, j, i + j, i - j));
                            break;
                        case Serializable.SerializableTypes.Color:
                            table.SetColor(allRowIDs[j], allColumnIDs[i], new Color(i, j, i + j, i - j));
                            break;
                        case Serializable.SerializableTypes.LayerMask:
                            table.SetLayerMask(allRowIDs[j], allColumnIDs[i], (LayerMask)(i + j));
                            break;
                        case Serializable.SerializableTypes.Bounds:
                            table.SetBounds(allRowIDs[j], allColumnIDs[i], new Bounds(new Vector3(i, j, i + j), new Vector3(i, j, i + j)));
                            break;
                        case Serializable.SerializableTypes.BoundsInt:
                            table.SetBoundsInt(allRowIDs[j], allColumnIDs[i], new BoundsInt(new Vector3Int(i, j, i + j), new Vector3Int(i, j, i + j)));
                            break;
                        case Serializable.SerializableTypes.Hash128:
                            table.SetHash128(allRowIDs[j], allColumnIDs[i], new Hash128((ulong)i, (ulong)j));
                            break;
                        case Serializable.SerializableTypes.Gradient:
                            table.SetGradient(allRowIDs[j], allColumnIDs[i], new Gradient());
                            break;
                        case Serializable.SerializableTypes.AnimationCurve:
                            table.SetAnimationCurve(allRowIDs[j], allColumnIDs[i], new AnimationCurve());
                            break;
                        case Serializable.SerializableTypes.Object:
                            table.SetObject(allRowIDs[j], allColumnIDs[i], table);
                            break;
                        default:
                            break;
                    }
                }
            }

            bool allCellsValid = true;
            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                Serializable.SerializableTypes currentType = (Serializable.SerializableTypes)i;
                for (int j = 0; j < 10; j++)
                {
                    switch (currentType)
                    {
                        case Serializable.SerializableTypes.Invalid:
                            break;
                        case Serializable.SerializableTypes.String:
                            allCellsValid &= table.GetString(allRowIDs[j], allColumnIDs[i]) == i.ToString() + j.ToString() && table.GetStringRef(allRowIDs[j], allColumnIDs[i]) == i.ToString() + j.ToString();
                            break;
                        case Serializable.SerializableTypes.Char:
                            allCellsValid &= table.GetChar(allRowIDs[j], allColumnIDs[i]) == (char)(i + j) && table.GetCharRef(allRowIDs[j], allColumnIDs[i]) == (char)(i + j);
                            break;
                        case Serializable.SerializableTypes.Bool:
                            allCellsValid &= table.GetBool(allRowIDs[j], allColumnIDs[i]) == (i % 2 == 0 ? true : false) && table.GetBoolRef(allRowIDs[j], allColumnIDs[i]) == (i % 2 == 0 ? true : false);
                            break;
                        case Serializable.SerializableTypes.SByte:
                            allCellsValid &= table.GetSByte(allRowIDs[j], allColumnIDs[i]) == (sbyte)(i + j) && table.GetSbyteRef(allRowIDs[j], allColumnIDs[i]) == (sbyte)(i + j);
                            break;
                        case Serializable.SerializableTypes.Byte:
                            allCellsValid &= table.GetByte(allRowIDs[j], allColumnIDs[i]) == (byte)(i + j) && table.GetByteRef(allRowIDs[j], allColumnIDs[i]) == (byte)(i + j);
                            break;
                        case Serializable.SerializableTypes.Short:
                            allCellsValid &= table.GetShort(allRowIDs[j], allColumnIDs[i]) == (short)(i + j) && table.GetShortRef(allRowIDs[j], allColumnIDs[i]) == (short)(i + j);
                            break;
                        case Serializable.SerializableTypes.UShort:
                            allCellsValid &= table.GetUShort(allRowIDs[j], allColumnIDs[i]) == (ushort)(i + j) && table.GetUshortRef(allRowIDs[j], allColumnIDs[i]) == (ushort)(i + j);
                            break;
                        case Serializable.SerializableTypes.Int:
                            allCellsValid &= table.GetInt(allRowIDs[j], allColumnIDs[i]) == (int)(i + j) && table.GetIntRef(allRowIDs[j], allColumnIDs[i]) == (int)(i + j);
                            break;
                        case Serializable.SerializableTypes.UInt:
                            allCellsValid &= table.GetUInt(allRowIDs[j], allColumnIDs[i]) == (uint)(i + j) && table.GetUintRef(allRowIDs[j], allColumnIDs[i]) == (uint)(i + j);
                            break;
                        case Serializable.SerializableTypes.Long:
                            allCellsValid &= table.GetLong(allRowIDs[j], allColumnIDs[i]) == (long)(i + j) && table.GetLongRef(allRowIDs[j], allColumnIDs[i]) == (long)(i + j);
                            break;
                        case Serializable.SerializableTypes.ULong:
                            allCellsValid &= table.GetULong(allRowIDs[j], allColumnIDs[i]) == (ulong)(i + j) && table.GetUlongRef(allRowIDs[j], allColumnIDs[i]) == (ulong)(i + j);
                            break;
                        case Serializable.SerializableTypes.Float:
                            allCellsValid &= table.GetFloat(allRowIDs[j], allColumnIDs[i]) == (float)(i + j) && table.GetFloatRef(allRowIDs[j], allColumnIDs[i]) == (float)(i + j);
                            break;
                        case Serializable.SerializableTypes.Double:
                            allCellsValid &= table.GetDouble(allRowIDs[j], allColumnIDs[i]) == (double)(i + j) && table.GetDoubleRef(allRowIDs[j], allColumnIDs[i]) == (double)(i + j);
                            break;
                        case Serializable.SerializableTypes.Vector2:
                            allCellsValid &= table.GetVector2(allRowIDs[j], allColumnIDs[i]) == new Vector2(i, j) && table.GetVector2Ref(allRowIDs[j], allColumnIDs[i]) == new Vector2(i, j);
                            break;
                        case Serializable.SerializableTypes.Vector3:
                            allCellsValid &= table.GetVector3(allRowIDs[j], allColumnIDs[i]) == new Vector3(i, j, i + j) && table.GetVector3Ref(allRowIDs[j], allColumnIDs[i]) == new Vector3(i, j, i + j);
                            break;
                        case Serializable.SerializableTypes.Vector4:
                            allCellsValid &= table.GetVector4(allRowIDs[j], allColumnIDs[i]) == new Vector4(i, j, i + j, i - j) && table.GetVector4Ref(allRowIDs[j], allColumnIDs[i]) == new Vector4(i, j, i + j, i - j);
                            break;
                        case Serializable.SerializableTypes.Vector2Int:
                            allCellsValid &= table.GetVector2Int(allRowIDs[j], allColumnIDs[i]) == new Vector2Int(i, j) && table.GetVector2IntRef(allRowIDs[j], allColumnIDs[i]) == new Vector2Int(i, j);
                            break;
                        case Serializable.SerializableTypes.Vector3Int:
                            allCellsValid &= table.GetVector3Int(allRowIDs[j], allColumnIDs[i]) == new Vector3Int(i, j, i + j) && table.GetVector3IntRef(allRowIDs[j], allColumnIDs[i]) == new Vector3Int(i, j, i + j);
                            break;
                        case Serializable.SerializableTypes.Quaternion:
                            allCellsValid &= table.GetQuaternion(allRowIDs[j], allColumnIDs[i]) == new Quaternion(i, j, i + j, i - j) && table.GetQuaternionRef(allRowIDs[j], allColumnIDs[i]) == new Quaternion(i, j, i + j, i - j);
                            break;
                        case Serializable.SerializableTypes.Rect:
                            allCellsValid &= table.GetRect(allRowIDs[j], allColumnIDs[i]) == new Rect(i, j, i + j, i - j) && table.GetRectRef(allRowIDs[j], allColumnIDs[i]) == new Rect(i, j, i + j, i - j);
                            break;
                        case Serializable.SerializableTypes.RectInt:
                            allCellsValid &= table.GetRectInt(allRowIDs[j], allColumnIDs[i]).Equals(new RectInt(i, j, i + j, i - j)) && table.GetRectIntRef(allRowIDs[j], allColumnIDs[i]).Equals(new RectInt(i, j, i + j, i - j));
                            break;
                        case Serializable.SerializableTypes.Color:
                            allCellsValid &= table.GetColor(allRowIDs[j], allColumnIDs[i]).Equals(new Color(i, j, i + j, i - j)) && table.GetColorRef(allRowIDs[j], allColumnIDs[i]).Equals(new Color(i, j, i + j, i - j));
                            break;
                        case Serializable.SerializableTypes.LayerMask:
                            allCellsValid &= table.GetLayerMask(allRowIDs[j], allColumnIDs[i]) == (LayerMask)(i + j) && table.GetLayerMaskRef(allRowIDs[j], allColumnIDs[i]) == (LayerMask)(i + j);
                            break;
                        case Serializable.SerializableTypes.Bounds:
                            allCellsValid &= table.GetBounds(allRowIDs[j], allColumnIDs[i]) == new Bounds(new Vector3(i, j, i + j), new Vector3(i, j, i + j)) && table.GetBoundsRef(allRowIDs[j], allColumnIDs[i]) == new Bounds(new Vector3(i, j, i + j), new Vector3(i, j, i + j));
                            break;
                        case Serializable.SerializableTypes.BoundsInt:
                            allCellsValid &= table.GetBoundsInt(allRowIDs[j], allColumnIDs[i]).Equals(new BoundsInt(new Vector3Int(i, j, i + j), new Vector3Int(i, j, i + j))) && table.GetBoundsIntRef(allRowIDs[j], allColumnIDs[i]).Equals(new BoundsInt(new Vector3Int(i, j, i + j), new Vector3Int(i, j, i + j)));
                            break;
                        case Serializable.SerializableTypes.Hash128:
                            allCellsValid &= table.GetHash128(allRowIDs[j], allColumnIDs[i]).Equals(new Hash128((ulong)i, (ulong)j)) && table.GetHash128Ref(allRowIDs[j], allColumnIDs[i]).Equals(new Hash128((ulong)i, (ulong)j));
                            break;
                        case Serializable.SerializableTypes.Gradient:
                            allCellsValid &= table.GetGradient(allRowIDs[j], allColumnIDs[i]) != null && table.GetGradientRef(allRowIDs[j], allColumnIDs[i]) != null;
                            break;
                        case Serializable.SerializableTypes.AnimationCurve:
                            allCellsValid &= table.GetAnimationCurve(allRowIDs[j], allColumnIDs[i]) != null && table.GetAnimationCurveRef(allRowIDs[j], allColumnIDs[i]) != null;
                            break;
                        case Serializable.SerializableTypes.Object:
                            allCellsValid &= table.GetObject(allRowIDs[j], allColumnIDs[i]) == table && table.GetObjectRef(allRowIDs[j], allColumnIDs[i]) == table;
                            break;
                        default:
                            break;
                    }
                }
            }

            Assert.IsTrue(allCellsValid);
        }

        //[Test]
        //[Category(Core.TestCategory)]
        //[UnityEditor.MenuItem("Tools/Blargh")]
        static void StableTable_STUFF()
        {
            StableTable table = ScriptableObject.CreateInstance<StableTable>();

            System.Collections.Generic.List<int> ids = new System.Collections.Generic.List<int>();
            System.Collections.Generic.HashSet<int> toRemoveList = new System.Collections.Generic.HashSet<int>();

            for (int i = 0; i < 32; i++)
            {
                int currID = table.AddColumn(Serializable.SerializableTypes.String, i.ToString());
                Debug.Log("Add " + currID);
                ids.Add(currID);
            }

            for (int i = 0; i < 32; i++)
            {
                if (!ids.Contains(i))
                {
                    throw new Exception("What");
                }
            }

            table.AddRow("A");
            table.AddRow("B");
            PrintTable(table);
            for (int i = 0; i < 100; i++)
            {
                int columnCount = table.GetColumnCount();
                int random = UnityEngine.Random.Range(1, 5);
                bool direction = UnityEngine.Random.Range(0, 2) == 1;
                if (columnCount <= 32)
                {
                    Debug.Log("New add run.");
                    for (int j = 0; j < random; j++)
                    {
                        PrintTable(table);
                        int newID = table.AddColumn(Serializable.SerializableTypes.String, j.ToString());
                        Debug.Log("Add " + newID);
                        if (ids.Contains(newID))
                        {
                            throw new Exception("table contains new ID already: " + newID);
                        }
                        ids.Add(newID);
                    }
                }
                else if (columnCount >= 42)
                {
                    var descriptions = table.GetAllColumnDescriptions();

                    while (toRemoveList.Count < random)
                    {
                        int index = UnityEngine.Random.Range(0, descriptions.Length);
                        toRemoveList.Add(index);
                    }

                    Debug.Log("New remove run.");
                    foreach(int randomIndex in toRemoveList)
                    {
                        PrintTable(table);
                        int internalIndex = descriptions[randomIndex].InternalIndex;
                        Debug.Log("Remove " + internalIndex);
                        
                        if (!ids.Contains(internalIndex))
                        {
                            throw new Exception("table contains ID not tracked: " + internalIndex);
                        }
                        ids.Remove(internalIndex);
                        table.RemoveColumn(Serializable.SerializableTypes.String, internalIndex);

                        if (ids.Contains(internalIndex))
                        {
                            throw new Exception("table STILL contains ID: " + internalIndex);
                        }
                    }

                    toRemoveList.Clear();
                }
                else
                {
                    if (direction)
                    {
                        Debug.Log("New add run.");
                        for (int j = 0; j < random; j++)
                        {
                            PrintTable(table);
                            int newID = table.AddColumn(Serializable.SerializableTypes.String, j.ToString());
                            Debug.Log("Add " + newID);
                            if (ids.Contains(newID))
                            {
                                throw new Exception("table contains new ID already: " + newID);
                            }
                            ids.Add(newID);
                        }
                    }
                    else
                    {
                        var descriptions = table.GetAllColumnDescriptions();

                        while (toRemoveList.Count < random)
                        {
                            int index = UnityEngine.Random.Range(0, descriptions.Length);
                            toRemoveList.Add(index);
                        }

                        Debug.Log("New remove run.");
                        foreach (int randomIndex in toRemoveList)
                        {
                            PrintTable(table);
                            int internalIndex = descriptions[randomIndex].InternalIndex;
                            Debug.Log("Remove " + internalIndex);
                            if (!ids.Contains(internalIndex))
                            {
                                throw new Exception("table contains ID not tracked: " + internalIndex);
                            }
                            ids.Remove(internalIndex);
                            table.RemoveColumn(Serializable.SerializableTypes.String, internalIndex);

                            if (ids.Contains(internalIndex))
                            {
                                throw new Exception("table STILL contains ID: " + internalIndex);
                            }
                        }

                        toRemoveList.Clear();
                    }

                }
            }


            Assert.IsTrue(true);
        }

        static void PrintTable(StableTable table)
        {
            var allColumnDescriptions = table.GetAllColumnDescriptions();

            string allColumnDescriptionsString = "descript(" + allColumnDescriptions.Length + "): ";

            for (int i = 0; i < allColumnDescriptions.Length; i++)
            {
                allColumnDescriptionsString += allColumnDescriptions[i].InternalIndex + ",";
            }

            string allIDsString = "allIDsToSorted(" + table.ColumnIDToSortOrderMap.Length + "): ";

            for (int i = 0; i < table.ColumnIDToSortOrderMap.Length; i++)
            {
                allIDsString += table.ColumnIDToSortOrderMap[i] + ",";
            }

            string sortedOrderToIDMapString = "sortedIDs(" + table.SortedOrderToColumnIDMap.Length + "): ";
            for (int i = 0; i < table.SortedOrderToColumnIDMap.Length; i++)
            {
                sortedOrderToIDMapString += table.SortedOrderToColumnIDMap[i] + ",";
            }

            string columnIDs = "allIDs(" + table.ColumnIDToDenseIndexMap.Length + "): ";
            string columnIDVals = "allIDsVals: ";
            for (int i = 0; i < table.ColumnIDToDenseIndexMap.Length; i++)
            {
                columnIDs += (table.ColumnIDToDenseIndexMap[i].ColumnType == Serializable.SerializableTypes.Invalid ? "0" : "1") + ",";
                columnIDVals += table.ColumnIDToDenseIndexMap[i].ColumnDenseIndex + ",";
            }
            Debug.Log(allColumnDescriptionsString + System.Environment.NewLine + allIDsString + System.Environment.NewLine + sortedOrderToIDMapString + System.Environment.NewLine + columnIDs + System.Environment.NewLine + columnIDVals);
        }
    }
}
