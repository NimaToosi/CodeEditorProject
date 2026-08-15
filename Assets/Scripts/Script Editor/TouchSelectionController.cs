using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NTL.ScriptEditor
{
    public class TouchSelectionController :
        MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IDragHandler
    {
        [SerializeField]
        private float longPressTime = 0.5f;

        [SerializeField]
        private float movementTolerance = 15f;

        private float pointerDownTime;

        private Vector2 pointerDownPosition;

        private bool pointerDown;

        private bool longPressTriggered;

        private bool dragging;

        public Action OnLongPress;

        public Action<PointerEventData> OnSelectionDrag;

        public void OnPointerDown(
            PointerEventData eventData)
        {
            pointerDown = true;
            dragging = false;
            longPressTriggered = false;

            pointerDownTime =
                Time.unscaledTime;

            pointerDownPosition =
                eventData.position;
        }

        public void OnPointerUp(
            PointerEventData eventData)
        {
            pointerDown = false;
            dragging = false;
        }

        public void OnDrag(
            PointerEventData eventData)
        {
            if (!longPressTriggered)
                return;

            dragging = true;

            if (OnSelectionDrag != null)
                OnSelectionDrag(eventData);
        }

        private void Update()
        {
            if (!pointerDown ||
                longPressTriggered)
            {
                return;
            }

            float elapsed =
                Time.unscaledTime -
                pointerDownTime;

            float movement =
                Vector2.Distance(
                    Input.mousePosition,
                    pointerDownPosition);

            if (movement > movementTolerance)
            {
                pointerDown = false;
                return;
            }

            if (elapsed >= longPressTime)
            {
                longPressTriggered = true;

                if (OnLongPress != null)
                    OnLongPress();
            }
        }
    }
}