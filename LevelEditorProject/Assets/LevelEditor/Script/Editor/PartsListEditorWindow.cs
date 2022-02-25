/// <author> Thomas Krahl </author>
/// <version>1.01</version>
/// <date>25/02/2022</date>

using UnityEditor;
using UnityEngine;

public class PartsListEditorWindow : EditorWindow
{
    #region Fields

    private static PartsListEditorWindow window;
    private static AssetEditorData assetEditorData;
    private string assetListName;
    private string[] assetListNames;
    private int index;
    private Vector2 scrollPosition = Vector2.zero;

    public static int AssetListCount
    {
        get
        {
            return assetEditorData.createdPartsLists.Count;
        }
    }

    #endregion

    #region UnityFunctions

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        LeftArea();
        MyGUI.DrawUILine(Color.gray, new Rect(249, 15, 2,1000));
        RightArea();
        GUILayout.EndHorizontal();
    }

    private void OnEnable()
    {        
        try
        {
            assetEditorData = AssetDatabase.LoadAssetAtPath<AssetEditorData>("Assets/LevelEditor/DataEditor/AssetEditorData.asset");
            LoadLists();
            //PartsListStyles.SetStyles();
        }
        catch (System.Exception)
        {
            Debug.LogError("COULD NOT LOAD ASSETDATALIST");
            window.Close();
            throw;          
        }      
    }

    private void LoadLists()
    {
        if (AssetListCount == 0)
        {
            return;
        }

        assetListNames = new string[AssetListCount];
        for (int i = 0; i < assetEditorData.createdPartsLists.Count; i++)
        {
            assetListNames[i] = assetEditorData.createdPartsLists[i].assetList.name;
        }

        if (LevelEditorWindow.EditorIsActive)
        {
            LevelEditorWindow.Reload();
        }

    }

    #endregion

    #region Window

    /// <summary>
    /// Open the EditorWindow
    /// </summary>
    [MenuItem("Tools/LevelEditor/PartsListsEditorWindow")]
    public static void OpenWindow()
    {
        window = GetWindow<PartsListEditorWindow>("Part List Editor");
        window.minSize = new Vector2(400, 400);
    }

    /// <summary>
    /// Left Side of the Window - draws the ButtonList
    /// </summary>
    private void LeftArea()
    {
        GUILayout.BeginArea(new Rect(15, 15, 205, 1000));
        if (AssetListCount == 0)
        {
            GUILayout.Label("No PartList created");
        }
        else
        {
            GUILayout.Label("PartLists");
            EditorGUILayout.Space(5);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < AssetListCount; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(assetListNames[i], GUILayout.Width(180f), GUILayout.Height(25f)))
                {
                    index = i;
                }
                if (GUILayout.Button("X", GUILayout.Width(20f), GUILayout.Height(25f)))
                {
                    index = i;
                    DeleteList();
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.Space(2);
            }

            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.Separator();
        LeftAreaBottom();
        GUILayout.EndArea();
    }

    /// <summary>
    /// LeftBottomArea - draws the createList input and button
    /// </summary>
    private void LeftAreaBottom()
    {
        GUILayout.BeginArea(new Rect(15, window.position.height - 85, 200, 75));
        CreateNewPartsList();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Right side of window - draws the values from selected partlist
    /// </summary>
    private void RightArea()
    {
        GUILayout.BeginArea(new Rect(255, 15, 600, 1000));

        if (AssetListCount != 0)
        {
            var selection = assetEditorData.createdPartsLists[index].assetList;
            if (selection != null)
            {
                var editor = Editor.CreateEditor(selection);
                //editor.DrawDefaultInspector();
                editor.OnInspectorGUI();
                EditorUtility.SetDirty(selection);
            }
        }

        GUILayout.EndArea();
    }

    #endregion

    #region Create and delete List

    /// <summary>
    /// Creates a new PartsList asset and saves it on DataPath
    /// </summary>
    private void CreateNewPartsList()
    {
        GUILayout.Label("List Name:");
        assetListName = GUILayout.TextField(assetListName);
        if (GUILayout.Button("Create New List"))
        {
            if (assetListName == string.Empty)
            {
                Debug.LogWarning("Empty names are not allowed");
                return;
            }

            var data = CreateAsset(assetListName);
            assetEditorData.AddPartList(data);
            LoadLists();
            assetListName = string.Empty;
        }
    }

    private AssetData CreateAsset(string name)
    {
        var list = new AssetData();
        var assetList = ScriptableObject.CreateInstance<PartList>();
        var path = "Assets/LevelEditor/Data/" + name + ".asset";
        AssetDatabase.CreateAsset(assetList, path);
        AssetDatabase.SaveAssets();

        list.assetList = AssetDatabase.LoadAssetAtPath<PartList>(path);
        list.path = "Assets/LevelEditor/Data/" + name + ".asset";
        return list;
    }

    private void DeleteList()
    {
        index = 0;
        var assetData = assetEditorData.createdPartsLists[index];
        assetEditorData.RemovePartList(assetData);
        LoadLists();
        DeleteAsset(assetData);       
    }

    private void DeleteAsset(AssetData data)
    {
        AssetDatabase.DeleteAsset(data.path);
    }

    #endregion
}
