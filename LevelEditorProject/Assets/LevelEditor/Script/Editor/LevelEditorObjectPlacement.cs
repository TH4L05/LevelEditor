/// <author> Thomas Krahl </author>

using UnityEditor;
using UnityEngine;

public class LevelEditorObjectPlacement
{
    static Transform levelParent;
    public static Transform LevelParent
    {
        get
        {
            if (levelParent == null)
            {
                GameObject go = GameObject.Find("Environment");

                if (go != null)
                {
                    levelParent = go.transform;
                }
            }

            return levelParent;
        }
    }

    public void RemoveBlock(Vector3 position)
    {
        for (int i = 0; i < LevelParent.childCount; ++i)
        {
            float distanceToBlock = Vector3.Distance(LevelParent.GetChild(i).transform.position, position);
            if (distanceToBlock < 0.1f)
            {
                //Use Undo.DestroyObjectImmediate to destroy the object and create a proper Undo/Redo step for it
                Undo.DestroyObjectImmediate(LevelParent.GetChild(i).gameObject);

                //Mark the scene as dirty so it is being saved the next time the user saves
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                return;
            }
        }
    }

    public void AddBlock(Vector3 position, GameObject template, Vector3 rotation)
    {
        if (template == null) return;

        Vector3 spawnPosition = new Vector3(position.x, position.y, position.z);
        Vector3 objRotation = template.transform.rotation.eulerAngles;
        objRotation += rotation;

        GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(template);
        newObject.transform.parent = LevelParent;
        newObject.transform.position = spawnPosition;
        newObject.transform.rotation = Quaternion.Euler(objRotation);
        newObject.transform.name = template.name;

        //create Undo/Redo step
        Undo.RegisterCreatedObjectUndo(newObject, "Add " + template.name);

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }
}


