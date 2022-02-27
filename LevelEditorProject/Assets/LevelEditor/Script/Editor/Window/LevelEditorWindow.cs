/// <author> Thomas Krahl </author>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        #region Fields

        private static LevelEditorWindow window;
        private LevelEditorHandle handle;

        private static EditorData editorData;
        private static AssetEditorData assetEditorData;
        private static int assetDataListCount;

        //Toolbar
        private string[] texts = { "Edit", "Draw", };
        public static bool EditorIsActive = false;
        private int toolbarIndex;

        //Object
        public Material previewMaterial;
        private Vector3 scale = Vector3.zero;
        private Vector3 objRotation;
        private Vector3 objOffset = new Vector3(0f, 1.5f, 0f);
        private GameObject activeObj;

        //selection grid
        private Vector2 scrollposition = Vector2.zero;
        private Vector2[] scrollpositions;
        private PartList[] levelAssetLists;
        private List<GUIContent[]> guiContents = new List<GUIContent[]>();
        //private int[] gridIndices;
        //private int[] lastgridIndices;
        private string[] foldOutTexts;
        private bool[] foldOuts;

        //gridOptions 
        private float gridSelectionHeight = 125.0f;
        private float gridSelectionWidth = 200.0f;
        private Vector2 activePos = Vector2.zero;
        private int activePartIndex;
        private int activeListIndex;

        #endregion

        #region UnityFunctions

        private void OnEnable()
        {
            EditorIsActive = true;

            Setup();
        }

        private void OnGUI()
        {
            TopLabel();

            EditorGUILayout.Separator();

            TopBar();

            EditorGUILayout.Separator();
            MyGUI.DrawUILine(Color.grey);

            Toolbar();

            EditorGUILayout.Separator();
            MyGUI.DrawUILine(Color.grey);

            OptionsArea();

            EditorGUILayout.Separator();
            MyGUI.DrawUILine(Color.grey);
            GridOptions();
            EditorGUILayout.Space(3f);
            EditorGUILayout.Separator();

            FoldoutArea();
        }

        private void OnDestroy()
        {
            EditorIsActive = false;
            ScriptableObject.DestroyImmediate(handle);
        }

        #endregion

        #region Layout

        private void TopLabel()
        {
            GUILayout.Label(editorData.titleName, LevelEditorStyles.topLabel);
            GUILayout.Space(5);
        }

        private void TopBar()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("PartsListEditor", GUILayout.Height(35), GUILayout.Width(250)))
            {
                handle.SetToolBarIndex(0);
                PartsListEditorWindow.OpenWindow();
            }

            if (GUILayout.Button("Settings", GUILayout.Height(35), GUILayout.Width(250)))
            {
                handle.SetToolBarIndex(0);
                EditorSettingsWindow.OpenWindow();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Toolbar()
        {
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, texts, GUILayout.Height(35));
            handle.SetToolBarIndex(toolbarIndex);
            GUILayout.Space(3);
        }

        private void OptionsArea()
        {
            GUILayout.Space(3);
            objRotation = EditorGUILayout.Vector3Field("Active Object Rotation", objRotation, GUILayout.MaxWidth(300));
            GUILayout.Space(5);
            objOffset = EditorGUILayout.Vector3Field("Active Object Offset", objOffset, GUILayout.MaxWidth(300));
            GUILayout.Space(3);

            if (handle != null)
            {
                handle.SetHandleHeight(objOffset);
                handle.SetHandleRotation(objRotation);
            }
        }

        private void FoldoutArea()
        {
            if (assetDataListCount != 0)
            {
                for (int i = 0; i < assetDataListCount; i++)
                {
                    if (levelAssetLists[i].parts.Count == 0) continue;

                    foldOuts[i] = EditorGUILayout.Foldout(foldOuts[i], foldOutTexts[i], true);

                    if (foldOuts[i])
                    {
                        scrollpositions[i] = EditorGUILayout.BeginScrollView(scrollpositions[i]);
                        SetParts(i);

                        //old layout with selectiongrid                   
                        //gridIndices[i] = GUILayout.SelectionGrid(gridIndices[i], guiContents[i], 2);
                        //SetActiveObject(i, gridIndices[i]);

                        EditorGUILayout.EndScrollView();
                    }
                }
            }
            else
            {
                GUILayout.Label("No PartsList created - Open PartsListEditorWindow to Create One");
            }

        }

        private void SetParts(int listIndex)
        {
            int xValue = 0;
            bool horizontalStart = false;

            var partAmount = levelAssetLists[listIndex].parts.Count;
            var areaAmount = partAmount + 1;

            float xPos = 15f;
            float yPos = 15f;

            var xAmount = (int)(window.position.width / gridSelectionWidth);
            if (xAmount > 10) xAmount = 10;

            var yAmount = (int)(guiContents[listIndex].Length / xAmount) + 1;


            float areaWidth = 10f + (gridSelectionWidth + 10) * xAmount;
            float areaHeight = 10f + (gridSelectionHeight + 10) * yAmount;
            Rect areaRect = new Rect(0f, 0f, areaWidth, areaHeight);

            scrollposition = GUILayout.BeginScrollView(scrollposition, GUILayout.Height(areaHeight + 10));
            MyGUI.DrawColorRect(Color.gray, areaRect);

            for (int partIndex = 0; partIndex < partAmount; partIndex++)
            {
                if (xValue == 0)
                {
                    GUILayout.BeginHorizontal();
                    horizontalStart = true;
                }

                if (partIndex == activePartIndex && listIndex == activeListIndex)
                {
                    MyGUI.DrawColorRect(new Color(0.27f, 0.37f, 0.48f), new Rect(xPos, yPos, gridSelectionWidth + 1, gridSelectionHeight + 1));
                }

                GUILayout.BeginArea(new Rect(xPos, yPos, gridSelectionWidth, gridSelectionHeight));
                GUILayout.BeginVertical();
                GUILayout.Label(guiContents[listIndex][partIndex].text, LevelEditorStyles.gridLabel);
                if (GUILayout.Button(guiContents[listIndex][partIndex].image, GUILayout.Height(gridSelectionHeight - 20)))
                {
                    activePartIndex = partIndex;
                    activeListIndex = listIndex;
                    SetActiveObject(activeListIndex, activePartIndex);

                }
                GUILayout.EndVertical();
                GUILayout.EndArea();

                //TestRect
                //MyGUI.DrawUILine(Color.green, new Rect(xPos ,yPos, gridSelectionWidth, gridSelectionHeight));

                xValue++;
                xPos += gridSelectionWidth + 1;

                if (xValue >= xAmount)
                {
                    horizontalStart = false;
                    GUILayout.EndHorizontal();
                    xValue = 0;
                    xPos = 15f;
                    yPos += gridSelectionHeight + 5f;
                    continue;
                }
            }

            if (horizontalStart)
            {
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private void GridOptions()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("gridButtonHeight", GUILayout.Width(100));
            gridSelectionHeight = EditorGUILayout.Slider(gridSelectionHeight, 100, 300, GUILayout.MaxWidth(200), GUILayout.Height(20));

            GUILayout.Label("gridButtonWidth", GUILayout.Width(100));
            gridSelectionWidth = EditorGUILayout.Slider(gridSelectionWidth, 100, 500, GUILayout.MaxWidth(200), GUILayout.Height(20));
            EditorGUILayout.EndHorizontal();

            SaveGridLayoutOptions();
        }

        #endregion

        #region Setup

        [MenuItem("Tools/LevelEditor/LevelEditorWindow")]
        public static void OpenWindow()
        {
            if (PartsListEditorWindow.EditorIsActive) return;
            window = GetWindow<LevelEditorWindow>("Level Editor");
            window.minSize = new Vector2(250f, 500f);
        }

        public static void Reload()
        {
            window?.Close();
            OpenWindow();
        }

        public static void CloseWindow()
        {
            window?.Close();
        }

        private void Setup()
        {
            handle = ScriptableObject.CreateInstance<LevelEditorHandle>();

            LoadData();
            LoadLevelPartsLists();
            LevelEditorStyles.SetStyles();

            if (editorData == null) return;
            if (editorData.previewMaterial != null) previewMaterial = editorData.previewMaterial;
            if (editorData.LevelEditorGridButtonHeight != 0) gridSelectionHeight = editorData.LevelEditorGridButtonHeight;
            if (editorData.LevelEditorGridButtonWidth != 0) gridSelectionWidth = editorData.LevelEditorGridButtonWidth;

            SetActiveObject(0, 0);
        }

        private void LoadData()
        {
            string[] path = AssetDatabase.FindAssets("LevelEditor");
            string dataPath = AssetDatabase.GUIDToAssetPath(path[0]);

            if (dataPath == string.Empty)
            {
                Debug.LogError("Could not find LevelEditor Path");
                window?.Close();
            }

            editorData = AssetDatabase.LoadAssetAtPath<EditorData>(dataPath +"/DataEditor/Settings.asset");

             if (editorData == null)
             {
                 Debug.LogError("Could not load EditorData");
                 window?.Close();
            } 
            
            assetEditorData = AssetDatabase.LoadAssetAtPath<AssetEditorData>(dataPath + "/DataEditor/AssetEditorData.asset");
        }

        private void LoadLevelPartsLists()
        {
            assetDataListCount = assetEditorData.createdPartsLists.Count;

            if (assetDataListCount == 0) return;

            levelAssetLists = new PartList[assetDataListCount];
            //gridIndices = new int[assetDataListCount];
            //lastgridIndices = new int[assetDataListCount];
            foldOutTexts = new string[assetDataListCount];
            foldOuts = new bool[assetDataListCount];
            scrollpositions = new Vector2[assetDataListCount];

            for (int i = 0; i < assetDataListCount; i++)
            {
                levelAssetLists[i] = AssetDatabase.LoadAssetAtPath<PartList>(assetEditorData.createdPartsLists[i].path);
                foldOutTexts[i] = levelAssetLists[i].name;

                if (i == 0)
                {
                    foldOuts[i] = true;
                }
                else
                {
                    foldOuts[i] = false;
                }

                var partAmount = levelAssetLists[i].parts.Count;
                if (partAmount == 0) return;
                var gridContentImages = new GUIContent[partAmount];

                for (int p = 0; p < partAmount; p++)
                {
                    string text = levelAssetLists[i].parts[p].PartName;
                    Texture image = AssetPreview.GetAssetPreview(levelAssetLists[i].parts[p].Template);
                    gridContentImages[p] = new GUIContent(text, image, text);
                }

                guiContents.Add(gridContentImages);
            }
        }

        #endregion

        #region Other

        private void SaveGridLayoutOptions()
        {
            

            editorData.LevelEditorGridButtonHeight = gridSelectionHeight;
            editorData.LevelEditorGridButtonWidth = gridSelectionWidth;
        }

        void SetActiveObject(int listIndex, int partIndex)
        {
            activeObj = levelAssetLists[listIndex].parts[partIndex].Template;
            handle.SetActiveObject(activeObj, previewMaterial);
        }

        #endregion
    }
}