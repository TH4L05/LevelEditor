/// <author> Thomas Krahl </author>

using UnityEngine;
using UnityEditor;

public class EditorSettingsWindow : EditorWindow
{
    private static EditorSettingsWindow window;
    private EditorData editorData;
    Editor editor;

    [MenuItem("Tools/LevelEditor/EditorSettingsWindow")]
    public static void OpenWindow()
    {
        window = GetWindow<EditorSettingsWindow>("EditorSettings");
    }

    private void OnGUI()
    {
        //editor.DrawDefaultInspector();
        editor.OnInspectorGUI();
        EditorUtility.SetDirty(editorData);
    }

    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        editorData = AssetDatabase.LoadAssetAtPath<EditorData>("Assets/LevelEditor/DataEditor/EditorSettings.asset");

        if (editorData != null)
        {
            editor = Editor.CreateEditor(editorData);           
        }
        else
        {
            Debug.Log("AAAAAA");
        }
    }

}
