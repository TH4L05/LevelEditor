/// <author> Thomas Krahl </author>
/// <version>1.00</version>
/// <date>24/02/2022</date>

using UnityEditor;
using UnityEngine;

public class LevelEditorObjectPlacement : MonoBehaviour
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

    public void AddBlock(Vector3 position, GameObject template, float rotationY)
    {
        if (template == null) return;
        //Debug.Log("DrawObject");

        Vector3 finalPosition = new Vector3(position.x, position.y, position.z);
        Vector3 objRotation = template.transform.rotation.eulerAngles;
        objRotation += new Vector3(0, rotationY, 0);

        GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(template);
        //GameObject newObject = Instantiate(template, finalPosition, Quaternion.Euler(objRotation));
        newObject.transform.parent = LevelParent;
        newObject.transform.position = finalPosition;
        newObject.transform.rotation = Quaternion.Euler(objRotation);
        newObject.transform.name = template.name;

        //Make sure a proper Undo/Redo step is created. This is a special type for newly created objects
        Undo.RegisterCreatedObjectUndo(newObject, "Add " + template.name);

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }
}
