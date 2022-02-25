/// <author> Thomas Krahl </author>
/// <version>1.00</version>
/// <date>24/02/2022</date>

using UnityEditor;
using UnityEngine;

public struct LevelEditorStyles
{
    #region Fields

    public static GUIStyle topLabel = new GUIStyle();
    public static GUIStyle textstyle2 = new GUIStyle();
    public static GUIStyle toolbarStyle = new GUIStyle();
    public static GUIStyle boxStyle = new GUIStyle();
    public static GUIStyle gridLabel = new GUIStyle();
    public static GUIStyle gridButton = new GUIStyle();
    private static EditorData data;

    #endregion

    #region Styles

    public static void SetStyles()
    {
        data = AssetDatabase.LoadAssetAtPath<EditorData>("Assets/LevelEditor/DataEditor/EditorDefault.asset");

        topLabel.alignment = TextAnchor.MiddleCenter;
        topLabel.normal.textColor = data.displayNameColor;
        topLabel.fontStyle = FontStyle.Bold;
        topLabel.fontSize = 25;

        textstyle2.alignment = TextAnchor.MiddleCenter;
        textstyle2.normal.textColor = Color.white;

        boxStyle.fixedWidth = 100f;
        boxStyle.normal.textColor = Color.white;

        gridLabel.alignment = TextAnchor.MiddleCenter;
        gridLabel.normal.textColor = Color.white;


        //gridButton.alignment = TextAnchor.MiddleCenter;
        //gridButton.normal.background = ;
        //gridButton.hover.background = ;
    }

    #endregion
}

/*public struct PartsListStyles
{
    #region Fields

    public static GUIStyle rectStyle = new GUIStyle();
    public static GUIStyle buttonList = new GUIStyle();
    public static GUIStyle buttonDelete = new GUIStyle();
    
    #endregion

    #region Styles

    public static void SetStyles()
    {
        rectStyle.stretchWidth = true;
        rectStyle.border = new RectOffset(1, 1, 1, 1);
    }

    #endregion
}*/

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
