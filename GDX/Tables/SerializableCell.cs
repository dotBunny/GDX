// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Tables.CellValues;

namespace GDX.Tables
{
    [Serializable]
    public struct SerializableCell
    {
        public Serializable.SerializableTypes Type;
        public UnityEngine.Object Table;
        public int Row;
        public int Column;

        public object GetCellValue()
        {
            switch (Type)
            {
                case Serializable.SerializableTypes.String:
                    return new StringCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Char:
                    return new CharCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Bool:
                    return new BoolCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.SByte:
                    return new SByteCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Byte:
                    return new ByteCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Short:
                    return new ShortCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.UShort:
                    return new UShortCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Int:
                    return new IntCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.UInt:
                    return new UIntCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Long:
                    return new LongCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.ULong:
                    return new ULongCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Float:
                    return new FloatCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Double:
                    return new DoubleCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Vector2:
                    return new Vector2CellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Vector3:
                    return new Vector3CellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Vector4:
                    return new Vector4CellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Vector2Int:
                    return new Vector2IntCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Vector3Int:
                    return new Vector3IntCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Quaternion:
                    return new QuaternionCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Rect:
                    return new RectCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.RectInt:
                    return new RectIntCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Color:
                    return new ColorCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.LayerMask:
                    return new LayerMaskCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Bounds:
                    return new BoundsCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.BoundsInt:
                    return new BoundsIntCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Hash128:
                    return new Hash128CellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Gradient:
                    return new GradientCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.AnimationCurve:
                    return new AnimationCurveCellValue((TableBase)Table, Row, Column);
                case Serializable.SerializableTypes.Object:
                    return new ObjectCellValue((TableBase)Table, Row, Column);
            }
            return null;
        }

        public static SerializableCell Get(StringCellValue stringCellValue)
        {
            return new SerializableCell()
            {
                Type = Serializable.SerializableTypes.String,
                Table = stringCellValue.Table as UnityEngine.Object,
                Row = stringCellValue.Row,
                Column = stringCellValue.Column
            };
        }
    }
}