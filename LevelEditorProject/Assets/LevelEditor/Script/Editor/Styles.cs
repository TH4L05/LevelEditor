/// <author> Thomas Krahl </author>

using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public struct LevelEditorStyles
    {
        #region Fields

        public static GUIStyle topLabel = new GUIStyle();
        public static GUIStyle gridLabel = new GUIStyle();
        public static EditorData EditorData { get; set; }

        #endregion

        #region Styles

        public static void SetStyles()
        {
            topLabel.alignment = TextAnchor.MiddleCenter;
            topLabel.normal.textColor = EditorData.titleNameColor;
            topLabel.fontStyle = FontStyle.Bold;
            topLabel.fontSize = 25;

            gridLabel.alignment = TextAnchor.MiddleCenter;
            gridLabel.normal.textColor = Color.white;
        }

        #endregion
    }

    public class MyGUI
    {
        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawUILine(Color color, Rect rect)
        {
            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawColorRect(Color color, Rect rect)
        {
            EditorGUI.DrawRect(rect, color);
        }
    }
}


