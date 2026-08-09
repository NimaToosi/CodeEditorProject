using UnityEngine;
using UnityEngine.UI;

namespace NTL.ScriptEditor
{
    /// <summary>
    /// Renders the visual selection area for a ScriptLine.
    /// </summary>
    public class SelectionRenderer : MonoBehaviour
    {
        [SerializeField]
        private Image selectionImage;

        [SerializeField]
        private RectTransform selectionRect;

        public void SetSelection(
            float x,
            float width,
            float height)
        {
            if (selectionRect == null)
                return;

            selectionRect.anchoredPosition =
                new Vector2(x, 0f);

            selectionRect.sizeDelta =
                new Vector2(width, height);

            if (selectionImage != null)
                selectionImage.enabled = true;
        }

        public void Hide()
        {
            if (selectionImage != null)
                selectionImage.enabled = false;
        }
    }
}