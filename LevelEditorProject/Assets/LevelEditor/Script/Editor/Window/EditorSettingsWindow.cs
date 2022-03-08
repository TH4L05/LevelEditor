/// <author> Thomas Krahl </author>

using UnityEngine;
using UnityEditor;

namespace LevelEditor
{
    public class EditorSettingsWindow : EditorWindow
    {
        private static EditorSettingsWindow window;
        private EditorData editorData;
        private Editor editor;
        private string dataPath;

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
            if (editor != null) DestroyImmediate(editor);
        }

        private void OnEnable()
        {
            bool setupResult = Setup();

            if (!setupResult)
            { 
                window.Close();
                Debug.LogError("Setup Failed");
            }
        }

        private bool Setup()
        {
            bool result;

            result = CheckEditorPath();
            if (!result)
            {
                return false;
            }

            editorData = AssetDatabase.LoadAssetAtPath<EditorData>(dataPath + "/DataEditor/Settings.asset");        
            editor = Editor.CreateEditor(editorData);
            return true;
        }

        private bool CheckEditorPath()
        {
            string[] path = AssetDatabase.FindAssets("LevelEditor");
            dataPath = AssetDatabase.GUIDToAssetPath(path[0]);

            if (dataPath == string.Empty)
            {
                Debug.LogError("Could not find LevelEditor Path");
                return false;
            }
            return true;
        }


    }
}




