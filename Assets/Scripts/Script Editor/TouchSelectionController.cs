using UnityEngine;
using UnityEngine.EventSystems;

namespace NTL.ScriptEditor
{
    public class TouchSelectionController :
        MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField]
        private float longPressTime = 0.5f;

        private float pointerDownTime;
        private bool pointerDown;
        private bool longPressTriggered;

        public System.Action OnLongPress;

        public void OnPointerDown(
            PointerEventData eventData)
        {
            pointerDown = true;
            longPressTriggered = false;

            pointerDownTime = Time.unscaledTime;
        }

        public void OnPointerUp(
            PointerEventData eventData)
        {
            pointerDown = false;
        }

        private void Update()
        {
            if (!pointerDown ||
                longPressTriggered)
                return;

            float elapsed =
                Time.unscaledTime -
                pointerDownTime;

            if (elapsed >= longPressTime)
            {
                longPressTriggered = true;

                if (OnLongPress != null)
                    OnLongPress();
            }
        }
    }
}