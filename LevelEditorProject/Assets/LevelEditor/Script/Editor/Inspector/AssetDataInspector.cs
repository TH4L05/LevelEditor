/// <author> Thomas Krahl </author>

using UnityEditor;
using UnityEngine;
using LevelEditor;

[CustomEditor(typeof(AssetEditorData), true)]
public class AssetDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}