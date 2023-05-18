// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using GDX.Tables;
using GDX.Tables.CellValues;

namespace GDX
{
    public static class SimpleTableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringCellValue GetStringCellValue(this SimpleTable table, int row, int columnID)
        {
            return new StringCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoolCellValue GetBoolCellValue(this SimpleTable table,int row, int columnID)
        {
            return new BoolCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CharCellValue GetCharCellValue(this SimpleTable table,int row, int columnID)
        {
            return new CharCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByteCellValue GetSByteCellValue(this SimpleTable table,int row, int columnID)
        {
            return new SByteCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByteCellValue GetByteCellValue(this SimpleTable table,int row, int columnID)
        {
            return new ByteCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ShortCellValue GetShortCellValue(this SimpleTable table,int row, int columnID)
        {
            return new ShortCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UShortCellValue GetUShortCellValue(this SimpleTable table,int row, int columnID)
        {
            return new UShortCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntCellValue GetIntCellValue(this SimpleTable table,int row, int columnID)
        {
            return new IntCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UIntCellValue GetUIntCellValue(this SimpleTable table,int row, int columnID)
        {
            return new UIntCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LongCellValue GetLongCellValue(this SimpleTable table,int row, int columnID)
        {
            return new LongCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ULongCellValue GetULongCellValue(this SimpleTable table,int row, int columnID)
        {
            return new ULongCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatCellValue GetFloatCellValue(this SimpleTable table,int row, int columnID)
        {
            return new FloatCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleCellValue GetDoubleCellValue(this SimpleTable table,int row, int columnID)
        {
            return new DoubleCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2CellValue GetVector2CellValue(this SimpleTable table,int row, int columnID)
        {
            return new Vector2CellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3CellValue GetVector3CellValue(this SimpleTable table,int row, int columnID)
        {
            return new Vector3CellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4CellValue GetVector4CellValue(this SimpleTable table,int row, int columnID)
        {
            return new Vector4CellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2IntCellValue GetVector2IntCellValue(this SimpleTable table,int row, int columnID)
        {
            return new Vector2IntCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3IntCellValue GetVector3IntCellValue(this SimpleTable table,int row, int columnID)
        {
            return new Vector3IntCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QuaternionCellValue GetQuaternionCellValue(this SimpleTable table,int row, int columnID)
        {
            return new QuaternionCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectCellValue GetRectCellValue(this SimpleTable table,int row, int columnID)
        {
            return new RectCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectIntCellValue GetRectIntCellValue(this SimpleTable table,int row, int columnID)
        {
            return new RectIntCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColorCellValue GetColorCellValue(this SimpleTable table,int row, int columnID)
        {
            return new ColorCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMaskCellValue GetLayerMaskCellValue(this SimpleTable table,int row, int columnID)
        {
            return new LayerMaskCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsCellValue GetBoundsCellValue(this SimpleTable table,int row, int columnID)
        {
            return new BoundsCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsIntCellValue GetBoundsIntCellValue(this SimpleTable table,int row, int columnID)
        {
            return new BoundsIntCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Hash128CellValue GetHash128CellValue(this SimpleTable table,int row, int columnID)
        {
            return new Hash128CellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GradientCellValue GetGradientCellValue(this SimpleTable table,int row, int columnID)
        {
            return new GradientCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnimationCurveCellValue GetAnimationCurveCellValue(this SimpleTable table,int row, int columnID)
        {
            return new AnimationCurveCellValue(table, row, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectCellValue GetObjectCellValue(this SimpleTable table,int row, int columnID)
        {
            return new ObjectCellValue(table, row, columnID);
        }
    }
}