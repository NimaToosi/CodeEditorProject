using UnityEngine;

namespace NTL.ScriptEditor
{
    public class SelectionHandleController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform startHandle;

        [SerializeField]
        private RectTransform endHandle;

        [SerializeField]
        private Canvas canvas;

        private ScriptEditor scriptEditor;

        private SelectionHandle startHandleComponent;
        private SelectionHandle endHandleComponent;
        private TextPosition fixedPosition;

        public void Initialize(
            ScriptEditor editor)
        {
            scriptEditor = editor;

            if (startHandle != null)
            {
                startHandleComponent =
                    startHandle.GetComponent<SelectionHandle>();

                if (startHandleComponent != null)
                {
                    startHandleComponent.OnDragStart =
                        HandleDragStart;
                    startHandleComponent.OnHandleDrag =
                        HandleDrag;
                }
            }

            if (endHandle != null)
            {
                endHandleComponent =
                    endHandle.GetComponent<SelectionHandle>();

                if (endHandleComponent != null)
                {
                    endHandleComponent.OnDragStart =
                        HandleDragStart;
                    endHandleComponent.OnHandleDrag =
                        HandleDrag;
                }
            }

            //startHandle.SetParent(scriptEditor.WrapItem.transform);
            //startHandle.localScale = Vector3.one;
            //endHandle.SetParent(scriptEditor.WrapItem.transform);
            //endHandle.localScale = Vector3.one;

            Hide();
        }

        public void Show()
        {
            if (startHandle != null)
                startHandle.gameObject.SetActive(true);

            if (endHandle != null)
                endHandle.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (startHandle != null)
                startHandle.gameObject.SetActive(false);

            if (endHandle != null)
                endHandle.gameObject.SetActive(false);
        }

        private void HandleDragStart(SelectionHandle handle)
        {
            if (scriptEditor == null ||
                scriptEditor.Selection == null)
                return;

            if (handle.HandleType == SelectionHandleType.Start)
            {
                fixedPosition =
                    scriptEditor.Selection.End;
            }
            else
            {
                fixedPosition =
                    scriptEditor.Selection.Start;
            }
        }

        private void HandleDrag(SelectionHandle handle)
        {
            if (scriptEditor == null)
                return;

            scriptEditor.UpdateSelectionFromHandle(handle, fixedPosition);
        }
        
        public void PositionHandles(TextPosition start, TextPosition end)
        {
            if (scriptEditor == null)
                return;
            
            Vector2 startPosition =
                scriptEditor.GetTextPositionScreenPosition(
                    start);

            Vector2 endPosition =
                scriptEditor.GetTextPositionScreenPosition(
                    end);

            if (startHandle != null)
            {
                startHandle.position =
                    startPosition;
            }

            if (endHandle != null)
            {
                endHandle.position =
                    endPosition;
            }
        }
    }
}