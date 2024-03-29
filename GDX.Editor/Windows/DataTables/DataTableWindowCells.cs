﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.DataTables;
using GDX.DataTables.CellValues;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    static class DataTableWindowCells
    {
        internal const float k_DoubleHeight = 50f;
        const string k_RowHeaderFieldName = "gdx-table-row-header";
        const string k_CellFieldName = "gdx-table-field";

        static readonly StyleLength k_StyleLength25 = new StyleLength(new Length(25, LengthUnit.Pixel));

        internal static void BindStringCell(VisualElement cell, int row)
        {
            TextField field = (TextField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);
            StringCellValue cellValue =
                new StringCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                StringCellValue local = (StringCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindCharCell(VisualElement cell, int row)
        {
            TextField field = (TextField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            CharCellValue cellValue =
                new CharCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe().ToString());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                CharCellValue local = (CharCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue[0]);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindBoolCell(VisualElement cell, int row)
        {
            Toggle field = (Toggle)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            BoolCellValue cellValue =
                new BoolCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                BoolCellValue local = (BoolCellValue)localMap.CellValue;
                local.Set(e.newValue);
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindSByteCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            SByteCellValue cellValue =
                new SByteCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                SByteCellValue local = (SByteCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set((sbyte)e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindByteCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            ByteCellValue cellValue =
                new ByteCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ByteCellValue local = (ByteCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set((byte)e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindShortCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            ShortCellValue cellValue =
                new ShortCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ShortCellValue local = (ShortCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set((short)e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindUShortCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            UShortCellValue cellValue =
                new UShortCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                UShortCellValue local = (UShortCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set((ushort)e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindIntCell(VisualElement cell, int row)
        {
            IntegerField field = (IntegerField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            IntCellValue cellValue =
                new IntCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                IntCellValue local = (IntCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindUIntCell(VisualElement cell, int row)
        {
#if UNITY_2022_3_OR_NEWER
            UnsignedIntegerField field = (UnsignedIntegerField)cell;
#else
            IntegerField field = (IntegerField)cell;
#endif
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            UIntCellValue cellValue =
                new UIntCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
#if UNITY_2022_3_OR_NEWER
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
#else
            field.SetValueWithoutNotify((int)cellValue.GetUnsafe());
#endif
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                UIntCellValue local = (UIntCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
#if UNITY_2022_3_OR_NEWER
                local.Set(e.newValue);
#else
                local.Set((uint)e.newValue);
#endif
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindLongCell(VisualElement cell, int row)
        {
            LongField field = (LongField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            LongCellValue cellValue =
                new LongCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                LongCellValue local = (LongCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindULongCell(VisualElement cell, int row)
        {
#if UNITY_2022_3_OR_NEWER
            UnsignedLongField field = (UnsignedLongField)cell;
#else
            LongField field = (LongField)cell;
#endif
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            ULongCellValue cellValue =
                new ULongCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;

#if UNITY_2022_3_OR_NEWER
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
#else
            field.SetValueWithoutNotify((long)cellValue.GetUnsafe());
#endif
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ULongCellValue local = (ULongCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
#if UNITY_2022_3_OR_NEWER
                local.Set(e.newValue);
#else
                local.Set((ulong)e.newValue);
#endif
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindFloatCell(VisualElement cell, int row)
        {
            FloatField field = (FloatField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            FloatCellValue cellValue =
                new FloatCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                FloatCellValue local = (FloatCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindDoubleCell(VisualElement cell, int row)
        {
            DoubleField field = (DoubleField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            DoubleCellValue cellValue =
                new DoubleCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                DoubleCellValue local = (DoubleCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindVector2Cell(VisualElement cell, int row)
        {
            Vector2Field field = (Vector2Field)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            Vector2CellValue cellValue =
                new Vector2CellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector2CellValue local = (Vector2CellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindVector3Cell(VisualElement cell, int row)
        {
            Vector3Field field = (Vector3Field)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            Vector3CellValue cellValue =
                new Vector3CellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector3CellValue local = (Vector3CellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindVector4Cell(VisualElement cell, int row)
        {
            Vector4Field field = (Vector4Field)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            Vector4CellValue cellValue =
                new Vector4CellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector4CellValue local = (Vector4CellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindVector2IntCell(VisualElement cell, int row)
        {
            Vector2IntField field = (Vector2IntField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            Vector2IntCellValue cellValue =
                new Vector2IntCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector2IntCellValue local = (Vector2IntCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindVector3IntCell(VisualElement cell, int row)
        {
            Vector3IntField field = (Vector3IntField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            Vector3IntCellValue cellValue =
                new Vector3IntCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Vector3IntCellValue local = (Vector3IntCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindQuaternionCell(VisualElement cell, int row)
        {
            Vector4Field field = (Vector4Field)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            QuaternionCellValue cellValue =
                new QuaternionCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

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
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindRectCell(VisualElement cell, int row)
        {
            RectField field = (RectField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            RectCellValue cellValue =
                new RectCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                RectCellValue local = (RectCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindRectIntCell(VisualElement cell, int row)
        {
            RectIntField field = (RectIntField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            RectIntCellValue cellValue =
                new RectIntCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                RectIntCellValue local = (RectIntCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindColorCell(VisualElement cell, int row)
        {
            ColorField field = (ColorField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            ColorCellValue cellValue =
                new ColorCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ColorCellValue local = (ColorCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindLayerMaskCell(VisualElement cell, int row)
        {
            LayerMaskField field = (LayerMaskField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            LayerMaskCellValue cellValue =
                new LayerMaskCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                LayerMaskCellValue local = (LayerMaskCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindBoundsCell(VisualElement cell, int row)
        {
            BoundsField field = (BoundsField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            BoundsCellValue cellValue =
                new BoundsCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                BoundsCellValue local = (BoundsCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindBoundsIntCell(VisualElement cell, int row)
        {
            BoundsIntField field = (BoundsIntField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            BoundsIntCellValue cellValue =
                new BoundsIntCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                BoundsIntCellValue local = (BoundsIntCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            field.SetEnabled(!dataTable.GetMetaData().ReferencesOnlyMode);
        }

        internal static void BindHash128Cell(VisualElement cell, int row)
        {
            Hash128Field field = (Hash128Field)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            Hash128CellValue cellValue =
                new Hash128CellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                Hash128CellValue local = (Hash128CellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
        }

        internal static void BindGradientCell(VisualElement cell, int row)
        {
            GradientField field = (GradientField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            GradientCellValue cellValue =
                new GradientCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                GradientCellValue local = (GradientCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
        }

        internal static void BindAnimationCurveCell(VisualElement cell, int row)
        {
            CurveField field = (CurveField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            AnimationCurveCellValue cellValue =
                new AnimationCurveCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                AnimationCurveCellValue local = (AnimationCurveCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
        }

        internal static void BindObjectCell(VisualElement cell, int row)
        {
            ObjectField field = (ObjectField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            ObjectCellValue cellValue =
                new ObjectCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            string qualifiedType = dataTable.GetTypeNameForColumn(map.ColumnIdentifier);
            field.objectType = (!string.IsNullOrEmpty(qualifiedType) ? Type.GetType(qualifiedType) : typeof(Object)) ??
                               typeof(Object);

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                ObjectCellValue local = (ObjectCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set(e.newValue);
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
        }

        internal static void BindEnumIntCell(VisualElement cell, int row)
        {
            EnumField field = (EnumField)cell;
            CellData map = (CellData)field.userData;
            DataTableBase dataTable = DataTableTracker.GetTable(map.TableTicket);

            EnumIntCellValue cellValue =
                new EnumIntCellValue(dataTable, map.View.GetRowDescriptionBySortedOrder(row).Identifier,
                    map.ColumnIdentifier);

            field.Init(cellValue.GetEnum());

            map.CellValue = cellValue;
            field.SetValueWithoutNotify(cellValue.GetEnum());

            field.RegisterValueChangedCallback(e =>
            {
                CellData localMap = (CellData)field.userData;
                EnumIntCellValue local = (EnumIntCellValue)localMap.CellValue;
                DataTableWindow window = DataTableWindowProvider.GetTableWindow(localMap.TableTicket);
                DataTableTracker.RecordCellValueUndo(localMap.TableTicket, local.RowIdentifier, local.ColumnIdentifier);
                local.Set((int)(object)e.newValue);
                ;
                DataTableTracker.NotifyOfCellValueChange(localMap.TableTicket, local.RowIdentifier,
                    local.ColumnIdentifier,
                    window);
            });
            //field.SetEnabled(!dataTable.GetMeta().ReferencesOnlyMode);
        }

        internal static VisualElement MakeStringCell(DataTableWindowView view, int table, int column)
        {
            TextField newField =
                new TextField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeEnumIntCell(DataTableWindowView view, int table, int column)
        {
            EnumField newField = new EnumField
            {
                name = k_CellFieldName,
                label = null,
                userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
            };
            return newField;
        }

        internal static VisualElement MakeCharCell(DataTableWindowView view, int table, int column)
        {
            TextField newField = new TextField(null, 1, false, false, ' ')
            {
                name = k_CellFieldName,
                style = { maxWidth = k_StyleLength25 },
                userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
            };
            return newField;
        }

        internal static VisualElement MakeBoolCell(DataTableWindowView view, int table, int column)
        {
            Toggle newField = new Toggle(null)
            {
                name = k_CellFieldName,
                userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
            };
            return newField;
        }

        internal static VisualElement MakeSByteCell(DataTableWindowView view, int table, int column)
        {
            SliderInt newField = new SliderInt(sbyte.MinValue, sbyte.MaxValue)
            {
                name = k_CellFieldName,
                showInputField = true,
                label = null,
                userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
            };
            return newField;
        }

        internal static VisualElement MakeByteCell(DataTableWindowView view, int table, int column)
        {
            SliderInt newField = new SliderInt(byte.MinValue, byte.MaxValue)
            {
                name = k_CellFieldName,
                showInputField = true,
                label = null,
                userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
            };
            return newField;
        }

        internal static VisualElement MakeShortCell(DataTableWindowView view, int table, int column)
        {
            SliderInt newField = new SliderInt(short.MinValue, short.MaxValue)
            {
                name = k_CellFieldName,
                showInputField = true,
                label = null,
                userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
            };
            return newField;
        }

        internal static VisualElement MakeUShortCell(DataTableWindowView view, int table, int column)
        {
            SliderInt newField = new SliderInt(ushort.MinValue, ushort.MaxValue)
            {
                name = k_CellFieldName,
                showInputField = true,
                label = null,
                userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
            };
            return newField;
        }

        internal static VisualElement MakeIntCell(DataTableWindowView view, int table, int column)
        {
            IntegerField newField =
                new IntegerField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeUIntCell(DataTableWindowView view, int table, int column)
        {
            UnsignedIntegerField newField =
                new UnsignedIntegerField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeLongCell(DataTableWindowView view, int table, int column)
        {
            LongField newField =
                new LongField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeULongCell(DataTableWindowView view, int table, int column)
        {
            UnsignedLongField newField =
                new UnsignedLongField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeFloatCell(DataTableWindowView view, int table, int column)
        {
            FloatField newField =
                new FloatField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeDoubleCell(DataTableWindowView view, int table, int column)
        {
            DoubleField newField =
                new DoubleField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeVector2Cell(DataTableWindowView view, int table, int column)
        {
            Vector2Field newField =
                new Vector2Field(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeVector3Cell(DataTableWindowView view, int table, int column)
        {
            Vector3Field newField =
                new Vector3Field(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeVector4Cell(DataTableWindowView view, int table, int column)
        {
            Vector4Field newField =
                new Vector4Field(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeVector2IntCell(DataTableWindowView view, int table, int column)
        {
            Vector2IntField newField =
                new Vector2IntField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeVector3IntCell(DataTableWindowView view, int table, int column)
        {
            Vector3IntField newField =
                new Vector3IntField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeQuaternionCell(DataTableWindowView view, int table, int column)
        {
            Vector4Field newField =
                new Vector4Field(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeRectCell(DataTableWindowView view, int table, int column)
        {
            RectField newField =
                new RectField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeRectIntCell(DataTableWindowView view, int table, int column)
        {
            RectIntField newField =
                new RectIntField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeColorCell(DataTableWindowView view, int table, int column)
        {
            ColorField newField =
                new ColorField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeLayerMaskCell(DataTableWindowView view, int table, int column)
        {
            LayerMaskField newField =
                new LayerMaskField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeBoundsCell(DataTableWindowView view, int table, int column)
        {
            BoundsField newField =
                new BoundsField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeBoundsIntCell(DataTableWindowView view, int table, int column)
        {
            BoundsIntField newField =
                new BoundsIntField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeHash128Cell(DataTableWindowView view, int table, int column)
        {
            Hash128Field newField =
                new Hash128Field(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeGradientCell(DataTableWindowView view, int table, int column)
        {
            GradientField newField =
                new GradientField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeAnimationCurveCell(DataTableWindowView view, int table, int column)
        {
            CurveField newField =
                new CurveField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeObjectCell(DataTableWindowView view, int table, int column)
        {
            ObjectField newField =
                new ObjectField(null)
                {
                    name = k_CellFieldName,
                    userData = new CellData { TableTicket = table, ColumnIdentifier = column, View = view }
                };
            return newField;
        }

        internal static VisualElement MakeRowHeader()
        {
            return new Label { name = k_RowHeaderFieldName };
        }

        public class CellData
        {
            public object CellValue;
            public int ColumnIdentifier = -1;
            public int TableTicket = -1;
            public DataTableWindowView View;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}