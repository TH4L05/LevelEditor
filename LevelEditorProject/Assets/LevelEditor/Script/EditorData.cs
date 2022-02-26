/// <author> Thomas Krahl </author>

using UnityEngine;

namespace LevelEditor
{
    public class EditorData : ScriptableObject
    {
        public string titleName = "LevelEditor";
        public Color titleNameColor = Color.white;
        public Material previewMaterial;

        public float LevelEditorGridButtonHeight { get; set; }
        public float LevelEditorGridButtonWidth { get; set; }
    }
}