// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables;
using GDX.Editor.Windows.DataTables;
using UnityEditor;

namespace GDX.Editor.Inspectors
{
    /// <summary>
    ///     Custom inspector for <see cref="StableDataTable"/> based on <see cref="DataTableInspectorBase" />.
    /// </summary>
    [CustomEditor(typeof(StableDataTable))]
    public class StableDataTableInspector : DataTableInspectorBase
    {
#if UNITY_2022_2_OR_NEWER
#if GDX_TOOLS
        [MenuItem("Tools/GDX/Create StableTable Example", false)]
#endif // GDX_TOOLS
        public static void CreateStableTableExample()
        {
            StableDataTable asset = CreateInstance<StableDataTable>();
            AssetDatabase.CreateAsset(asset, "Assets/StableTableExample.asset");
            AssetDatabase.SaveAssets();

            BuildExample(asset);

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            DataTableWindowProvider.OpenAsset(asset);
        }

        public static void BuildExample(StableDataTable table)
        {
            table.SetFlag(DataTableBase.Settings.EnableUndo, true);
            table.AddColumn(Serializable.SerializableTypes.String, "String");
            table.AddRow("0");
            table.AddColumn(Serializable.SerializableTypes.Char, "Char");
            table.AddRow("1");
            table.AddColumn(Serializable.SerializableTypes.Bool, "Bool");
            table.AddRow("2");
            table.AddColumn(Serializable.SerializableTypes.SByte, "SByte");
            table.AddRow("3");
            table.AddColumn(Serializable.SerializableTypes.Byte, "Byte");
            table.AddRow("4");
            table.AddColumn(Serializable.SerializableTypes.Short, "Short");
            table.AddRow("5");
            table.AddColumn(Serializable.SerializableTypes.UShort, "UShort");
            table.AddRow("6");
            table.AddColumn(Serializable.SerializableTypes.Int, "Int");
            table.AddRow("7");
            table.AddColumn(Serializable.SerializableTypes.UInt, "UInt");
            table.AddRow("8");
            table.AddColumn(Serializable.SerializableTypes.Long, "Long");
            table.AddRow("9");
            table.AddColumn(Serializable.SerializableTypes.ULong, "ULong");
            table.AddRow("10");
            table.AddColumn(Serializable.SerializableTypes.Float, "Float");
            table.AddRow("11");
            table.AddColumn(Serializable.SerializableTypes.Double, "Double");
            table.AddRow("12");
            table.AddColumn(Serializable.SerializableTypes.Vector2, "Vector2");
            table.AddRow("13");
            table.AddColumn(Serializable.SerializableTypes.Vector3, "Vector3");
            table.AddRow("14");
            table.AddColumn(Serializable.SerializableTypes.Vector4, "Vector4");
            table.AddRow("15");
            table.AddColumn(Serializable.SerializableTypes.Vector2Int, "Vector2Int");
            table.AddRow("16");
            table.AddColumn(Serializable.SerializableTypes.Vector3Int, "Vector3Int");
            table.AddRow("17");
            table.AddColumn(Serializable.SerializableTypes.Quaternion, "Quaternion");
            table.AddRow("18");
            table.AddColumn(Serializable.SerializableTypes.Rect, "Rect");
            table.AddRow("19");
            table.AddColumn(Serializable.SerializableTypes.RectInt, "RectInt");
            table.AddRow("20");
            table.AddColumn(Serializable.SerializableTypes.Color, "Color");
            table.AddRow("21");
            table.AddColumn(Serializable.SerializableTypes.LayerMask, "LayerMask");
            table.AddRow("22");
            table.AddColumn(Serializable.SerializableTypes.Bounds, "Bounds");
            table.AddRow("23");
            table.AddColumn(Serializable.SerializableTypes.BoundsInt, "BoundsInt");
            table.AddRow("24");
            table.AddColumn(Serializable.SerializableTypes.Hash128, "Hash128");
            table.AddRow("25");
            table.AddColumn(Serializable.SerializableTypes.Gradient, "Gradient");
            table.AddRow("26");
            table.AddColumn(Serializable.SerializableTypes.AnimationCurve, "AnimationCurve");
            table.AddRow("27");
            table.AddColumn(Serializable.SerializableTypes.Object, "Object");
            table.AddRow("28");
            int enumID = table.AddColumn(Serializable.SerializableTypes.EnumInt, "EnumInt");
            table.SetTypeNameForColumn(enumID, Reflection.SerializedTypesName);
            table.AddRow("29");
        }
#endif // UNITY_2022_2_OR_NEWER
    }
}