/// <author> Thomas Krahl </author>

using UnityEngine;
using UnityEditor;
using LevelEditor;

[CustomEditor(typeof(EditorData), true)]
public class EditorSettingsInspector : Editor
{
    private void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.DrawDefaultInspector();
        bool changes = serializedObject.ApplyModifiedProperties();
        if (LevelEditorWindow.EditorIsActive && changes)
        {
            LevelEditorWindow.Reload();
        }
    }
}
