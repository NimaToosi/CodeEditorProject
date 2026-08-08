using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler, 
                                        IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string Label { get; set; }
    public bool Press { get; private set; }
    public bool Release { get; private set; }
    public bool Directions { get; private set; }
    public Vector2 Delta { get; private set; }
    public System.Action<KeyButton> DownAction;
    public System.Action<KeyButton> ClickAction;
    public System.Action<KeyButton> DirectionsAction;
    private Image highlight;
    private Color colorDown, colorUp;
    private bool clickDone;

    private void Start()
    {
        highlight = GetComponent<Image>();
        colorDown = colorUp = highlight.color;
        colorUp.a = 0.0f;
        highlight.color = colorUp;
    }

    public void Click()
    {
        if (!clickDone)
        {
            if (ClickAction != null) ClickAction(this);
            clickDone = true;
        }
    }
    public void Up()
    {
        highlight.color = colorUp;
        Press = false;
        Release = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        highlight.color = colorDown;
        Press = true;
        clickDone = false;
        if (DownAction != null) DownAction(this);
        Release = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Up();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Up();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        clickDone = true;
        Directions = true;
        Delta = eventData.delta;
        DirectionsAction?.Invoke(this);
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Implement for active OnBeginDrag!!!
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Directions = false;
    }
}