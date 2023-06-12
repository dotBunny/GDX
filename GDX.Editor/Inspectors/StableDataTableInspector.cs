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

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            asset.SetFlag(DataTableBase.Settings.EnableUndo, true);

            asset.AddColumn(Serializable.SerializableTypes.String, "String");
            asset.AddRow("0");
            asset.AddColumn(Serializable.SerializableTypes.Char, "Char");
            asset.AddRow("1");
            asset.AddColumn(Serializable.SerializableTypes.Bool, "Bool");
            asset.AddRow("2");
            asset.AddColumn(Serializable.SerializableTypes.SByte, "SByte");
            asset.AddRow("3");
            asset.AddColumn(Serializable.SerializableTypes.Byte, "Byte");
            asset.AddRow("4");
            asset.AddColumn(Serializable.SerializableTypes.Short, "Short");
            asset.AddRow("5");
            asset.AddColumn(Serializable.SerializableTypes.UShort, "UShort");
            asset.AddRow("6");
            asset.AddColumn(Serializable.SerializableTypes.Int, "Int");
            asset.AddRow("7");
            asset.AddColumn(Serializable.SerializableTypes.UInt, "UInt");
            asset.AddRow("8");
            asset.AddColumn(Serializable.SerializableTypes.Long, "Long");
            asset.AddRow("9");
            asset.AddColumn(Serializable.SerializableTypes.ULong, "ULong");
            asset.AddRow("10");
            asset.AddColumn(Serializable.SerializableTypes.Float, "Float");
            asset.AddRow("11");
            asset.AddColumn(Serializable.SerializableTypes.Double, "Double");
            asset.AddRow("12");
            asset.AddColumn(Serializable.SerializableTypes.Vector2, "Vector2");
            asset.AddRow("13");
            asset.AddColumn(Serializable.SerializableTypes.Vector3, "Vector3");
            asset.AddRow("14");
            asset.AddColumn(Serializable.SerializableTypes.Vector4, "Vector4");
            asset.AddRow("15");
            asset.AddColumn(Serializable.SerializableTypes.Vector2Int, "Vector2Int");
            asset.AddRow("16");
            asset.AddColumn(Serializable.SerializableTypes.Vector3Int, "Vector3Int");
            asset.AddRow("17");
            asset.AddColumn(Serializable.SerializableTypes.Quaternion, "Quaternion");
            asset.AddRow("18");
            asset.AddColumn(Serializable.SerializableTypes.Rect, "Rect");
            asset.AddRow("19");
            asset.AddColumn(Serializable.SerializableTypes.RectInt, "RectInt");
            asset.AddRow("20");
            asset.AddColumn(Serializable.SerializableTypes.Color, "Color");
            asset.AddRow("21");
            asset.AddColumn(Serializable.SerializableTypes.LayerMask, "LayerMask");
            asset.AddRow("22");
            asset.AddColumn(Serializable.SerializableTypes.Bounds, "Bounds");
            asset.AddRow("23");
            asset.AddColumn(Serializable.SerializableTypes.BoundsInt, "BoundsInt");
            asset.AddRow("24");
            asset.AddColumn(Serializable.SerializableTypes.Hash128, "Hash128");
            asset.AddRow("25");
            asset.AddColumn(Serializable.SerializableTypes.Gradient, "Gradient");
            asset.AddRow("26");
            asset.AddColumn(Serializable.SerializableTypes.AnimationCurve, "AnimationCurve");
            asset.AddRow("27");
            asset.AddColumn(Serializable.SerializableTypes.Object, "Object");
            asset.AddRow("28");
            int enumID = asset.AddColumn(Serializable.SerializableTypes.EnumInt, "EnumInt");
            asset.SetTypeNameForColumn(enumID, Reflection.SerializedTypesName);
            asset.AddRow("29");

            DataTableWindowProvider.OpenAsset(asset);
        }
#endif // UNITY_2022_2_OR_NEWER
    }
}