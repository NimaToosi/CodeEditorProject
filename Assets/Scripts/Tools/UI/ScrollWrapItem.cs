using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollWrapItem : MonoBehaviour
{
    private int _itemCount = 0;
    public int ItemCount
    {
        get { return _itemCount; }
        set
        {
            int newCount = value;
            for (int i = 0; i < childCount; i++)
            {
                if (i < newCount) ItemList[i].Visible = true;
                else ItemList[i].Visible = false;
            }
            _itemCount = value;
            SetContentHeight();
        }
    }
    public float CellMargin;
    public ScrollRect ScrollView;

    public RectTransform Rect { get; private set; }
    public Rect Viewport { get; private set; }
    public Vector2 CellSize { get; private set; }
    private int childCount;
    private int firstRealIndex;
    private int firstItemIndex, lastItemIndex;
    private ScrollItem firstItem, lastItem;
    private Vector2 upPosition, downPosition;
    public ScrollItem[] ItemList { get; private set; }

    public System.Action<int, ScrollItem> OnInitItem { get; set; }

    public void InitializeItems()
    {
        Rect = GetComponent<RectTransform>();
        Viewport = Rect.rect;

        childCount = 1;//transform.childCount;
        CellSize = ((RectTransform)transform.GetChild(0)).sizeDelta + new Vector2(0, CellMargin * 2);

        //Add new items and Set first item an last item
        firstItemIndex = 0;
        firstItem = transform.GetChild(0).GetComponent<ScrollItem>();
        int count = (int)(Rect.rect.height / CellSize.y) + 4;
        ItemList = new ScrollItem[count];
        for (int i = 0; i < count; i++)
        {
            if (i == 0)
            {
                ItemList[i] = firstItem;
                firstItem.InitItem(i);
                continue;
            }
            ScrollItem newItem = Instantiate(firstItem);
            newItem.transform.SetParent(transform);
            newItem.transform.localScale = Vector3.one;
            newItem.GetComponent<RectTransform>().sizeDelta = firstItem.GetComponent<RectTransform>().sizeDelta;
            newItem.transform.localPosition = firstItem.transform.localPosition - new Vector3(0, i * CellSize.y, 0);
            childCount++;
            ItemList[i] = newItem;
            newItem.InitItem(i);
        }
        lastItemIndex = childCount - 1;
        lastItem = ItemList[lastItemIndex];

        //Set up and down posiiton
        Vector2 pos = firstItem.transform.localPosition;
        firstItem.transform.localPosition += new Vector3(0, CellSize.y, 0);
        upPosition = firstItem.transform.position;
        firstItem.transform.localPosition = pos;
        downPosition = lastItem.transform.position;

        ItemCount = 1;
    }

    public void ResetContent()
    {
        transform.localPosition = Vector3.zero;
        OnScroll(Vector2.zero);
        for (int i = 0; i < ItemList.Length; i++)
        {
            ItemList[i].Visible = false;
        }
    }
    public void SetContentHeight()
    {
        Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _itemCount * CellSize.y);
    }
    public void SetContentWidth(float size)
    {
        Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Viewport.width + size);
    }
    public void OnScroll(Vector2 delta)
    {
        while (firstRealIndex + childCount < _itemCount && firstItem.transform.position.y > upPosition.y)
        {
            firstItem.transform.localPosition = lastItem.transform.localPosition - new Vector3(0, CellSize.y);
            firstItemIndex++;
            if (firstItemIndex == childCount)
                firstItemIndex = 0;
            firstItem = ItemList[firstItemIndex];
            lastItemIndex++;
            if (lastItemIndex == childCount)
                lastItemIndex = 0;
            lastItem = ItemList[lastItemIndex];
            firstRealIndex++;

            if (OnInitItem != null)
                OnInitItem(firstRealIndex + childCount - 1, lastItem);
        }

        while (firstRealIndex > 0 && lastItem.transform.position.y < downPosition.y)
        {
            lastItem.transform.localPosition = firstItem.transform.localPosition + new Vector3(0, CellSize.y);
            firstItemIndex--;
            if (firstItemIndex < 0)
                firstItemIndex = childCount - 1;
            firstItem = ItemList[firstItemIndex];
            lastItemIndex--;
            if (lastItemIndex < 0)
                lastItemIndex = childCount - 1;
            lastItem = ItemList[lastItemIndex];
            firstRealIndex--;

            if (OnInitItem != null)
                OnInitItem(firstRealIndex, firstItem);
        }
    }
    public ScrollItem GetItemAt(int realIndex)
    {
        for (int i = 0; i < ItemList.Length; i++)
        {
            if (ItemList[i].RealIndex == realIndex)
                return ItemList[i];
        }
        return null;
    }
    public void CallSetForAllVisibleItems()
    {
        for (int i = 0; i < ItemList.Length; i++)
        {
            var item = ItemList[i];
            if (item.gameObject.activeSelf)
                item.Set(item.RealIndex);
        }
    }
}
