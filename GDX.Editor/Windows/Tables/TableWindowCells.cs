// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Collections.Generic;
using GDX.Tables.CellValues;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    static class TableWindowCells
    {
        const string k_RowHeaderFieldName = "gdx-table-row-header";
        const string k_CellFieldName = "gdx-table-field";

        static readonly Dictionary<VisualElement, TableWindowCellMapping> k_CellToMapping =
            new Dictionary<VisualElement, TableWindowCellMapping>();

        internal static void DestroyCell(VisualElement cell)
        {
            k_CellToMapping.Remove(cell);
        }

        internal static void CleanTableReferences(int knownTableIndex)
        {
            int count = k_CellToMapping.Count;
            SimpleList<VisualElement> elementsToClean = new SimpleList<VisualElement>(count);
            foreach (KeyValuePair<VisualElement, TableWindowCellMapping> kvp in k_CellToMapping)
            {
                if (kvp.Value.KnownTableIndex == knownTableIndex)
                {
                    elementsToClean.AddUnchecked(kvp.Key);
                }
            }

            for (int i = 0; i < elementsToClean.Count; i++)
            {
                k_CellToMapping.Remove(elementsToClean.Array[i]);
            }
        }

        internal static void BindStringCell(VisualElement cell, int row)
        {
            TextField field = (TextField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            StringCellValue cellValue =
                new StringCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                StringCellValue local = (StringCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindCharCell(VisualElement cell, int row)
        {
            TextField field = (TextField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            CharCellValue cellValue =
                new CharCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe().ToString());
            field.RegisterValueChangedCallback(e =>
            {
                CharCellValue local = (CharCellValue)field.userData;
                local.Set(e.newValue[0]);
            });
        }

        internal static void BindBoolCell(VisualElement cell, int row)
        {
            Toggle field = (Toggle)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];


            BoolCellValue cellValue =
                new BoolCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                BoolCellValue local = (BoolCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindSByteCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            SByteCellValue cellValue =
                new SByteCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                SByteCellValue local = (SByteCellValue)field.userData;
                local.Set((sbyte)e.newValue);
            });
        }

        internal static void BindByteCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            ByteCellValue cellValue =
                new ByteCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ByteCellValue local = (ByteCellValue)field.userData;
                local.Set((byte)e.newValue);
            });
        }

        internal static void BindShortCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            ShortCellValue cellValue =
                new ShortCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ShortCellValue local = (ShortCellValue)field.userData;
                local.Set((short)e.newValue);
            });
        }

        internal static void BindUShortCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            UShortCellValue cellValue =
                new UShortCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                UShortCellValue local = (UShortCellValue)field.userData;
                local.Set((ushort)e.newValue);
            });
        }

        internal static void BindIntCell(VisualElement cell, int row)
        {
            IntegerField field = (IntegerField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            IntCellValue cellValue =
                new IntCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                IntCellValue local = (IntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindUIntCell(VisualElement cell, int row)
        {
            IntegerField field = (IntegerField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            UIntCellValue cellValue =
                new UIntCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify((int)cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                UIntCellValue local = (UIntCellValue)field.userData;
                local.Set((uint)e.newValue);
            });
        }

        internal static void BindLongCell(VisualElement cell, int row)
        {
            LongField field = (LongField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            LongCellValue cellValue =
                new LongCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                LongCellValue local = (LongCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindULongCell(VisualElement cell, int row)
        {
            LongField field = (LongField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            ULongCellValue cellValue =
                new ULongCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify((long)cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ULongCellValue local = (ULongCellValue)field.userData;
                local.Set((ulong)e.newValue);
            });
        }

        internal static void BindFloatCell(VisualElement cell, int row)
        {
            FloatField field = (FloatField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            FloatCellValue cellValue =
                new FloatCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                FloatCellValue local = (FloatCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindDoubleCell(VisualElement cell, int row)
        {
            DoubleField field = (DoubleField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            DoubleCellValue cellValue =
                new DoubleCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                DoubleCellValue local = (DoubleCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindVector2Cell(VisualElement cell, int row)
        {
            Vector2Field field = (Vector2Field)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            Vector2CellValue cellValue =
                new Vector2CellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector2CellValue local = (Vector2CellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindVector3Cell(VisualElement cell, int row)
        {
            Vector3Field field = (Vector3Field)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            Vector3CellValue cellValue =
                new Vector3CellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector3CellValue local = (Vector3CellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindVector4Cell(VisualElement cell, int row)
        {
            Vector4Field field = (Vector4Field)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            Vector4CellValue cellValue =
                new Vector4CellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector4CellValue local = (Vector4CellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindVector2IntCell(VisualElement cell, int row)
        {
            Vector2IntField field = (Vector2IntField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            Vector2IntCellValue cellValue =
                new Vector2IntCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector2IntCellValue local = (Vector2IntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindVector3IntCell(VisualElement cell, int row)
        {
            Vector3IntField field = (Vector3IntField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            Vector3IntCellValue cellValue =
                new Vector3IntCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector3IntCellValue local = (Vector3IntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindQuaternionCell(VisualElement cell, int row)
        {
            Vector4Field field = (Vector4Field)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            QuaternionCellValue cellValue =
                new QuaternionCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            // Figure it out
            Quaternion localQuaternion = cellValue.GetUnsafe();
            Vector4 fieldValue =
                new Vector4(localQuaternion.x, localQuaternion.y, localQuaternion.z, localQuaternion.w);

            field.SetValueWithoutNotify(fieldValue);
            field.RegisterValueChangedCallback(e =>
            {
                QuaternionCellValue local = (QuaternionCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindRectCell(VisualElement cell, int row)
        {
            RectField field = (RectField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            RectCellValue cellValue =
                new RectCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                RectCellValue local = (RectCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindRectIntCell(VisualElement cell, int row)
        {
            RectIntField field = (RectIntField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            RectIntCellValue cellValue =
                new RectIntCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                RectIntCellValue local = (RectIntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindColorCell(VisualElement cell, int row)
        {
            ColorField field = (ColorField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            ColorCellValue cellValue =
                new ColorCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ColorCellValue local = (ColorCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindLayerMaskCell(VisualElement cell, int row)
        {
            LayerMaskField field = (LayerMaskField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            LayerMaskCellValue cellValue =
                new LayerMaskCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                LayerMaskCellValue local = (LayerMaskCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindBoundsCell(VisualElement cell, int row)
        {
            BoundsField field = (BoundsField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            BoundsCellValue cellValue =
                new BoundsCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                BoundsCellValue local = (BoundsCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindBoundsIntCell(VisualElement cell, int row)
        {
            BoundsIntField field = (BoundsIntField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            BoundsIntCellValue cellValue =
                new BoundsIntCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                BoundsIntCellValue local = (BoundsIntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindHash128Cell(VisualElement cell, int row)
        {
            Hash128Field field = (Hash128Field)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            Hash128CellValue cellValue =
                new Hash128CellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Hash128CellValue local = (Hash128CellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindGradientCell(VisualElement cell, int row)
        {
            GradientField field = (GradientField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            GradientCellValue cellValue =
                new GradientCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                GradientCellValue local = (GradientCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindAnimationCurveCell(VisualElement cell, int row)
        {
            CurveField field = (CurveField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            AnimationCurveCellValue cellValue =
                new AnimationCurveCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                AnimationCurveCellValue local = (AnimationCurveCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static void BindObjectCell(VisualElement cell, int row)
        {
            ObjectField field = (ObjectField)cell;
            TableWindowCellMapping map = k_CellToMapping[cell];

            ObjectCellValue cellValue =
                new ObjectCellValue(TableWindowProvider.GetTable(map.KnownTableIndex),
                    TableWindowProvider.GetTableWindow(map.KnownTableIndex).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ObjectCellValue local = (ObjectCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        internal static VisualElement MakeStringCell(int table, int column)
        {
            TextField newField = new TextField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeCharCell(int table, int column)
        {
            TextField newField = new TextField(null, 1, false, false, ' ') { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeBoolCell(int table, int column)
        {
            Toggle newField = new Toggle(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeSByteCell(int table, int column)
        {
            SliderInt newField = new SliderInt(sbyte.MinValue, sbyte.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeByteCell(int table, int column)
        {
            SliderInt newField = new SliderInt(byte.MinValue, byte.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeShortCell(int table, int column)
        {
            SliderInt newField = new SliderInt(short.MinValue, short.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeUShortCell(int table, int column)
        {
            SliderInt newField = new SliderInt(ushort.MinValue, ushort.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeIntCell(int table, int column)
        {
            IntegerField newField = new IntegerField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeUIntCell(int table, int column)
        {
            //TODO: Crunches
            IntegerField newField = new IntegerField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeLongCell(int table, int column)
        {
            LongField newField = new LongField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeULongCell(int table, int column)
        {
            //TODO: Crunches
            LongField newField = new LongField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeFloatCell(int table, int column)
        {
            FloatField newField = new FloatField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeDoubleCell(int table, int column)
        {
            DoubleField newField = new DoubleField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeVector2Cell(int table, int column)
        {
            Vector2Field newField = new Vector2Field(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeVector3Cell(int table, int column)
        {
            Vector3Field newField = new Vector3Field(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeVector4Cell(int table, int column)
        {
            Vector4Field newField = new Vector4Field(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeVector2IntCell(int table, int column)
        {
            Vector2IntField newField = new Vector2IntField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeVector3IntCell(int table, int column)
        {
            Vector3IntField newField = new Vector3IntField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeQuaternionCell(int table, int column)
        {
            Vector4Field newField = new Vector4Field(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeRectCell(int table, int column)
        {
            RectField newField = new RectField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeRectIntCell(int table, int column)
        {
            RectIntField newField = new RectIntField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeColorCell(int table, int column)
        {
            ColorField newField = new ColorField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeLayerMaskCell(int table, int column)
        {
            LayerMaskField newField = new LayerMaskField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeBoundsCell(int table, int column)
        {
            BoundsField newField = new BoundsField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeBoundsIntCell(int table, int column)
        {
            BoundsIntField newField = new BoundsIntField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeHash128Cell(int table, int column)
        {
            Hash128Field newField = new Hash128Field(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeGradientCell(int table, int column)
        {
            GradientField newField = new GradientField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeAnimationCurveCell(int table, int column)
        {
            CurveField newField = new CurveField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeObjectCell(int table, int column)
        {
            ObjectField newField = new ObjectField(null) { name = k_CellFieldName };
            k_CellToMapping.Add(newField, new TableWindowCellMapping { KnownTableIndex = table, ColumnID = column });
            return newField;
        }

        internal static VisualElement MakeRowHeader()
        {
            return new Label { name = k_RowHeaderFieldName };
        }

        struct TableWindowCellMapping
        {
            public int KnownTableIndex;
            public int ColumnID;
        }
    }
#endif
}