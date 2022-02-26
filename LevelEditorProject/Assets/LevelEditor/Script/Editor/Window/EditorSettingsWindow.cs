/// <author> Thomas Krahl </author>

using UnityEngine;
using UnityEditor;

public class EditorSettingsWindow : EditorWindow
{
    private static EditorSettingsWindow window;
    private EditorData editorData;
    private Editor editor;

    public static void OpenWindow()
    {
        window = GetWindow<EditorSettingsWindow>("EditorSettings");
    }

    private void OnGUI()
    {
        editor.OnInspectorGUI();
        EditorUtility.SetDirty(editorData);
    }

    private void OnDestroy()
    {
        if(editor != null) DestroyImmediate(editor);
    }

    private void OnEnable()
    {
        Setup();
    }

    private void Setup()
    {
        try
        {
            editorData = AssetDatabase.LoadAssetAtPath<EditorData>("Assets/LevelEditor/DataEditor/EditorSettings.asset");
        }
        catch (System.Exception)
        {
            Debug.LogError("Could not Load EditorData");
            window.Close();
            throw;
        }

        editor = Editor.CreateEditor(editorData);
    }

}
