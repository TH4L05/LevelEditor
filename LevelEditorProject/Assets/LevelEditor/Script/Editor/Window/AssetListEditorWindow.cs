/// <author> Thomas Krahl </author>

using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class AssetListEditorWindow : EditorWindow
    {
        #region Fields

        private static AssetListEditorWindow window;
        private string dataPath;
        private Editor editor;
        private static AssetEditorData assetEditorData;
        private string partListName;
        private string[] partListNames;
        private int index;
        private Vector2 scrollPosition = Vector2.zero;
        private Vector2 scrollPositionLeft = Vector2.zero;
        private Vector2 scrollPositionRight = Vector2.zero;
        private bool levelEditorWasEnabled;
        public static bool EditorIsActive = false;

        public static int PartListsCount
        {
            get
            {
                return assetEditorData.createdPartsLists.Count;
            }
        }

        #endregion

        #region UnityFunctions

        private void OnEnable()
        {
            bool setupResult = Setup();

            if (setupResult)
            {
                EditorIsActive = true;
            }
            else
            {
                window.Close();
                Debug.LogError("Setup Failed");
            }

        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            LeftArea();
            MyGUI.DrawUILine(Color.gray, new Rect(245f, 0f, 2f, window.position.height));
            RightArea();
            GUILayout.EndHorizontal();
        }

        private void OnDestroy()
        {
            Destroy();
        }

        #endregion

        #region Window

        /// <summary>
        /// Open the EditorWindow and set the min window size
        /// </summary>
        [MenuItem("Tools/LevelEditor/AssetListsEditorWindow")]
        public static void OpenWindow()
        {
            window = GetWindow<AssetListEditorWindow>("Asset List Editor");
            window.minSize = new Vector2(400, 400);
        }

        /// <summary>
        /// Left Side of the Window - draws the ButtonList
        /// </summary>
        private void LeftArea()
        {
            scrollPositionLeft = EditorGUILayout.BeginScrollView(scrollPositionLeft, false, false);
            GUILayout.BeginArea(new Rect(15f, 15f, 225f, window.position.height - 150f));
            if (PartListsCount == 0)
            {
                GUILayout.Label("No PartList created");
            }
            else
            {
                GUILayout.Label("PartLists");
                EditorGUILayout.Space(5f);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                for (int i = 0; i < PartListsCount; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(partListNames[i], GUILayout.Width(175f), GUILayout.Height(25f)))
                    {
                        if (editor != null) DestroyImmediate(editor);
                        index = i;
                        CreateEditor();
                    }
                    if (GUILayout.Button("X", GUILayout.Width(20f), GUILayout.Height(25f)))
                    {
                        index = i;
                        DeleteList();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space(2f);
                }

                EditorGUILayout.EndScrollView();
            }

            GUILayout.EndArea();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Separator();

            LeftAreaBottom();
        }

        /// <summary>
        /// LeftBottomArea - draws the createList input and button
        /// </summary>
        private void LeftAreaBottom()
        {
            GUILayout.BeginArea(new Rect(15f, window.position.height - 85f, 200f, 75f));
            CreateNewPartsList();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Right side of window - draws the values from selected partlist
        /// </summary>
        private void RightArea()
        {
            GUILayout.BeginArea(new Rect(255f, 15f, window.position.width - 275f, window.position.height - 15f));
            scrollPositionRight = EditorGUILayout.BeginScrollView(scrollPositionRight);
            if (PartListsCount != 0 && editor != null) editor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Creates an Editor if ListButton is clicked
        /// </summary>
        private void CreateEditor()
        {
            if (PartListsCount == 0) return;

            var selection = assetEditorData.createdPartsLists[index].AssetList;
            if (selection != null)
            {
                editor = Editor.CreateEditor(selection);
                EditorUtility.SetDirty(selection);
            }
        }

        #endregion

        #region Setup and Destroy

        public void ReloadLevelEditorWindow()
        {
            if (LevelEditorWindow.EditorIsActive)
            {
                LevelEditorWindow.Reload();
            }
        }
     
        private bool Setup()
        {
            if (LevelEditorWindow.EditorIsActive)
            {
                levelEditorWasEnabled = true;
                LevelEditorWindow.CloseWindow();
                Debug.Log("LevelEditor gets closed to prevent causing Errors it will be reopened when AssetListEditor gets closed");
            }

            bool result;

            result = CheckDataPath();
            if (!result)
            {
                return false;
            }

            result = LoadAssetEditorData();
            if (!result)
            {
                return false;
            }

            LoadLists();
            index = 0;
            if (PartListsCount > 0) CreateEditor();
            return true;
        }

        private bool CheckDataPath()
        {
            string[] path = AssetDatabase.FindAssets("LevelEditor");
            dataPath = AssetDatabase.GUIDToAssetPath(path[0]);

            if (dataPath == string.Empty)
            {
                Debug.LogError("Could not find LevelEditor Path");
                return false;
            }
            return true;
        }

        private bool LoadAssetEditorData()
        {
            assetEditorData = AssetDatabase.LoadAssetAtPath<AssetEditorData>(dataPath + "/DataEditor/AssetEditorData.asset");

            if (assetEditorData == null)
            {
                Debug.LogError("Could not load EditorData");
                return false;
            }
            return true;
        }

        private void LoadLists()
        {
            if (PartListsCount == 0) return;

            partListNames = new string[PartListsCount];
            for (int i = 0; i < assetEditorData.createdPartsLists.Count; i++)
            {
                partListNames[i] = assetEditorData.createdPartsLists[i].AssetList.name;
            }
        }

        private void Destroy()
        {
            EditorIsActive = false;
            if (levelEditorWasEnabled)
            {
                LevelEditorWindow.OpenWindow();
            }

            if (editor != null) DestroyImmediate(editor);
        }

        #endregion

        #region Create / Delete List

        /// <summary>
        /// Creates a new PartsList asset and saves it on DataPath
        /// </summary>
        private void CreateNewPartsList()
        {
            GUILayout.Label("List Name:");
            partListName = GUILayout.TextField(partListName);
            if (GUILayout.Button("Create New List"))
            {
                if (partListName == string.Empty)
                {
                    Debug.LogWarning("Empty names are not allowed");
                    return;
                }
                else if (partListName.Length > 12)
                {
                    Debug.LogWarning("Names bigger then 12 Character are not allowed");
                    return;
                }

                var data = CreateAsset(partListName);
                assetEditorData.AddPartList(data);
                LoadLists();
                partListName = string.Empty;
            }
        }

        private AssetData CreateAsset(string name)
        {
            var list = new AssetData();
            var assetList = ScriptableObject.CreateInstance<AssetList>();
            var listName = "/Data/" + name + ".asset";
            AssetDatabase.CreateAsset(assetList, dataPath + listName);
            AssetDatabase.SaveAssets();

            list.SetList(AssetDatabase.LoadAssetAtPath<AssetList>(dataPath + listName), listName);
            return list;
        }

        private void DeleteList()
        {
            if (editor != null) DestroyImmediate(editor);

            var assetData = assetEditorData.createdPartsLists[index];
            assetEditorData.RemovePartList(assetData);
            index = 0;
            LoadLists();
            CreateEditor();
            DeleteAsset(assetData);
        }

        private void DeleteAsset(AssetData data)
        {
            AssetDatabase.DeleteAsset(dataPath + data.Path);
        }

        #endregion
    }
}

