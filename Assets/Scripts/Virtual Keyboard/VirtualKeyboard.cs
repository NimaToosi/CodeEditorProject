using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour
{
    public KeyButton KeyPrefab;
    public Sprite[] AlphabetLabelSprites;
    public Image AlphabetLabel;
    public Image CapsLockIcon, CapsLockPressIcon;

    protected const string lowerCase = "1234567890qwertyuiopasdfghjklzxcvbnm";
    protected const string upperCase = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
    protected bool isCapsLock, isCapsLockPress;
    protected bool isBackSpacePress;
    protected KeyButton currentKey;

    //Actions
    public System.Action<string> InputStringCallback;
    public System.Action BackSpaceCallback;
    public System.Action SubmitCallback;

    protected void CreateKey(string label, ref Vector3 offset, Vector2 sizeDelta, 
        System.Action<KeyButton> down, System.Action<KeyButton> click)
    {
        KeyButton keyButton = Instantiate(KeyPrefab);
        keyButton.transform.SetParent(transform.GetChild(0));
        (keyButton.transform as RectTransform).sizeDelta = sizeDelta;
        keyButton.transform.localPosition = offset;
        offset.x += sizeDelta.x;
        keyButton.transform.localScale = Vector3.one;
        keyButton.Label = label;
        keyButton.DownAction = down;
        keyButton.ClickAction = click;
    }
    protected virtual void ArrangeKeyboard(float keySize) { }

    public void Initialize()
    {
        //Calc height of keyboard
        RectTransform rt = GetComponent<RectTransform>();
        float ar = 720f / rt.sizeDelta.y;
        float w = rt.rect.width;
        float h = w / ar;
        rt.sizeDelta = new Vector2(0, h);
        ArrangeKeyboard(w / 10f);
        
    }

    //Keys Actions =====================================================================

    protected void DownAction(KeyButton key)
    {
        if (currentKey && !currentKey.Release)
        {
            currentKey.Click();
            currentKey.Up();
        }
        currentKey = key;
    }
    protected bool KeyDown(KeyButton key)
    {
        DownAction(key);
        return true;
    }
    protected void InputString(KeyButton key)
    {
        if (isCapsLock)
        {
            InputStringCallback(key.Label.ToUpper());
            if (!isCapsLockPress)
                SwitchCapsLock();
        }
        else
            InputStringCallback(key.Label);
    }

    protected void BackSpaceDown(KeyButton key)
    {
        if (!KeyDown(key))
            return;
        isBackSpacePress = false;
        StartCoroutine(PressTimer(key, () => StartCoroutine(BackSpacePress(key))));
    }
    protected IEnumerator BackSpacePress(KeyButton key)
    {
        isBackSpacePress = true;
        while (key.Press)
        {
            BackSpaceCallback();
            yield return new WaitForSeconds(0.05f);
        }
    }
    protected void BackSpace(KeyButton key)
    {
        if (isBackSpacePress)
            return;
        BackSpaceCallback();
    }

    protected void SpaceBar(KeyButton key)
    {
        InputStringCallback(" ");
    }
    protected void Submit(KeyButton key)
    {
        SubmitCallback();
    }

    protected void SwitchCapsLock()
    {
        isCapsLock = !isCapsLock;
        CapsLockIcon.color = new Color(1, 1, 1, isCapsLock ? 1.0f : 0.0f);
        CapsLockPressIcon.color = new Color(1, 1, 1, isCapsLockPress ? 1.0f : 0.0f);
        AlphabetLabel.sprite = AlphabetLabelSprites[isCapsLock ? 1 : 0];
    }
    protected void CapsLockDown(KeyButton key)
    {
        if (!KeyDown(key))
            return;

        if (isCapsLockPress)
            isCapsLockPress = false;
        else
            StartCoroutine(PressTimer(key, () =>
            {
                isCapsLockPress = true;
                if (isCapsLock)
                    CapsLockPressIcon.color = new Color(1, 1, 1, 1f);
                else
                    SwitchCapsLock();
            }));
    }
    protected void CapsLock(KeyButton key)
    {
        if (isCapsLockPress)
            return;
        SwitchCapsLock();
    }

    //Timer
    protected IEnumerator PressTimer(KeyButton key, System.Action callback)
    {
        bool cancel = false;
        float frame = 0;
        do
        {
            if (!key.Press && !key.Directions)
            {
                cancel = true;
                break;
            }
            frame += Time.deltaTime;
            yield return null;

        } while (frame < 0.4f);

        if (!cancel) callback();
    }

    //===================================================================================
}
