/// <author> Thomas Krahl </author>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelEditorWindow : EditorWindow
{
    #region Fields

    private static LevelEditorWindow window;

    private static EditorData editorData;
    private static AssetEditorData assetEditorData;
    public static int assetDataListCount;

    private LevelEditorHandle handle;
    private LevelEditorObjectPlacement objectPlacement;

    //Toolbar
    public static int toolbarTabIndex = 0;
    private string[] texts = { "Edit", "Draw", };
    public static bool EditorIsActive = false;

    //Object
    public Material previewMaterial;    
    private Vector3 scale = Vector3.zero;
    private Vector3 objRotation;
    private Vector3 objOffset = new Vector3(0f, 0.5f, 0f);
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

    #endregion

    #region UnityFunctions

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

        FoldOuts();
       
        CheckToolBarIndex();
    }

    private void OnEnable()
    {
        EditorIsActive = true;
        LoadData();
        Setup();
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        EditorIsActive = false;
        handle.Destroy();        
        ScriptableObject.DestroyImmediate(handle);
        ToolbarOption1();
        SceneView.RepaintAll();
    }

    #endregion

    #region Layout

    private void TopLabel()
    {
        GUILayout.Label(editorData.displayName, LevelEditorStyles.topLabel);
        GUILayout.Space(5);
    }

    private void TopBar()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("PartsListEditor", GUILayout.Height(35), GUILayout.Width(250)))
        {
            ToolbarOption1();           
            PartsListEditorWindow.OpenWindow();
        }

        if (GUILayout.Button("Settings", GUILayout.Height(35), GUILayout.Width(250)))
        {
            ToolbarOption1();
            EditorSettingsWindow.OpenWindow();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void Toolbar()
    {
        toolbarTabIndex = GUILayout.Toolbar(toolbarTabIndex, texts, GUILayout.Height(35));
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

    private void FoldOuts()
    {
        if (assetDataListCount != 0)
        {
            for (int i = 0; i < assetDataListCount; i++)
            {
                if (levelAssetLists[i].parts.Count == 0) continue;

                GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
                //style.fixedWidth = 5;
                foldoutStyle.fixedHeight = 400;

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

        var yAmount = (int)(guiContents[listIndex].Length / xAmount) +1;


        float areaWidth = 10f + (gridSelectionWidth + 10) * xAmount;
        float areaHeight = 10f + (gridSelectionHeight +10) * yAmount;
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
           
            GUILayout.BeginArea(new Rect(xPos, yPos, gridSelectionWidth, gridSelectionHeight));
            GUILayout.BeginVertical();            
            GUILayout.Label(guiContents[listIndex][partIndex].text, LevelEditorStyles.gridLabel);
            if (GUILayout.Button(guiContents[listIndex][partIndex].image, GUILayout.Height(gridSelectionHeight -20)))
            {
                SetActiveObject(listIndex, partIndex);
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

    #region ToolbarOptions

    void CheckToolBarIndex()
    {
        switch (toolbarTabIndex)
        {
            case 0:
            default:
                ToolbarOption1();
                break;

            case 1:
                ToolbarOption2();
                break;

            case 2:
                ToolbarOption3();
                break;
        }
    }

    void ToolbarOption1()
    {
        Tools.hidden = false;
        //Debug.Log("OPtion1 Selected");       
        SceneView.RepaintAll();
    }

    void ToolbarOption2()
    {
        Tools.hidden = true;
        //Debug.Log("OPtion2 Selected");       
        SceneView.RepaintAll();
        handle.SetDrawColor(Color.yellow);
        handle.SetObjectScale(scale);
    }

    void ToolbarOption3()
    {
        Tools.hidden = true;
        //Debug.Log("OPtion3 Selected");
        SceneView.RepaintAll();
        handle.SetDrawColor(Color.magenta);
        handle.SetActiveObject(null, null);
        handle.SetObjectScale(new Vector3(1f, 1f, 1f));
    }

    #endregion

    #region Setup

    public LevelEditorWindow()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    [MenuItem("Tools/LevelEditor/LevelEditorWindow")]
    public static void OpenWindow()
    {
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
        objectPlacement = new LevelEditorObjectPlacement();

        toolbarTabIndex = 0;        
        handle.Initialize();
        
        LoadLevelPartsLists();       
        LevelEditorStyles.SetStyles();

        if (editorData == null) return;
        if (editorData.previewMaterial != null) previewMaterial = editorData.previewMaterial;
        if (editorData.LevelEditorGridButtonHeight != 0) gridSelectionHeight = editorData.LevelEditorGridButtonHeight;
        if(editorData.LevelEditorGridButtonWidth != 0) gridSelectionWidth = editorData.LevelEditorGridButtonWidth;
    }

    private void LoadData()
    {
        editorData = AssetDatabase.LoadAssetAtPath<EditorData>("Assets/LevelEditor/DataEditor/EditorSettings.asset");
        assetEditorData = AssetDatabase.LoadAssetAtPath<AssetEditorData>("Assets/LevelEditor/DataEditor/AssetEditorData.asset");                  
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
            levelAssetLists[i] = AssetDatabase.LoadAssetAtPath<PartList>("Assets/LevelEditor/Data/" + assetEditorData.createdPartsLists[i].assetList.name + ".asset");
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

            this.guiContents.Add(gridContentImages);
        }
    }

    #endregion

    #region Other

    private void SaveGridLayoutOptions()
    {
        editorData.LevelEditorGridButtonHeight = gridSelectionHeight;
        editorData.LevelEditorGridButtonWidth = gridSelectionWidth;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        LevelEditorObjectPlacement(); 
    }

    void LevelEditorObjectPlacement()
    {
        if (toolbarTabIndex == 0) return;

        //By creating a new ControlID here we can grab the mouse input to the SceneView and prevent Unitys default mouse handling from happening
        //FocusType.Passive means this control cannot receive keyboard input since we are only interested in mouse input
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
      
        if (Event.current.type == EventType.MouseDown &&
            Event.current.button == 0 &&
            Event.current.alt == false &&
            Event.current.shift == false &&
            Event.current.control == false)
        {
            if (handle.IsMouseInValidArea == true)
            {
                /*if (toolbarTabIndex == 2)
                {
                    objecthandle.RemoveBlock(handle.CurrentHandlePosition);
                }*/

                if (toolbarTabIndex == 1)
                {
                    objectPlacement.AddBlock(handle.CurrentHandlePosition, activeObj, objRotation);
                }
            }
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            toolbarTabIndex = 0;
        }

        HandleUtility.AddDefaultControl(controlId);
    }

    void SetActiveObject(int listIndex, int partIndex)
    {
        activeObj = levelAssetLists[listIndex].parts[partIndex].Template;
        handle.SetActiveObject(activeObj, previewMaterial);
    }

    #endregion
}