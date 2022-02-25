/// <author> Thomas Krahl </author>

using UnityEngine;
using UnityEditor;

public class EditorSettingsWindow : EditorWindow
{
    private static EditorSettingsWindow window;

    [MenuItem("Tools/LevelEditor/EditorSettingsWindow")]
    public static void OpenWindow()
    {
        window = GetWindow<EditorSettingsWindow>("EditorSettings");
    }
}
