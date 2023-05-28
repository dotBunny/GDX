// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using GDX.Developer;
using GDX.Tables;
using GDX.Tables.CellValues;

namespace GDX
{
    public static class TableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string MakeContentSafeForCSV(string content)
        {
            if (content == null) return string.Empty;

            // Double quote quotes
            if(content.IndexOf('"') != -1)
            {
                content = content.Replace("\"", "\"\"");
            }

            // Ensure quotes for commas
            return content.IndexOf(',') != -1 ? $"\"{content}\"" : content;
        }

        public static void ToCSV(this TableBase table, string filePath)
        {
            int rowCount = table.GetRowCount();
            int columnCount = table.GetColumnCount();
            TextGenerator generator = new TextGenerator();

            // Build first line
            TableBase.ColumnDescription[] columnDescriptions = table.GetAllColumnDescriptions();
            generator.Append("Row Name");
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
                generator.Append(MakeContentSafeForCSV(rowDescription.Name));
                for (int c = 0; c < columnCount; c++)
                {
                    TableBase.ColumnDescription columnDescription = table.GetColumnDescription(c);
                    generator.Append(", ");
                    switch (columnDescription.Type)
                    {
                        case Serializable.SerializableTypes.String:
                            generator.Append(MakeContentSafeForCSV(table.GetString(rowDescription.InternalIndex,
                                columnDescription.InternalIndex)));
                            break;
                        case Serializable.SerializableTypes.Char:
                            generator.Append(MakeContentSafeForCSV(table
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