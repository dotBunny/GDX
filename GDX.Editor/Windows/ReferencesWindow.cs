using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows
{
    // TODO: extensions shoudl probably be configurable?
    // Limt to 2022
    // Graph View
    public class ReferencesWindow : EditorWindow
    {
        Toolbar m_Toolbar;
        ListView m_ListView;
        internal ObjectField m_ObjectField;
        Button m_FindButton;

        List<Object> m_LoadedObjects = new List<Object>(0);
        List<string> m_LoadedObjectPaths = new List<string>(0);

        [MenuItem("Tools/GDX/Find References")]
        public static EditorWindow NewWindow()
        {
            EditorWindow newWindow = EditorWindow.CreateWindow<ReferencesWindow>();

            newWindow.Show();
            newWindow.Focus();

            return newWindow;
        }

        [MenuItem("Assets/Find References In Project", false, 2)]
        public static void FindWindow()
        {
            ReferencesWindow newWindow = (ReferencesWindow)EditorWindow.CreateWindow<ReferencesWindow>();
            newWindow.m_ObjectField.value = Selection.activeObject;
            newWindow.Find();
        }

        [MenuItem("Assets/Find References In Project", true, 2)]
        static bool ValidateFindWindow()
        {
            return Selection.activeObject != null;
        }

        void CreateGUI()
        {
            ResourcesProvider.SetupSharedStylesheets(rootVisualElement);
            ResourcesProvider.SetupStylesheet("GDXReferencesWindow", rootVisualElement);
            ResourcesProvider.GetVisualTreeAsset("GDXReferencesWindow").CloneTree(rootVisualElement);
            ResourcesProvider.CheckTheme(rootVisualElement);

            m_Toolbar = rootVisualElement.Q<Toolbar>("gdx-references-toolbar");
            m_ObjectField = m_Toolbar.Q<ObjectField>("gdx-references-toolbar-object");
            m_FindButton = m_Toolbar.Q<Button>("gdx-references-toolbar-find");

            m_ListView = rootVisualElement.Q<ListView>("gdx-references-list");
            m_ListView.fixedItemHeight = 40;
            m_ListView.selectionType = SelectionType.Single;
            m_ListView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            m_ListView.makeItem += MakeItem;
            m_ListView.bindItem += BindItem;
            m_ListView.selectionChanged += ListViewOnselectionChanged;


            m_FindButton.clicked += Find;
        }

        void ListViewOnselectionChanged(IEnumerable<object> obj)
        {
            Selection.activeObject = m_LoadedObjects[m_ListView.selectedIndex];
        }

        void BindItem(VisualElement arg1, int arg2)
        {
            Label label = (Label)arg1[0];
            Label path = (Label)arg1[1];

            label.text = m_LoadedObjects[arg2].name;
            path.text = m_LoadedObjectPaths[arg2];
        }

        VisualElement MakeItem()
        {
            VisualElement itemElement = new VisualElement();
            itemElement.AddToClassList("gdx-references-list-item");

            Label nameLabel = new Label("gdx-references-list-item-label");
            Label namePath = new Label("gdx-references-list-item-description");

            itemElement.Add(nameLabel);
            itemElement.Add(namePath);

            return itemElement;
        }

        void Find()
        {
            // Unload previous batch just incase
            UnloadObjects();

            // Make sure we are operating on the full written out experience
            AssetDatabase.SaveAssets();

            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_ObjectField.value));
            string[] references = GetReferences(guid);
            int referenceCount = references.Length;
            m_LoadedObjects = new List<Object>(referenceCount);
            m_LoadedObjectPaths = new List<string>(referenceCount);
            m_ListView.Clear();

            for (int i = 0; i < referenceCount; i++)
            {
                string path = references[i];

                Object loadedObject = AssetDatabase.LoadAssetAtPath<Object>(path);

                m_LoadedObjects.Add(loadedObject);
                m_LoadedObjectPaths.Add(path);
            }

            m_ListView.itemsSource = m_LoadedObjects;
        }

        void UnloadObjects()
        {
            // int loadCount = m_LoadedObjects.Count;
            // for (int i = 0; i < loadCount; i++)
            // {
            //     m_LoadedObjects[i].SafeDestroy();
            // }
            m_LoadedObjects.Clear();
        }

        static string[] GetPossibleFiles(string baseDirectory)
        {
            ConcurrentBag<string> foundFiles = new ConcurrentBag<string>();

            // Handle Folder
            string[] baseDirectories = System.IO.Directory.GetDirectories(baseDirectory);
            Parallel.ForEach(baseDirectories, directory =>
            {
                string[] files = GetPossibleFiles(directory);
                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        foundFiles.Add(files[i]);
                    }
                }
            });

            // Handle Files
            string[] baseFiles = System.IO.Directory.GetFiles(baseDirectory);
            Parallel.ForEach(baseFiles, file =>
            {
                string extension = Path.GetExtension(file);
                if (extension == ".meta" || extension == ".prefab" || extension == ".asset" || extension == ".mat")
                {
                    foundFiles.Add(file);
                }
            });

            // Return
            return foundFiles.ToArray();
        }

        static string[] GetReferences(string guid)
        {
            string dataPath = Application.dataPath;

            // Parallelized
            string[] files = GetPossibleFiles(dataPath);

            ConcurrentBag<string> hitFiles = new ConcurrentBag<string>();
            Parallel.ForEach(files, file =>
            {
                string contents = File.ReadAllText(file);
                if (contents.Contains(guid))
                {
                    // TODO: need to enforce unique
                    hitFiles.Add(file.EndsWith(".meta") ? file.Substring(0, file.Length - 5) : file);
                }
            });


            string[] found = hitFiles.ToArray();
            string[] relativePaths = new string[found.Length];
            for (int i = 0; i < found.Length; i++)
            {
                // TODO: Need to check if it is relative to assets folder and use "Assets/" prefix,
                // this wont work for packages
                string foundPath = found[i];
                if (foundPath.StartsWith(dataPath))
                {
                    relativePaths[i] = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, found[i]));
                }
                else
                {
                    relativePaths[i] = found[i];
                }
            }

            return relativePaths;
        }

        void OnDestroy()
        {
            UnloadObjects();
        }
    }
}
