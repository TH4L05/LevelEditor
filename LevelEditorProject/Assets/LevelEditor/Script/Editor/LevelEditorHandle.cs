/// <author> Thomas Krahl </author>
/// <version>1.00</version>
/// <date>24/02/2022</date>

using UnityEditor;
using UnityEngine;

public class LevelEditorHandle : Editor
{
    #region Fields

    private Vector3 currentHandlePosition = Vector3.zero;
    private bool isMouseInValidArea = false;
    private Vector3 oldHandlePosition = Vector3.zero;
    private Vector3 objectScale = Vector3.zero;
    private Color drawColor = Color.white;
    private GameObject activeObject;
    //private GameObject tempObjectInstance;
    private Material previewMaterial;
    private float additionalRotation;
    private int handleHeight;

    public bool IsMouseInValidArea => isMouseInValidArea;
    public Vector3 CurrentHandlePosition => currentHandlePosition;

    #endregion

    #region InitAndDestroy


    public void IntializeHandle()
    {
        //The OnSceneGUI delegate is called every time the SceneView is redrawn and allows you
        //to draw GUI elements into the SceneView to create in editor functionality
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public void DestroyHandle()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    #endregion

    #region Main

    void OnSceneGUI(SceneView sceneView)
    {
        if (LevelEditorWindow.toolbarTabIndex == 0) return;

        UpdateHandlePosition();
        UpdateIsMouseInValidArea(sceneView.position);
        UpdateRepaint();
        DrawCubeDrawPreview();

        //Debug.Log(currentHandlePosition);
        //Debug.Log(oldHandlePosition);
        //Debug.Log(isMouseInValidArea);
    }

    void UpdateHandlePosition()
    {
        if (Event.current == null)
        {
            return;
        }

        //Debug.Log("HandlePosition");
        Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
        //Debug.Log(mousePosition);

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 offset = Vector3.zero;

            if (EditorPrefs.GetBool("SelectBlockNextToMousePosition", true) == true)
            {
                offset = hit.normal;
            }

            currentHandlePosition.x = Mathf.Floor(hit.point.x - hit.normal.x * 0.001f + offset.x);
            currentHandlePosition.y = Mathf.Floor(hit.point.y - hit.normal.y * 0.001f + offset.y);
            currentHandlePosition.z = Mathf.Floor(hit.point.z - hit.normal.z * 0.001f + offset.z);

            currentHandlePosition += new Vector3(0f, 0f + handleHeight, 0f);
        }
    }

    void UpdateIsMouseInValidArea(Rect sceneViewRect)
    {
        bool isInValidArea = Event.current.mousePosition.y < sceneViewRect.height - 35;

        if (isInValidArea != isMouseInValidArea)
        {
            isMouseInValidArea = isInValidArea;
            SceneView.RepaintAll();
        }
    }

    void UpdateRepaint()
    {
        if (currentHandlePosition != oldHandlePosition)
        {
            SceneView.RepaintAll();
            oldHandlePosition = currentHandlePosition;
        }
    }

    void DrawCubeDrawPreview()
    {
        if (isMouseInValidArea == false)
        {
            return;
        }

        Handles.color = drawColor;
        DrawHandleCube(currentHandlePosition, objectScale);
    }

    void DrawHandleCube(Vector3 center, Vector3 scale)
    {
        if (scale == Vector3.zero)
        {
            scale = new Vector3(1f, 1f, 1f);
        }

        Handles.DrawWireCube(center, scale);

        if (activeObject == null) return;


        if (LevelEditorWindow.toolbarTabIndex == 1)
        {
            if (activeObject == null) return;
            var mesh = activeObject.GetComponent<MeshFilter>().sharedMesh;

            var rotation = activeObject.transform.rotation.eulerAngles;
            rotation.y += additionalRotation;

            Graphics.DrawMesh(mesh, new Vector3(center.x, center.y, center.z ), Quaternion.Euler(rotation), previewMaterial, 0);
        }
    }

    #endregion

    #region Set

    public void SetObjectScale(Vector3 scale)
    {
        //Debug.Log(scale);
        objectScale = scale;
    }

    public void SetDrawColor(Color color)
    {
        drawColor = color;
    }

    public void SetActiveObject(GameObject obj, Material material)
    {
        activeObject = obj;
        previewMaterial = material;
    }

    public void SetHandleRotation(float rotation)
    {
        additionalRotation = rotation;
    }

    public void SetHandleHeight(int height)
    {
        handleHeight = height;
    }

    #endregion
}
