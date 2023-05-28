// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using GDX.Tables;
using GDX.Tables.CellValues;
using UnityEngine;
using TextGenerator = GDX.Developer.TextGenerator;

namespace GDX
{
    public static class TableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string MakeSafeString(string content)
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

        static string[] GetDataFromSafeStrings(string csvText)
        {
            List<string> tokens = new List<string>();

            int last = -1;
            int current = 0;
            bool inText = false;

            while(current < csvText.Length)
            {
                switch(csvText[current])
                {
                    case '"':
                        inText = !inText; break;
                    case ',':
                        if (!inText)
                        {
                            tokens.Add(csvText.Substring(last + 1, (current - last)).Trim(' ', ','));
                            last = current;
                        }
                        break;
                }
                current++;
            }

            if (last != csvText.Length - 1)
            {
                tokens.Add(csvText.Substring(last+1).Trim());
            }

            return tokens.ToArray();
        }

        struct VirtualRow
        {
            int RowID;
            string RowName;
            string[] Values;
        }

        public static bool FromCSV(this TableBase table, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Unable to find {filePath}.");
                return false;
            }

            string[] fileContent = File.ReadAllLines(filePath);
            int tableRowCount = table.GetRowCount();
            int tableColumnCount = table.GetColumnCount();
            TableBase.ColumnDescription[] columnDescriptions = table.GetAllColumnDescriptions();

            // Test Columns
            string[] columnTest = GetDataFromSafeStrings(fileContent[0]);
            if (columnTest.Length != tableColumnCount + 2)
            {
                Debug.LogError($"The importing data has {columnTest.Length} columns where {tableColumnCount + 2} was expected.");
                return false;
            }

            // Build a list of previous row ID, so we know what was removed
            List<int> previousRowInternalIndices = new List<int>(tableRowCount);
            TableBase.RowDescription[] rowDescriptions = table.GetAllRowDescriptions();
            int rowDescriptionsLength = rowDescriptions.Length;
            for (int i = 0; i < rowDescriptionsLength; i++)
            {
                previousRowInternalIndices.Add(rowDescriptions[i].InternalIndex);
            }
            List<int> foundRowInternalIndices = new List<int>(tableRowCount);

            for (int i = 1; i < fileContent.Length; i++)
            {
                int internalIndex = -1;
                string[] rowStrings = GetDataFromSafeStrings(fileContent[i]);
                if (string.IsNullOrEmpty(rowStrings[0]))
                {
                    string rowName = rowStrings[1];
                    if (string.IsNullOrEmpty(rowName))
                    {
                        rowName = "Unnamed";
                    }

                    // Create new row
                    internalIndex = table.AddRow(rowName);
                }
                else
                {
                    internalIndex = int.Parse(rowStrings[0]);
                }

                foundRowInternalIndices.Add(internalIndex);

                for (int j = 0; j < tableColumnCount; j++)
                {
                    string columnString = rowStrings[j + 2];
                    switch (columnDescriptions[j].Type)
                    {
                        case Serializable.SerializableTypes.String:
                            table.SetString(internalIndex, columnDescriptions[j].InternalIndex, columnString);
                            break;
                        case Serializable.SerializableTypes.Char:
                            table.SetChar(internalIndex, columnDescriptions[j].InternalIndex, columnString[0]);
                            break;
                        case Serializable.SerializableTypes.Bool:
                            table.SetBool(internalIndex, columnDescriptions[j].InternalIndex, bool.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.SByte:
                            table.SetSByte(internalIndex, columnDescriptions[j].InternalIndex,
                                sbyte.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Byte:
                            table.SetByte(internalIndex, columnDescriptions[j].InternalIndex, byte.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Short:
                            table.SetShort(internalIndex, columnDescriptions[j].InternalIndex,
                                short.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.UShort:
                            table.SetUShort(internalIndex, columnDescriptions[j].InternalIndex,
                                ushort.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Int:
                            table.SetInt(internalIndex, columnDescriptions[j].InternalIndex, int.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.UInt:
                            table.SetUInt(internalIndex, columnDescriptions[j].InternalIndex, uint.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Long:
                            table.SetLong(internalIndex, columnDescriptions[j].InternalIndex, long.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.ULong:
                            table.SetULong(internalIndex, columnDescriptions[j].InternalIndex,
                                ulong.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Float:
                            table.SetFloat(internalIndex, columnDescriptions[j].InternalIndex,
                                float.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Double:
                            table.SetDouble(internalIndex, columnDescriptions[j].InternalIndex,
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
                table.RemoveRow(previousRowInternalIndices[i]);
            }

            return true;
        }

        public static void ToCSV(this TableBase table, string filePath)
        {
            int rowCount = table.GetRowCount();
            int columnCount = table.GetColumnCount();
            TextGenerator generator = new TextGenerator();

            // Build first line
            TableBase.ColumnDescription[] columnDescriptions = table.GetAllColumnDescriptions();
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
                TableBase.RowDescription rowDescription = table.GetRowDescription(r);
                generator.Append($"{rowDescription.InternalIndex}, {MakeSafeString(rowDescription.Name)}");
                for (int c = 0; c < columnCount; c++)
                {
                    TableBase.ColumnDescription columnDescription = table.GetColumnDescription(c);
                    generator.Append(", ");
                    switch (columnDescription.Type)
                    {
                        case Serializable.SerializableTypes.String:
                            generator.Append(MakeSafeString(table.GetString(rowDescription.InternalIndex,
                                columnDescription.InternalIndex)));
                            break;
                        case Serializable.SerializableTypes.Char:
                            generator.Append(MakeSafeString(table
                                .GetChar(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString()));
                            break;
                        case Serializable.SerializableTypes.Bool:
                            generator.Append(table
                                .GetBool(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.SByte:
                            generator.Append(table
                                .GetSByte(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Byte:
                            generator.Append(table
                                .GetByte(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Short:
                            generator.Append(table
                                .GetShort(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.UShort:
                            generator.Append(table
                                .GetUShort(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Int:
                            generator.Append(table
                                .GetInt(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.UInt:
                            generator.Append(table
                                .GetUInt(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Long:
                            generator.Append(table
                                .GetLong(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.ULong:
                            generator.Append(table
                                .GetULong(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Float:
                            generator.Append(table
                                .GetFloat(rowDescription.InternalIndex, columnDescription.InternalIndex)
                                .ToString(CultureInfo.InvariantCulture));
                            break;
                        case Serializable.SerializableTypes.Double:
                            generator.Append(table
                                .GetDouble(rowDescription.InternalIndex, columnDescription.InternalIndex)
                                .ToString(CultureInfo.InvariantCulture));
                            break;
                        case Serializable.SerializableTypes.Vector2:
                            generator.Append(table
                                .GetUShort(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Vector3:
                            generator.Append(table
                                .GetVector3(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Vector4:
                            generator.Append(table
                                .GetVector4(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Vector2Int:
                            generator.Append(table
                                .GetVector2Int(rowDescription.InternalIndex, columnDescription.InternalIndex)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Vector3Int:
                            generator.Append(table
                                .GetVector3Int(rowDescription.InternalIndex, columnDescription.InternalIndex)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Quaternion:
                            generator.Append(table
                                .GetQuaternion(rowDescription.InternalIndex, columnDescription.InternalIndex)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Rect:
                            generator.Append(table
                                .GetRect(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.RectInt:
                            generator.Append(table
                                .GetRectInt(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Color:
                            generator.Append(table
                                .GetColor(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.LayerMask:
                            generator.Append(table
                                .GetLayerMask(rowDescription.InternalIndex, columnDescription.InternalIndex)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Bounds:
                            generator.Append(table
                                .GetBounds(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.BoundsInt:
                            generator.Append(table
                                .GetBoundsInt(rowDescription.InternalIndex, columnDescription.InternalIndex)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Hash128:
                            generator.Append(table
                                .GetHash128(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.Gradient:
                            generator.Append(table
                                .GetGradient(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                        case Serializable.SerializableTypes.AnimationCurve:
                            generator.Append(table
                                .GetAnimationCurve(rowDescription.InternalIndex, columnDescription.InternalIndex)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Object:
                            generator.Append(table
                                .GetObject(rowDescription.InternalIndex, columnDescription.InternalIndex).ToString());
                            break;
                    }
                }

                generator.NextLine();
            }

            File.WriteAllText(filePath, generator.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringCellValue GetStringCellValue(this TableBase table, int rowID, int columnID)
        {
            return new StringCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoolCellValue GetBoolCellValue(this TableBase table, int rowID, int columnID)
        {
            return new BoolCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CharCellValue GetCharCellValue(this TableBase table, int rowID, int columnID)
        {
            return new CharCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByteCellValue GetSByteCellValue(this TableBase table, int rowID, int columnID)
        {
            return new SByteCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByteCellValue GetByteCellValue(this TableBase table, int rowID, int columnID)
        {
            return new ByteCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ShortCellValue GetShortCellValue(this TableBase table, int rowID, int columnID)
        {
            return new ShortCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UShortCellValue GetUShortCellValue(this TableBase table, int rowID, int columnID)
        {
            return new UShortCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntCellValue GetIntCellValue(this TableBase table, int rowID, int columnID)
        {
            return new IntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UIntCellValue GetUIntCellValue(this TableBase table, int rowID, int columnID)
        {
            return new UIntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LongCellValue GetLongCellValue(this TableBase table, int rowID, int columnID)
        {
            return new LongCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ULongCellValue GetULongCellValue(this TableBase table, int rowID, int columnID)
        {
            return new ULongCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatCellValue GetFloatCellValue(this TableBase table, int rowID, int columnID)
        {
            return new FloatCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleCellValue GetDoubleCellValue(this TableBase table, int rowID, int columnID)
        {
            return new DoubleCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2CellValue GetVector2CellValue(this TableBase table, int rowID, int columnID)
        {
            return new Vector2CellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3CellValue GetVector3CellValue(this TableBase table, int rowID, int columnID)
        {
            return new Vector3CellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4CellValue GetVector4CellValue(this TableBase table, int rowID, int columnID)
        {
            return new Vector4CellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2IntCellValue GetVector2IntCellValue(this TableBase table, int rowID, int columnID)
        {
            return new Vector2IntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3IntCellValue GetVector3IntCellValue(this TableBase table, int rowID, int columnID)
        {
            return new Vector3IntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QuaternionCellValue GetQuaternionCellValue(this TableBase table, int rowID, int columnID)
        {
            return new QuaternionCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectCellValue GetRectCellValue(this TableBase table, int rowID, int columnID)
        {
            return new RectCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectIntCellValue GetRectIntCellValue(this TableBase table, int rowID, int columnID)
        {
            return new RectIntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColorCellValue GetColorCellValue(this TableBase table, int rowID, int columnID)
        {
            return new ColorCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMaskCellValue GetLayerMaskCellValue(this TableBase table, int rowID, int columnID)
        {
            return new LayerMaskCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsCellValue GetBoundsCellValue(this TableBase table, int rowID, int columnID)
        {
            return new BoundsCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsIntCellValue GetBoundsIntCellValue(this TableBase table, int rowID, int columnID)
        {
            return new BoundsIntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Hash128CellValue GetHash128CellValue(this TableBase table, int rowID, int columnID)
        {
            return new Hash128CellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GradientCellValue GetGradientCellValue(this TableBase table, int rowID, int columnID)
        {
            return new GradientCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnimationCurveCellValue GetAnimationCurveCellValue(this TableBase table, int rowID, int columnID)
        {
            return new AnimationCurveCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectCellValue GetObjectCellValue(this TableBase table, int rowID, int columnID)
        {
            return new ObjectCellValue(table, rowID, columnID);
        }
    }
}