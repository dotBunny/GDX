// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using GDX.DataTables.CellValues;
using UnityEngine;
using TextGenerator = GDX.Developer.TextGenerator;

namespace GDX.DataTables
{
    public static class DataTableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string MakeCommaSeperatedValue(string content)
        {
            if (content == null)
            {
                return string.Empty;
            }

            // Double quote quotes
            if (content.IndexOf('"') != -1)
            {
                content = content.Replace("\"", "\"\"");
            }

            // Ensure quotes for commas
            return content.IndexOf(',') != -1 ? $"\"{content}\"" : content;
        }

        static string[] ParseCommaSeperatedValues(string content)
        {
            List<string> returnStrings = new List<string>();
            int lastIndex = -1;
            int currentIndex = 0;
            bool isQuoted = false;
            int length = content.Length;
            while(currentIndex < length)
            {
                switch(content[currentIndex])
                {
                    case '"':
                        isQuoted = !isQuoted;
                        break;
                    case ',':
                        if (!isQuoted)
                        {
                            returnStrings.Add(content.Substring(lastIndex + 1, (currentIndex - lastIndex)).Trim(' ', ','));
                            lastIndex = currentIndex;
                        }
                        break;
                }
                currentIndex++;
            }
            if (lastIndex != content.Length - 1)
            {
                returnStrings.Add(content.Substring(lastIndex+1).Trim());
            }
            return returnStrings.ToArray();
        }

        public static bool UpdateFromCommaSeperatedValues(this DataTableObject dataTable, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Unable to find {filePath}.");
                return false;
            }

            string[] fileContent = File.ReadAllLines(filePath);
            int tableRowCount = dataTable.GetRowCount();
            int tableColumnCount = dataTable.GetColumnCount();
            DataTableObject.ColumnDescription[] columnDescriptions = dataTable.GetAllColumnDescriptions();

            // Test Columns
            string[] columnTest = ParseCommaSeperatedValues(fileContent[0]);
            if (columnTest.Length != tableColumnCount + 2)
            {
                Debug.LogError($"The importing data has {columnTest.Length} columns where {tableColumnCount + 2} was expected.");
                return false;
            }

            // Build a list of previous row ID, so we know what was removed
            List<int> previousRowInternalIndices = new List<int>(tableRowCount);
            DataTableObject.RowDescription[] rowDescriptions = dataTable.GetAllRowDescriptions();
            int rowDescriptionsLength = rowDescriptions.Length;
            for (int i = 0; i < rowDescriptionsLength; i++)
            {
                previousRowInternalIndices.Add(rowDescriptions[i].Identifier);
            }
            List<int> foundRowInternalIndices = new List<int>(tableRowCount);

            for (int i = 1; i < fileContent.Length; i++)
            {
                int rowIdentifier = -1;
                string[] rowStrings = ParseCommaSeperatedValues(fileContent[i]);
                if (string.IsNullOrEmpty(rowStrings[0]))
                {
                    string rowName = rowStrings[1];
                    if (string.IsNullOrEmpty(rowName))
                    {
                        rowName = "Unnamed";
                    }

                    // Create new row
                    rowIdentifier = dataTable.AddRow(rowName);
                }
                else
                {
                    rowIdentifier = int.Parse(rowStrings[0]);
                }

                foundRowInternalIndices.Add(rowIdentifier);

                for (int j = 0; j < tableColumnCount; j++)
                {
                    string columnString = rowStrings[j + 2];
                    switch (columnDescriptions[j].Type)
                    {
                        case Serializable.SerializableTypes.String:
                            dataTable.SetString(rowIdentifier, columnDescriptions[j].Identifier, columnString);
                            break;
                        case Serializable.SerializableTypes.Char:
                            dataTable.SetChar(rowIdentifier, columnDescriptions[j].Identifier, columnString[0]);
                            break;
                        case Serializable.SerializableTypes.Bool:
                            dataTable.SetBool(rowIdentifier, columnDescriptions[j].Identifier, bool.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.SByte:
                            dataTable.SetSByte(rowIdentifier, columnDescriptions[j].Identifier,
                                sbyte.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Byte:
                            dataTable.SetByte(rowIdentifier, columnDescriptions[j].Identifier, byte.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Short:
                            dataTable.SetShort(rowIdentifier, columnDescriptions[j].Identifier,
                                short.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.UShort:
                            dataTable.SetUShort(rowIdentifier, columnDescriptions[j].Identifier,
                                ushort.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Int:
                            dataTable.SetInt(rowIdentifier, columnDescriptions[j].Identifier, int.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.UInt:
                            dataTable.SetUInt(rowIdentifier, columnDescriptions[j].Identifier, uint.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Long:
                            dataTable.SetLong(rowIdentifier, columnDescriptions[j].Identifier, long.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.ULong:
                            dataTable.SetULong(rowIdentifier, columnDescriptions[j].Identifier,
                                ulong.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Float:
                            dataTable.SetFloat(rowIdentifier, columnDescriptions[j].Identifier,
                                float.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Double:
                            dataTable.SetDouble(rowIdentifier, columnDescriptions[j].Identifier,
                                double.Parse(columnString));
                            break;
                    }
                }
            }

            // Remove indices that were not found any more?
            int foundIndicesCount = foundRowInternalIndices.Count;
            for (int i = 0; i < foundIndicesCount; i++)
            {
                if (previousRowInternalIndices.Contains(foundRowInternalIndices[i]))
                {
                    previousRowInternalIndices.Remove(foundRowInternalIndices[i]);
                }
            }

            int indicesToRemove = previousRowInternalIndices.Count;
            for(int i = 0; i < indicesToRemove; i++)
            {
                dataTable.RemoveRow(previousRowInternalIndices[i]);
            }

            return true;
        }

        public static void ExportToCommaSeperatedValues(this DataTableObject dataTable, string filePath)
        {
            int rowCount = dataTable.GetRowCount();
            int columnCount = dataTable.GetColumnCount();
            TextGenerator generator = new TextGenerator();

            // Build first line
            DataTableObject.ColumnDescription[] columnDescriptions = dataTable.GetAllColumnDescriptions();
            generator.Append("Row ID, Row Name");
            for (int i = 0; i < columnCount; i++)
            {
                generator.Append(", ");
                generator.Append(columnDescriptions[i].Name);
            }

            generator.NextLine();

            // Build lines for rows
            for (int r = 0; r < rowCount; r++)
            {
                DataTableObject.RowDescription rowDescription = dataTable.GetRowDescription(r);
                generator.Append($"{rowDescription.Identifier}, {MakeCommaSeperatedValue(rowDescription.Name)}");
                for (int c = 0; c < columnCount; c++)
                {
                    DataTableObject.ColumnDescription columnDescription = dataTable.GetColumnDescription(c);
                    generator.Append(", ");
                    switch (columnDescription.Type)
                    {
                        case Serializable.SerializableTypes.String:
                            generator.Append(MakeCommaSeperatedValue(dataTable.GetString(rowDescription.Identifier,
                                columnDescription.Identifier)));
                            break;
                        case Serializable.SerializableTypes.Char:
                            generator.Append(MakeCommaSeperatedValue(dataTable
                                .GetChar(rowDescription.Identifier, columnDescription.Identifier).ToString()));
                            break;
                        case Serializable.SerializableTypes.Bool:
                            generator.Append(dataTable
                                .GetBool(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.SByte:
                            generator.Append(dataTable
                                .GetSByte(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Byte:
                            generator.Append(dataTable
                                .GetByte(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Short:
                            generator.Append(dataTable
                                .GetShort(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.UShort:
                            generator.Append(dataTable
                                .GetUShort(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Int:
                            generator.Append(dataTable
                                .GetInt(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.UInt:
                            generator.Append(dataTable
                                .GetUInt(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Long:
                            generator.Append(dataTable
                                .GetLong(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.ULong:
                            generator.Append(dataTable
                                .GetULong(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Float:
                            generator.Append(dataTable
                                .GetFloat(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString(CultureInfo.InvariantCulture));
                            break;
                        case Serializable.SerializableTypes.Double:
                            generator.Append(dataTable
                                .GetDouble(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString(CultureInfo.InvariantCulture));
                            break;
                        case Serializable.SerializableTypes.Vector2:
                            generator.Append(dataTable
                                .GetUShort(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Vector3:
                            generator.Append(dataTable
                                .GetVector3(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Vector4:
                            generator.Append(dataTable
                                .GetVector4(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Vector2Int:
                            generator.Append(dataTable
                                .GetVector2Int(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Vector3Int:
                            generator.Append(dataTable
                                .GetVector3Int(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Quaternion:
                            generator.Append(dataTable
                                .GetQuaternion(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Rect:
                            generator.Append(dataTable
                                .GetRect(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.RectInt:
                            generator.Append(dataTable
                                .GetRectInt(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Color:
                            generator.Append(dataTable
                                .GetColor(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.LayerMask:
                            generator.Append(dataTable
                                .GetLayerMask(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Bounds:
                            generator.Append(dataTable
                                .GetBounds(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.BoundsInt:
                            generator.Append(dataTable
                                .GetBoundsInt(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Hash128:
                            generator.Append(dataTable
                                .GetHash128(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Gradient:
                            generator.Append(dataTable
                                .GetGradient(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.AnimationCurve:
                            generator.Append(dataTable
                                .GetAnimationCurve(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Object:
                            generator.Append(dataTable
                                .GetObject(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                    }
                }

                generator.NextLine();
            }

            File.WriteAllText(filePath, generator.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringCellValue GetStringCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new StringCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoolCellValue GetBoolCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new BoolCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CharCellValue GetCharCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new CharCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByteCellValue GetSByteCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new SByteCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByteCellValue GetByteCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new ByteCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ShortCellValue GetShortCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new ShortCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UShortCellValue GetUShortCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new UShortCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntCellValue GetIntCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new IntCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UIntCellValue GetUIntCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new UIntCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LongCellValue GetLongCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new LongCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ULongCellValue GetULongCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new ULongCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatCellValue GetFloatCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new FloatCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleCellValue GetDoubleCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new DoubleCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2CellValue GetVector2CellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new Vector2CellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3CellValue GetVector3CellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new Vector3CellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4CellValue GetVector4CellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new Vector4CellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2IntCellValue GetVector2IntCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new Vector2IntCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3IntCellValue GetVector3IntCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new Vector3IntCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QuaternionCellValue GetQuaternionCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new QuaternionCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectCellValue GetRectCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new RectCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectIntCellValue GetRectIntCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new RectIntCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColorCellValue GetColorCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new ColorCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMaskCellValue GetLayerMaskCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new LayerMaskCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsCellValue GetBoundsCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new BoundsCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsIntCellValue GetBoundsIntCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new BoundsIntCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Hash128CellValue GetHash128CellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new Hash128CellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GradientCellValue GetGradientCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new GradientCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnimationCurveCellValue GetAnimationCurveCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new AnimationCurveCellValue(dataTable, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectCellValue GetObjectCellValue(this DataTableObject dataTable, int rowID, int columnID)
        {
            return new ObjectCellValue(dataTable, rowID, columnID);
        }
    }
}