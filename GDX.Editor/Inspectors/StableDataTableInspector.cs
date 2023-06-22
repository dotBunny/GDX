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
            int typeCount = Serializable.SerializableTypesCount;
            for (int i = 0; i < typeCount; i++)
            {
                table.AddColumn((Serializable.SerializableTypes)i,
                    ((Serializable.SerializableTypes)i).GetLabel());
                table.AddRow(i.ToString());
            }
        }
#endif // UNITY_2022_2_OR_NEWER
    }
}