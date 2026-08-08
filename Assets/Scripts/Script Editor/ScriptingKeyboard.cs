using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptingKeyboard : VirtualKeyboard
{
    private string[] specials = { "+", "-", "*", "/", "%", "<", ">", "=", "[", "]", "(", ")", "_", "!", ",", ".", ";" };

    public System.Action LeftCallbak;
    public System.Action RightCallbak;
    public System.Action UpCallbak;
    public System.Action DownCallbak;
    public System.Action TabCallback;
    public System.Action HomeCallback;
    public System.Action EndCallback;
    public System.Action SnippetSpaceCallback;

    protected override void ArrangeKeyboard(float keySize)
    {
        Vector2 size1 = new Vector2(keySize, keySize);
        Vector2 size2 = new Vector2(keySize * 1.5f, keySize);
        Vector2 size3 = new Vector2(keySize * 3, keySize);
        Vector2 size4 = new Vector2(keySize / 1.2f, keySize);
        //Vector2 size5 = new Vector2(keySize * 2, keySize);
        int id = 0;

        ////Create Arrow keys
        //Vector3 offset = new Vector3(0, 0, 0);
        //CreateKey(0, ref offset, size5, DownAction, Left);
        //CreateKey(0, ref offset, size5, DownAction, Up);
        //CreateKey(0, ref offset, size5, DownAction, Down);
        //CreateKey(0, ref offset, size5, DownAction, Right);
        //CreateKey(specials.Length - 1, ref offset, size5, DownAction, InputSpecialString); // ;

        // Create special keys
        Vector3 offset = new Vector3(0, 0, 0);  // offset = new Vector3(0, -keySize, 0);
        for (int i = 0; i < 12; i++)
        {
            CreateKey(specials[i], ref offset, size4, DownAction, InputSpecialString);
        }

        // Create number keys
        offset = new Vector3(0, -keySize, 0);
        for (int i = 0; i < 10; i++)
        {
            CreateKey(lowerCase[id].ToString(), ref offset, size1, DownAction, InputString);
            id++;
        }

        // Create alphabetical keys
        offset = new Vector3(0, -keySize * 2, 0);
        for (int i = 0; i < 10; i++)
        {
            CreateKey(lowerCase[id].ToString(), ref offset, size1, DownAction, InputString);
            id++;
        }
        offset = new Vector3(keySize * 0.5f, -keySize * 3, 0);
        for (int i = 0; i < 9; i++)
        {
            CreateKey(lowerCase[id].ToString(), ref offset, size1, DownAction, InputString);
            id++;
        }
        offset = new Vector3(0, -keySize * 4, 0);
        CreateKey("\"", ref offset, size2, DownAction, Tab);
        for (int i = 0; i < 7; i++)
        {
            CreateKey(lowerCase[id].ToString(), ref offset, size1, DownAction, InputString);
            id++;
        }
        CreateKey("&", ref offset, size2, BackSpaceDown, BackSpace);

        offset = new Vector3(0, -keySize * 5, 0);
        CreateKey(specials[specials.Length - 5], ref offset, size2, DownAction, InputSpecialString); // _
        CreateKey(specials[specials.Length - 4], ref offset, size1, DownAction, InputSpecialString); // !
        CreateKey(specials[specials.Length - 3], ref offset, size1, DownAction, InputSpecialString); // ,
        CreateKey(" ", ref offset, size3, DownAction, SnippetSpace);
        CreateKey(specials[specials.Length - 2], ref offset, size1, DownAction, InputSpecialString); // .
        CreateKey(specials[specials.Length - 1], ref offset, size1, DownAction, InputSpecialString); // ;
        CreateKey("\n", ref offset, size2, DownAction, Submit);

        Transform allKeys = transform.GetChild(0);
        for (int i = 0; i < allKeys.childCount; i++)
        {
            allKeys.GetChild(i).GetComponent<KeyButton>().DirectionsAction = Directions;
        }
    }

    protected void InputSpecialString(KeyButton key)
    {
        InputStringCallback(key.Label);
    }
    protected void Left(KeyButton key) => LeftCallbak();
    protected void Right(KeyButton key) => RightCallbak();
    protected void Up(KeyButton key) => UpCallbak();
    protected void Down(KeyButton key) => DownCallbak();
    protected void Tab(KeyButton key) => InputStringCallback("\""); //TabCallback();
    protected void Home(KeyButton key) => HomeCallback();
    protected void End(KeyButton key) => EndCallback();
    protected void SnippetSpace(KeyButton key) => SnippetSpaceCallback();
    protected void Directions(KeyButton key)
    {
        System.Action act;
        Vector2 delta = key.Delta;
        if(Mathf.Abs(delta.x / delta.y) >= 2)
            act = delta.x > 0 ? RightCallbak : LeftCallbak;
        else
            act = delta.y > 0 ? UpCallbak : DownCallbak;
        act();
        StartCoroutine(PressTimer(key, () => StartCoroutine(DirectionsPress(key, act))));
    }
    private IEnumerator DirectionsPress(KeyButton key, System.Action act)
    {
        while (key.Directions)
        {
            act();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
