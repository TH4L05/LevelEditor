/// <author> Thomas Krahl </author>

using UnityEngine;

public class EditorData : ScriptableObject
{
    public string displayName = "LevelEditor";
    public Color displayNameColor = Color.white;
    public Material previewMaterial;

    public float LevelEditorGridButtonHeight { get; set; }
    public float LevelEditorGridButtonWidth { get; set; }

}
