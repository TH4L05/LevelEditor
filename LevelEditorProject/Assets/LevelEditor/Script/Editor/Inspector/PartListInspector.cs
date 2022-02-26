/// <author> Thomas Krahl </author>

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using LevelEditor;

[CustomEditor(typeof(PartList), true)]
public class PartListInspector : Editor
{
    #region Fields

    private SerializedProperty parts;
    private ReorderableList list;
    private Vector2 scrollPosition = Vector2.zero;
    private bool changes;

    #endregion

    #region UnityFunctions

    private void OnEnable()
    {
        parts = serializedObject.FindProperty("parts");
        list = new ReorderableList(serializedObject, parts);
        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawHeader;
    }

    private void OnValidate()
    {
        if (LevelEditorWindow.EditorIsActive && changes)
        {
            LevelEditorWindow.Reload();
        }
    }

    private void OnDestroy()
    {
        if (LevelEditorWindow.EditorIsActive)
        {
            LevelEditorWindow.Reload();
        }
    }

    public override void OnInspectorGUI()
    {      
        serializedObject.Update();
        //base.DrawDefaultInspector();
        list.DoLayoutList();

        //Layout without ReorderableList 
        /*EditorGUILayout.Space(5);
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Parts");
        for (int i = 0; i < parts.arraySize; i++)
        {
            parts.GetArrayElementAtIndex(i).isExpanded = true;
            EditorGUILayout.PropertyField(parts.GetArrayElementAtIndex(i), true);
            EditorGUILayout.Space(3);
            
            MyGUI.DrawUILine(Color.gray);
        }*/

        changes = serializedObject.ApplyModifiedProperties();        
    }

    #endregion

    #region Reorderable List

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), "Name:");
        EditorGUI.PropertyField(new Rect(rect.x + 55, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("partName"), GUIContent.none);
        EditorGUI.LabelField(new Rect(rect.x + 265, rect.y, 50, EditorGUIUtility.singleLineHeight), "Prefab:");
        EditorGUI.PropertyField(new Rect(rect.x + 320, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("template"), GUIContent.none);
    }

    void DrawHeader(Rect rect)
    {
        string name = "PartList - Count: " + parts.arraySize.ToString("00");
        EditorGUI.LabelField(rect, name);
    }

    #endregion

    #region Test

    void AddPart()
    {
        parts.arraySize++;
    }

    void RemovePart()
    {
        parts.arraySize--;
    }

    #endregion
}
