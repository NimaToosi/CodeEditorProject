using UnityEngine;
using UnityEngine.EventSystems;

namespace NTL.ScriptEditor
{
    public enum SelectionHandleType
    {
        Start,
        End
    }

    public class SelectionHandle :
        MonoBehaviour,
        IPointerDownHandler,
        IDragHandler,
        IPointerUpHandler
    {
        [SerializeField]
        private SelectionHandleType handleType;

        private bool dragging;

        public SelectionHandleType HandleType
        {
            get { return handleType; }
        }

        public System.Action<SelectionHandle> OnDragStart;
        public System.Action<SelectionHandle> OnHandleDrag;
        public System.Action<SelectionHandle> OnDragEnd;

        public void OnPointerDown(
            PointerEventData eventData)
        {
            dragging = true;

            if (OnDragStart != null)
                OnDragStart(this);
        }

        public void OnDrag(
            PointerEventData eventData)
        {
            if (!dragging)
                return;

            transform.position = eventData.position;
            transform.localPosition += new Vector3(0, 35, 0);

            if (OnHandleDrag != null)
            {
                OnHandleDrag(this);
            }
        }

        public void OnPointerUp(
            PointerEventData eventData)
        {
            if (!dragging)
                return;

            dragging = false;

            if (OnDragEnd != null)
                OnDragEnd(this);
        }
    }
}