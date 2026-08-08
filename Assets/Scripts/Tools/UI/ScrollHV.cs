using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollHV : MonoBehaviour, IBeginDragHandler
{
    public ScrollRect Scroll;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x / eventData.delta.y) >= 2f)
        {
            Scroll.horizontal = true;
            Scroll.vertical = false;
        }
        else
        {
            Scroll.horizontal = false;
            Scroll.vertical = true;
        }
    }
}
