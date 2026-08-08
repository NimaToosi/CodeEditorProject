using UnityEngine;
using System.Collections;

public abstract class ScrollItem : MonoBehaviour
{
    public int ID { get; private set; }
    public int RealIndex { get; private set; }
    public abstract bool Visible { get; set; }
    public virtual void InitItem(int id)
    {
        ID = id;
        RealIndex = id;
        Visible = false;
    }
    public virtual void Set(int index)
    {
        RealIndex = index;
    }
    public virtual bool IsSelect { set { } }
}
