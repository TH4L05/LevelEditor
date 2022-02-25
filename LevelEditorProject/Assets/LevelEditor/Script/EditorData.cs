/// <author> Thomas Krahl </author>
/// <version>1.00</version>
/// <date>24/02/2022</date>

using UnityEngine;

[CreateAssetMenu(fileName = "New EditorData", menuName = "LevelEditor/EditorData")]
public class EditorData : ScriptableObject
{
    public string displayName = "LevelEditor";
    public Color displayNameColor = Color.white;
    public Material previewMaterial;

    public float LevelEditorGridButtonHeight { get; set; }
    public float LevelEditorGridButtonWidth { get; set; }

}
