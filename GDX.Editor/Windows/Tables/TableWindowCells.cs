// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables;
using GDX.Tables.CellValues;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    static class TableWindowCells
    {
        internal const float k_DoubleHeight = 50f;

        const string k_RowHeaderFieldName = "gdx-table-row-header";
        const string k_CellFieldName = "gdx-table-field";

        static readonly StyleLength m_StyleLength25 = new StyleLength(new Length(25, LengthUnit.Pixel));

        internal static void BindStringCell(VisualElement cell, int row)
        {
            TextField field = (TextField)cell;
            CellData map = (CellData)field.userData;

            StringCellValue cellValue =
                new StringCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                StringCellValue local = (StringCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindCharCell(VisualElement cell, int row)
        {
            TextField field = (TextField)cell;
            CellData map = (CellData)field.userData;

            CharCellValue cellValue =
                new CharCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe().ToString());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                CharCellValue local = (CharCellValue)localMap.CellValue;
                local.Set(e.newValue[0]);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindBoolCell(VisualElement cell, int row)
        {
            Toggle field = (Toggle)cell;
            CellData map = (CellData)field.userData;

            BoolCellValue cellValue =
                new BoolCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                BoolCellValue local = (BoolCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindSByteCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            CellData map = (CellData)field.userData;

            SByteCellValue cellValue =
                new SByteCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                SByteCellValue local = (SByteCellValue)localMap.CellValue;
                local.Set((sbyte)e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindByteCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            CellData map = (CellData)field.userData;

            ByteCellValue cellValue =
                new ByteCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ByteCellValue local = (ByteCellValue)localMap.CellValue;
                local.Set((byte)e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindShortCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            CellData map = (CellData)field.userData;

            ShortCellValue cellValue =
                new ShortCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ShortCellValue local = (ShortCellValue)localMap.CellValue;
                local.Set((short)e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindUShortCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            CellData map = (CellData)field.userData;

            UShortCellValue cellValue =
                new UShortCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                UShortCellValue local = (UShortCellValue)localMap.CellValue;
                local.Set((ushort)e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindIntCell(VisualElement cell, int row)
        {
            IntegerField field = (IntegerField)cell;
            CellData map = (CellData)field.userData;

            IntCellValue cellValue =
                new IntCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                IntCellValue local = (IntCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindUIntCell(VisualElement cell, int row)
        {
            IntegerField field = (IntegerField)cell;
            CellData map = (CellData)field.userData;

            UIntCellValue cellValue =
                new UIntCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify((int)cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                UIntCellValue local = (UIntCellValue)localMap.CellValue;
                local.Set((uint)e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindLongCell(VisualElement cell, int row)
        {
            LongField field = (LongField)cell;
            CellData map = (CellData)field.userData;

            LongCellValue cellValue =
                new LongCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                LongCellValue local = (LongCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindULongCell(VisualElement cell, int row)
        {
            LongField field = (LongField)cell;
            CellData map = (CellData)field.userData;

            ULongCellValue cellValue =
                new ULongCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify((long)cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ULongCellValue local = (ULongCellValue)localMap.CellValue;
                local.Set((ulong)e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindFloatCell(VisualElement cell, int row)
        {
            FloatField field = (FloatField)cell;
            CellData map = (CellData)field.userData;

            FloatCellValue cellValue =
                new FloatCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                FloatCellValue local = (FloatCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindDoubleCell(VisualElement cell, int row)
        {
            DoubleField field = (DoubleField)cell;
            CellData map = (CellData)field.userData;

            DoubleCellValue cellValue =
                new DoubleCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                DoubleCellValue local = (DoubleCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindVector2Cell(VisualElement cell, int row)
        {
            Vector2Field field = (Vector2Field)cell;
            CellData map = (CellData)field.userData;

            Vector2CellValue cellValue =
                new Vector2CellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector2CellValue local = (Vector2CellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindVector3Cell(VisualElement cell, int row)
        {
            Vector3Field field = (Vector3Field)cell;
            CellData map = (CellData)field.userData;

            Vector3CellValue cellValue =
                new Vector3CellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector3CellValue local = (Vector3CellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindVector4Cell(VisualElement cell, int row)
        {
            Vector4Field field = (Vector4Field)cell;
            CellData map = (CellData)field.userData;

            Vector4CellValue cellValue =
                new Vector4CellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector4CellValue local = (Vector4CellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindVector2IntCell(VisualElement cell, int row)
        {
            Vector2IntField field = (Vector2IntField)cell;
            CellData map = (CellData)field.userData;

            Vector2IntCellValue cellValue =
                new Vector2IntCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector2IntCellValue local = (Vector2IntCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindVector3IntCell(VisualElement cell, int row)
        {
            Vector3IntField field = (Vector3IntField)cell;
            CellData map = (CellData)field.userData;

            Vector3IntCellValue cellValue =
                new Vector3IntCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector3IntCellValue local = (Vector3IntCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindQuaternionCell(VisualElement cell, int row)
        {
            Vector4Field field = (Vector4Field)cell;
            CellData map = (CellData)field.userData;

            QuaternionCellValue cellValue =
                new QuaternionCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            // Figure it out
            Quaternion localQuaternion = cellValue.GetUnsafe();
            Vector4 fieldValue =
                new Vector4(localQuaternion.x, localQuaternion.y, localQuaternion.z, localQuaternion.w);

            field.SetValueWithoutNotify(fieldValue);
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                QuaternionCellValue local = (QuaternionCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindRectCell(VisualElement cell, int row)
        {
            RectField field = (RectField)cell;
            CellData map = (CellData)field.userData;

            RectCellValue cellValue =
                new RectCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                RectCellValue local = (RectCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindRectIntCell(VisualElement cell, int row)
        {
            RectIntField field = (RectIntField)cell;
            CellData map = (CellData)field.userData;

            RectIntCellValue cellValue =
                new RectIntCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                RectIntCellValue local = (RectIntCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindColorCell(VisualElement cell, int row)
        {
            ColorField field = (ColorField)cell;
            CellData map = (CellData)field.userData;

            ColorCellValue cellValue =
                new ColorCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ColorCellValue local = (ColorCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindLayerMaskCell(VisualElement cell, int row)
        {
            LayerMaskField field = (LayerMaskField)cell;
            CellData map = (CellData)field.userData;

            LayerMaskCellValue cellValue =
                new LayerMaskCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                LayerMaskCellValue local = (LayerMaskCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindBoundsCell(VisualElement cell, int row)
        {
            BoundsField field = (BoundsField)cell;
            CellData map = (CellData)field.userData;

            BoundsCellValue cellValue =
                new BoundsCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                BoundsCellValue local = (BoundsCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindBoundsIntCell(VisualElement cell, int row)
        {
            BoundsIntField field = (BoundsIntField)cell;
            CellData map = (CellData)field.userData;

            BoundsIntCellValue cellValue =
                new BoundsIntCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                BoundsIntCellValue local = (BoundsIntCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindHash128Cell(VisualElement cell, int row)
        {
            Hash128Field field = (Hash128Field)cell;
            CellData map = (CellData)field.userData;

            Hash128CellValue cellValue =
                new Hash128CellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Hash128CellValue local = (Hash128CellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindGradientCell(VisualElement cell, int row)
        {
            GradientField field = (GradientField)cell;
            CellData map = (CellData)field.userData;

            GradientCellValue cellValue =
                new GradientCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                GradientCellValue local = (GradientCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindAnimationCurveCell(VisualElement cell, int row)
        {
            CurveField field = (CurveField)cell;
            CellData map = (CellData)field.userData;

            AnimationCurveCellValue cellValue =
                new AnimationCurveCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                AnimationCurveCellValue local = (AnimationCurveCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static void BindObjectCell(VisualElement cell, int row)
        {
            ObjectField field = (ObjectField)cell;
            CellData map = (CellData)field.userData;

            ObjectCellValue cellValue =
                new ObjectCellValue(TableCache.GetTable(map.TableTicket),
                    TableWindowProvider.GetTableWindow(map.TableTicket).GetView().GetRowDescriptionIndex(row),
                    map.ColumnID);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ObjectCellValue local = (ObjectCellValue)localMap.CellValue;
                local.Set(e.newValue);
                TableCache.NotifyOfCellValueChange(local.Table, local.Row, local.Column,
                    TableWindowProvider.GetTableWindow(localMap.TableTicket));
            });
        }

        internal static VisualElement MakeStringCell(int table, int column)
        {
            TextField newField =
                new TextField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeCharCell(int table, int column)
        {
            TextField newField = new TextField(null, 1, false, false, ' ')
            {
                name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}"
            };
            newField.style.maxWidth = m_StyleLength25;
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeBoolCell(int table, int column)
        {
            Toggle newField = new Toggle(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeSByteCell(int table, int column)
        {
            SliderInt newField = new SliderInt(sbyte.MinValue, sbyte.MaxValue)
            {
                name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}",
                showInputField = true,
                label = null
            };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeByteCell(int table, int column)
        {
            SliderInt newField = new SliderInt(byte.MinValue, byte.MaxValue)
            {
                name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}",
                showInputField = true,
                label = null
            };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeShortCell(int table, int column)
        {
            SliderInt newField = new SliderInt(short.MinValue, short.MaxValue)
            {
                name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}",
                showInputField = true,
                label = null
            };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeUShortCell(int table, int column)
        {
            SliderInt newField = new SliderInt(ushort.MinValue, ushort.MaxValue)
            {
                name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}",
                showInputField = true,
                label = null
            };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeIntCell(int table, int column)
        {
            IntegerField newField =
                new IntegerField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeUIntCell(int table, int column)
        {
            //TODO: Crunches
            IntegerField newField =
                new IntegerField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeLongCell(int table, int column)
        {
            LongField newField =
                new LongField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeULongCell(int table, int column)
        {
            //TODO: Crunches
            LongField newField =
                new LongField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeFloatCell(int table, int column)
        {
            FloatField newField =
                new FloatField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeDoubleCell(int table, int column)
        {
            DoubleField newField =
                new DoubleField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeVector2Cell(int table, int column)
        {
            Vector2Field newField =
                new Vector2Field(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeVector3Cell(int table, int column)
        {
            Vector3Field newField =
                new Vector3Field(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeVector4Cell(int table, int column)
        {
            Vector4Field newField =
                new Vector4Field(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeVector2IntCell(int table, int column)
        {
            Vector2IntField newField =
                new Vector2IntField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeVector3IntCell(int table, int column)
        {
            Vector3IntField newField =
                new Vector3IntField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeQuaternionCell(int table, int column)
        {
            Vector4Field newField =
                new Vector4Field(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeRectCell(int table, int column)
        {
            RectField newField =
                new RectField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeRectIntCell(int table, int column)
        {
            RectIntField newField =
                new RectIntField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeColorCell(int table, int column)
        {
            ColorField newField =
                new ColorField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeLayerMaskCell(int table, int column)
        {
            LayerMaskField newField =
                new LayerMaskField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeBoundsCell(int table, int column)
        {
            BoundsField newField =
                new BoundsField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeBoundsIntCell(int table, int column)
        {
            BoundsIntField newField =
                new BoundsIntField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeHash128Cell(int table, int column)
        {
            Hash128Field newField =
                new Hash128Field(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeGradientCell(int table, int column)
        {
            GradientField newField =
                new GradientField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeAnimationCurveCell(int table, int column)
        {
            CurveField newField =
                new CurveField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeObjectCell(int table, int column)
        {
            ObjectField newField =
                new ObjectField(null) { name = $"{k_CellFieldName}_{table.ToString()}_{column.ToString()}" };
            newField.userData = new CellData { TableTicket = table, ColumnID = column };
            return newField;
        }

        internal static VisualElement MakeRowHeader()
        {
            return new Label { name = k_RowHeaderFieldName };
        }

        class CellData
        {
            public object CellValue;
            public int ColumnID;
            public int TableTicket;
        }
    }
#endif
}