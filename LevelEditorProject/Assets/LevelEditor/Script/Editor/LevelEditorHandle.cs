/// <author> Thomas Krahl </author>

using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class LevelEditorHandle : Editor
    {
        #region Fields

        private LevelEditorObjectPlacement editorObjectPlacement;

        private Vector3 currentHandlePosition = Vector3.zero;
        private Vector3 oldHandlePosition = Vector3.zero;
        private bool isMouseInValidArea = false;

        private GameObject activeObject;
        private Material previewMaterial;
        private Color drawColor = Color.white;
        private Vector3 objectScale = Vector3.zero;
        private Vector3 additionalRotation = Vector3.zero;
        private Vector3 handleOffset = Vector3.zero;

        private bool selectBlockNextToMousePosition = false;
        private int levelEditorToolbarIndex;

        #endregion

        #region UnityFunctions

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Destroy();
        }

        #endregion

        #region Initialize And Destroy

        public void Initialize()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            editorObjectPlacement = new LevelEditorObjectPlacement();
            SetToolBarIndex(0);
            activeObject = null;
        }

        public void Destroy()
        {
            activeObject = null;
            SetToolBarIndex(0);
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        #endregion

        #region ToolbarOptions

        public void ResetToolbarIndex()
        {
            levelEditorToolbarIndex = 0;
        }

        public void SetToolBarIndex(int index)
        {
            levelEditorToolbarIndex = index;

            switch (levelEditorToolbarIndex)
            {
                case 0:
                default:
                    ToolbarOption1();
                    break;

                case 1:
                    ToolbarOption2();
                    break;

                case 2:
                    ToolbarOption3();
                    break;
            }
        }

        void ToolbarOption1()
        {
            Tools.hidden = false;
            SceneView.RepaintAll();
        }

        void ToolbarOption2()
        {
            Tools.hidden = true;
            SceneView.RepaintAll();
            SetDrawColor(Color.yellow);
        }

        void ToolbarOption3()
        {
            Tools.hidden = true;
            SceneView.RepaintAll();
            SetDrawColor(Color.magenta);
            SetActiveObject(null, null);
            SetObjectScale(new Vector3(1f, 1f, 1f));
        }

        #endregion

        #region Main

        void OnSceneGUI(SceneView sceneView)
        {
            if (levelEditorToolbarIndex == 0) return;

            UpdateRepaint();        
            UpdateIsMouseInValidArea(sceneView.position);
            UpdateHandlePosition();           
            DrawObjectPreview();            
            LevelEditorObjectPlacement();
            KeyboardEvents();
        }


        private void KeyboardEvents()
        {
            if (isMouseInValidArea)
            {
                Event e = Event.current;
                if (e?.isKey == true)
                {
                    switch (e.type)
                    {
                        case EventType.MouseDown:
                            break;

                        case EventType.MouseUp:
                            break;

                        case EventType.MouseMove:
                            break;

                        case EventType.MouseDrag:
                            break;

                        case EventType.KeyDown:

                            if (levelEditorToolbarIndex == 0) return;

                            switch (e.keyCode)
                            {
                                case KeyCode.Y:
                                    RotateYPlus();
                                    break;

                                case KeyCode.X:
                                    RotateYMinus();
                                    break;
                            }

                            break;
                        case EventType.KeyUp:
                            break;

                        case EventType.ScrollWheel:
                            break;

                        case EventType.Repaint:
                            break;

                        case EventType.Layout:
                            break;

                        case EventType.DragUpdated:
                            break;

                        case EventType.DragPerform:
                            break;

                        case EventType.DragExited:
                            break;

                        case EventType.Ignore:
                            break;

                        case EventType.Used:
                            break;

                        case EventType.ValidateCommand:
                            break;

                        case EventType.ExecuteCommand:
                            break;

                        case EventType.ContextClick:
                            break;

                        case EventType.MouseEnterWindow:
                            break;

                        case EventType.MouseLeaveWindow:
                            break;

                        default:
                            break;
                    }
                }
            }

            
        }

        private void UpdateIsMouseInValidArea(Rect sceneViewRect)
        {
            bool isInValidArea = Event.current.mousePosition.y < sceneViewRect.height - 35;

            if (isInValidArea != isMouseInValidArea)
            {
                isMouseInValidArea = isInValidArea;
                SceneView.RepaintAll();
            }
        }

        private void UpdateHandlePosition()
        {
            if (Event.current == null) return;

            Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 offset = Vector3.zero;

                if (selectBlockNextToMousePosition)
                {
                    offset = hit.normal;
                }

                currentHandlePosition.x = Mathf.Floor(hit.point.x - hit.normal.x * 0.001f + offset.x);
                currentHandlePosition.y = Mathf.Floor(hit.point.y - hit.normal.y * 0.001f + offset.y);
                currentHandlePosition.z = Mathf.Floor(hit.point.z - hit.normal.z * 0.001f + offset.z);
            }
        }    

        private void UpdateRepaint()
        {
            if (currentHandlePosition != oldHandlePosition)
            {
                SceneView.RepaintAll();
                oldHandlePosition = currentHandlePosition;
            }
        }

        private void DrawObjectPreview()
        {
            if (isMouseInValidArea == false)
            {
                return;
            }

            Handles.color = drawColor;
            DrawHandleCube(currentHandlePosition, objectScale);
        }

        private void DrawHandleCube(Vector3 position, Vector3 scale)
        {
            if (scale == Vector3.zero)
            {
                scale = new Vector3(1f, 1f, 1f);
            }

            //Handles.DrawWireCube(center, scale);

            if (activeObject == null) return;

            if (levelEditorToolbarIndex == 1)
            {
                if (activeObject == null) return;
                var mesh = activeObject.GetComponent<MeshFilter>().sharedMesh;
                var rotation = activeObject.transform.rotation.eulerAngles;
                rotation += additionalRotation;
                Graphics.DrawMesh(mesh, position += handleOffset, Quaternion.Euler(rotation), previewMaterial, 0);
            }
        }

        private void LevelEditorObjectPlacement()
        {
            if (levelEditorToolbarIndex == 0) return;

            //By creating a new ControlID here we can grab the mouse input to the SceneView and prevent Unitys default mouse handling from happening
            //FocusType.Passive means this control cannot receive keyboard input since we are only interested in mouse input
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                Event.current.alt == false && Event.current.shift == false && Event.current.control == false)
            {
                if (isMouseInValidArea == true)
                {
                    if (levelEditorToolbarIndex == 1)
                    {
                        editorObjectPlacement.AddBlock(currentHandlePosition += handleOffset, activeObject, additionalRotation);
                    }
                    else if (levelEditorToolbarIndex == 2)
                    {
                        editorObjectPlacement.RemoveBlock(currentHandlePosition);
                    }
                }
            }
            
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                SetToolBarIndex(0);
            }
            HandleUtility.AddDefaultControl(controlId);
        }

        #endregion

        #region Set

        public void SetObjectScale(Vector3 scale)
        {
            objectScale = scale;
        }

        public void SetDrawColor(Color color)
        {
            drawColor = color;
        }

        public void SetActiveObject(GameObject obj, Material material)
        {
            activeObject = null;
            activeObject = obj;
            previewMaterial = material;
        }

        public void SetHandleRotation(Vector3 rotation)
        {
            additionalRotation = rotation;
        }

        public void SetHandleHeight(Vector3 offset)
        {
            handleOffset = offset;
        }


        private void RotateYPlus()
        {
            //Debug.Log("RotatePLUS");
            additionalRotation.y += 45f;
            if (additionalRotation.y == 360f) additionalRotation.y = 0f;
            LevelEditorWindow.UpdateRotation(additionalRotation);
            
        }

        private void RotateYMinus()
        {
            //Debug.Log("RotateMINUS");
            additionalRotation.y -= 45f;
            if (additionalRotation.y == -360f) additionalRotation.y = 0f;
            LevelEditorWindow.UpdateRotation(additionalRotation);
        }


        #endregion
    }
}