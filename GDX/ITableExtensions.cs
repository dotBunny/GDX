// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using GDX.Tables;
using GDX.Tables.CellValues;

namespace GDX
{
    public static class ITableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringCellValue GetStringCellValue(this TableBase table, int rowID, int columnID)
        {
            return new StringCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoolCellValue GetBoolCellValue(this TableBase table,int rowID, int columnID)
        {
            return new BoolCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CharCellValue GetCharCellValue(this TableBase table,int rowID, int columnID)
        {
            return new CharCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByteCellValue GetSByteCellValue(this TableBase table,int rowID, int columnID)
        {
            return new SByteCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByteCellValue GetByteCellValue(this TableBase table,int rowID, int columnID)
        {
            return new ByteCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ShortCellValue GetShortCellValue(this TableBase table,int rowID, int columnID)
        {
            return new ShortCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UShortCellValue GetUShortCellValue(this TableBase table,int rowID, int columnID)
        {
            return new UShortCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntCellValue GetIntCellValue(this TableBase table,int rowID, int columnID)
        {
            return new IntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UIntCellValue GetUIntCellValue(this TableBase table,int rowID, int columnID)
        {
            return new UIntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LongCellValue GetLongCellValue(this TableBase table,int rowID, int columnID)
        {
            return new LongCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ULongCellValue GetULongCellValue(this TableBase table,int rowID, int columnID)
        {
            return new ULongCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatCellValue GetFloatCellValue(this TableBase table,int rowID, int columnID)
        {
            return new FloatCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleCellValue GetDoubleCellValue(this TableBase table,int rowID, int columnID)
        {
            return new DoubleCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2CellValue GetVector2CellValue(this TableBase table,int rowID, int columnID)
        {
            return new Vector2CellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3CellValue GetVector3CellValue(this TableBase table,int rowID, int columnID)
        {
            return new Vector3CellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4CellValue GetVector4CellValue(this TableBase table,int rowID, int columnID)
        {
            return new Vector4CellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2IntCellValue GetVector2IntCellValue(this TableBase table,int rowID, int columnID)
        {
            return new Vector2IntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3IntCellValue GetVector3IntCellValue(this TableBase table,int rowID, int columnID)
        {
            return new Vector3IntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QuaternionCellValue GetQuaternionCellValue(this TableBase table,int rowID, int columnID)
        {
            return new QuaternionCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectCellValue GetRectCellValue(this TableBase table,int rowID, int columnID)
        {
            return new RectCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectIntCellValue GetRectIntCellValue(this TableBase table,int rowID, int columnID)
        {
            return new RectIntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColorCellValue GetColorCellValue(this TableBase table,int rowID, int columnID)
        {
            return new ColorCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMaskCellValue GetLayerMaskCellValue(this TableBase table,int rowID, int columnID)
        {
            return new LayerMaskCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsCellValue GetBoundsCellValue(this TableBase table,int rowID, int columnID)
        {
            return new BoundsCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsIntCellValue GetBoundsIntCellValue(this TableBase table,int rowID, int columnID)
        {
            return new BoundsIntCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Hash128CellValue GetHash128CellValue(this TableBase table,int rowID, int columnID)
        {
            return new Hash128CellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GradientCellValue GetGradientCellValue(this TableBase table,int rowID, int columnID)
        {
            return new GradientCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnimationCurveCellValue GetAnimationCurveCellValue(this TableBase table,int rowID, int columnID)
        {
            return new AnimationCurveCellValue(table, rowID, columnID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectCellValue GetObjectCellValue(this TableBase table,int rowID, int columnID)
        {
            return new ObjectCellValue(table, rowID, columnID);
        }
    }
}