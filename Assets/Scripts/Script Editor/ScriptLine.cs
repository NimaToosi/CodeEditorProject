using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NTL.ScriptEditor
{
    #region Syntax classes

    public class Syntax
    {
        public string ColorCode;
        public List<string> Items;
        public Syntax()
        {
            Items = new List<string>();
        }
        public Syntax(Color color)
            :this()
        {
            ColorCode = ColorToString(color);
        }
        public Syntax(string color)
            : this()
        {
            ColorCode = ColorToString(color);
        }
        public static string ColorToString(Color color)
        {
            return Constants.OPEN_COLOR_TAG.Replace(Constants.COLOR_PLACE_HOLDER, ColorUtility.ToHtmlStringRGB(color));
        }
        public static string ColorToString(string color)
        {
            return Constants.OPEN_COLOR_TAG.Replace(Constants.COLOR_PLACE_HOLDER, color);
        }
    }

    public enum SyntaxType
    {
        Keyword,
        Type,
        CommandFunction,
        Parameter
    }

    #endregion

    public class ScriptLine : MonoBehaviour, IPointerDownHandler
    {
        public ScriptEditor ScriptEditor;
        public Text TextControl;
        public Text TextHelper;
        public ContentSizeFitter TextSizeFitter;
        public ContentSizeFitter HelperSizeFitter;
        public Image Caret;
        public Image SelectArea;
        public TouchSelectionController touchSelectionController { get; private set; }

        public List<Dictionary<SyntaxType, Syntax>> SyntaxList;
        public RectTransform Rect { get; private set; }
        public RectTransform TextRect { get; private set; }
        public System.Action OnSubmit;
        public System.Action<string> OnChanged;

        private Canvas Canvas;
        //private GUIStyle style;
        public int CaretPosition { get; private set; }
        private float caretFrame;
        private bool isTyping;
        public bool BlinkOff { get; set; }
        private bool selectAllMode;

        private string keywordColorCode;
        private string commentColorCode;
        private string inQuoteColorCode;
        private StringBuilder item;
        private StringBuilder output;

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = string.Empty;
                CaretPosition = 0;
                InputString(value);
            }
        }

        public enum SnippetTokens { None, @if, @while, @func }
        [HideInInspector]
        public SnippetTokens SnippetToken;

        private void InitColors()
        {
            commentColorCode = Syntax.ColorToString(ScriptSyntaxData.COMMENT_COLOR);
            inQuoteColorCode = Syntax.ColorToString(ScriptSyntaxData.STRING_COLOR);
            SyntaxList = new List<Dictionary<SyntaxType, Syntax>>();
            Dictionary<SyntaxType, Syntax> defaultSyntaxList = new Dictionary<SyntaxType, Syntax>();
            SyntaxList.Add(defaultSyntaxList);

            defaultSyntaxList[SyntaxType.Keyword] = new Syntax(ScriptSyntaxData.KEYWORD_COLOR);
            defaultSyntaxList[SyntaxType.Keyword].Items.AddRange(Keywords.All);

            defaultSyntaxList[SyntaxType.CommandFunction] = new Syntax(ScriptSyntaxData.FUNCTION_COLOR);
            defaultSyntaxList[SyntaxType.CommandFunction].Items.AddRange(Functions.All);

            defaultSyntaxList[SyntaxType.Parameter] = new Syntax(ScriptSyntaxData.PARAMETER_COLOR);
            defaultSyntaxList[SyntaxType.Parameter].Items.AddRange(Parameters.All);

            defaultSyntaxList[SyntaxType.Type] = new Syntax(ScriptSyntaxData.TYPE_COLOR);
        }
        void Awake()
        {
            Rect = GetComponent<RectTransform>();
            TextRect = TextControl.GetComponent<RectTransform>();
            Canvas = transform.root.GetComponent<Canvas>();
            //style = new GUIStyle()
            //{
            //    font = TextControl.font,
            //    fontStyle = TextControl.fontStyle,
            //    fontSize = TextControl.fontSize
            //};
            CaretPosition = 0;
            touchSelectionController = GetComponent<TouchSelectionController>();
            touchSelectionController.OnLongPress = HandleLongPress;
            _text = string.Empty;
            Color c = Caret.color;
            c.a = 1;
            Caret.color = c;
            Caret.enabled = true;
            caretFrame = 1;
            caretFrame = 1;
            item = new StringBuilder();
            output = new StringBuilder();
            InitColors();
            Text = string.Empty;
        }

        void Update()
        {
            //Blink caret
            caretFrame += Time.deltaTime;
            if (caretFrame >= 0.4f)
            {
                caretFrame -= 0.4f;
                Color c = Caret.color;
                c.a = BlinkOff || isTyping ? 1 : 1 - c.a;
                Caret.color = c;
                isTyping = false;
            }
        }

        public void InputString(string inStr)
        {
            if (selectAllMode)
                DeSelect(true);

            if (CaretPosition < _text.Length)
            {
                _text = _text.Insert(CaretPosition, inStr);
                CaretPosition += inStr.Length;
            }
            else
            {
                _text += inStr;
                CaretPosition += inStr.Length;
            }

            TextControl.text = DoHighlight(_text);
            SetCaretPos();
            if (OnChanged != null) OnChanged(_text);
        }
        public void BackSpace()
        {
            if (selectAllMode)
                DeSelect(true);

            if (_text.Length == 0 || CaretPosition == 0)
                return;

            CaretPosition--;
            _text = _text.Remove(CaretPosition, 1);

            TextControl.text = DoHighlight(_text);
            SetCaretPos();
            if (OnChanged != null) OnChanged(_text);
        }
        public void MoveCaretLeft()
        {
            if (CaretPosition == 0)
                return;

            CaretPosition--;
            SetCaretPos();
        }
        public void MoveCaretRight()
        {
            if (CaretPosition == _text.Length)
                return;

            CaretPosition++;
            SetCaretPos();
        }
        public void Home()
        {
            CaretPosition = 0;
            SetCaretPos();
        }
        public void End()
        {
            CaretPosition = _text.Length;
            SetCaretPos();
        }
        public void Submit()
        {
            Caret.enabled = false;
            SelectArea.enabled = false;
            if (OnSubmit != null) OnSubmit();
        }
        public void SelectAll()
        {
            if (string.IsNullOrEmpty(_text))
                return;

            CaretPosition = _text.Length;
            SetCaretPos();
            Caret.enabled = false;
            SelectArea.enabled = true;
            selectAllMode = true;
        }
        public void DeSelect(bool clear)
        {
            if (clear)
            {
                TextControl.text = _text = string.Empty;
                CaretPosition = 0;
            }
            else
                CaretPosition = _text.Length;
            SetCaretPos();
            Caret.enabled = true;
            SelectArea.enabled = false;
            selectAllMode = false;
        }
        private void SetCaretPos()
        {
            if (CaretPosition == 0)
                TextHelper.text = string.Empty;
            else
                TextHelper.text = _text.Substring(0, CaretPosition);

            HelperSizeFitter.SetLayoutHorizontal();
        }
        public void SetCaretPosition(int position)
        {
            if (position < 0) CaretPosition = 0;
            else if (position > _text.Length) CaretPosition = _text.Length;
            else CaretPosition = position;
            SetCaretPos();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            //isPointerDown = true;

            //if (!isFocus)
            //{
            //    Focus();
            //    return;
            //}

            //if (selectAllMode)
            //    DeSelect(false);

            if (ScriptEditor.HasSelection())
            {
                ScriptEditor.ClearSelection();
                Caret.enabled = true;
            }

            Vector2 v;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(TextRect, eventData.position, Canvas.worldCamera, out v);
            if (v.y < 0) v.y = 0;
            RectTransform crc = Caret.rectTransform;
            if (v.x <= crc.sizeDelta.x)
            {
                TextHelper.text = string.Empty;
                HelperSizeFitter.SetLayoutHorizontal();
                CaretPosition = 0;
                return;
            }
            item.Length = 0;
            for (int i = 0; i < _text.Length; i++)
            {
                item.Append(_text[i]);
                TextHelper.text = item.ToString();
                HelperSizeFitter.SetLayoutHorizontal();
                CaretPosition = i + 1;
                if (crc.localPosition.x + crc.sizeDelta.x > v.x)
                    break;
            }
            //CaretPosition = style.GetCursorStringIndex(TextRect.rect, new GUIContent(_text), v);
            //SetCaretPos();
        }
        private bool FindSyntax(string item, out string color)
        {
            for (int i = 0; i < SyntaxList.Count; i++)
            {
                var dic = SyntaxList[i];
                for (int j = 0; j < dic.Count; j++)
                {
                    Syntax syntax = dic.ElementAt(j).Value;
                    if(syntax.Items.Contains(item))
                    {
                        color = syntax.ColorCode;
                        return true;
                    }   
                }
            }

            color = string.Empty;
            return false;
        }
        public string DoHighlight(string str)
        {
            item.Length = output.Length = 0;
            bool inQuoteMode = false;
            char quoteChar = '\0';
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (!Constants.HIGHLIGHT_SPLITER.Contains(ch))
                {
                    item.Append(ch);
                    if (i + 1 == str.Length)
                        ch = Constants.EMPTY;
                    else
                        continue;
                }

                #region String in quote mode

                if (!inQuoteMode && (ch == Constants.QUOTE || ch == Constants.QUOTE1))
                {
                    inQuoteMode = true;
                    quoteChar = ch;
                    item.Append(inQuoteColorCode);
                    item.Append(ch);
                    if (i + 1 == str.Length)
                    {
                        item.Append(Constants.END_COLOR_TAG);
                    }
                    output.Append(item);
                    item.Length = 0;
                    continue;
                }
                if (inQuoteMode)
                {
                    item.Append(ch == Constants.EMPTY ? string.Empty : ch.ToString());
                    if (ch == quoteChar || i + 1 == str.Length)
                    {
                        inQuoteMode = false;
                        item.Append(Constants.END_COLOR_TAG);
                    }
                    output.Append(item);
                    item.Length = 0;
                    continue;
                }

                #endregion

                #region Keywords

                SnippetToken = SnippetTokens.None;
                string token = item.ToString();
                if (FindSyntax(token, out keywordColorCode))
                {
                    // find snippet token
                    if (CaretPosition == i + 1 && !Constants.HIGHLIGHT_SPLITER.Contains(ch))
                    {
                        System.Enum.TryParse<SnippetTokens>(token, out SnippetToken);
                    }

                    item.Insert(0, keywordColorCode);
                    item.Append(Constants.END_COLOR_TAG);
                }

                #endregion

                #region Comment mode

                if (ch == '/')
                {
                    if (i + 1 < str.Length && str[i + 1] == '/')
                    {
                        output.Append(item);
                        output.Append(commentColorCode);
                        output.Append(str.Substring(i));
                        output.Append(Constants.END_COLOR_TAG);
                        break;
                    }
                }

                //if (item.Length == 0)
                //{
                //    if (ch == Constants.SLASH && (i > 0 && str[i - 1] == Constants.SLASH))
                //    {
                //        output.Insert(output.Length - 1, commentColorCode);
                //        output.Append(str.Substring(i));
                //        output.Append(Constants.END_COLOR_TAG);
                //        break;
                //    }
                //}

                #endregion

                item.Append(ch);
                output.Append(item);
                item.Length = 0;
            }
            return output.ToString();
        }
        public void FitTextWidthSize()
        {
            TextSizeFitter.SetLayoutHorizontal();
        }
        public void SetTextForCalcRect(string text)
        {
            TextControl.text = text;
            FitTextWidthSize();
        }
        public void ResetCaretBlink()
        {
            caretFrame = 0.4f;
            isTyping = true;
        }
        public void HideSelectionVisual(TextLineUI lineUI)
        {
            Image selectArea = lineUI != null ? lineUI.SelectArea : SelectArea;
            selectArea.gameObject.SetActive(false);
        }
        private float CharacterPositionToLocalPosition(string txt, int charPos)
        {
            string str = TextHelper.text;
            TextHelper.text = txt.Substring(0, charPos);
            HelperSizeFitter.SetLayoutHorizontal();
            float result = Caret.transform.localPosition.x; //+ Caret.rectTransform.sizeDelta.x;
            TextHelper.text = str;
            return result;
        }
        public void SetSelectionVisual(int startCharacter, int endCharacter, TextLineUI lineUI)
        {
            Image selectArea = SelectArea;
            string txt = _text;
            if(lineUI != null)
            {
                selectArea = lineUI.SelectArea;
                txt = lineUI.RealText;
            }
            
            if (startCharacter < 0)
                startCharacter = 0;
            
            if (endCharacter > txt.Length)
                endCharacter = txt.Length;

            if (endCharacter < startCharacter)
            {
                int temp = startCharacter;
                startCharacter = endCharacter;
                endCharacter = temp;
            }
            
            float startX =
                CharacterPositionToLocalPosition(txt, startCharacter);

            float endX =
                CharacterPositionToLocalPosition(txt, endCharacter);

            float width = endX - startX;

            if (width < 0f)
                width = -width;

            RectTransform rect = selectArea.rectTransform;

            rect.localPosition =
                new Vector2(startX, rect.localPosition.y);

            rect.sizeDelta =
                new Vector2(
                    width,
                    rect.sizeDelta.y);

            selectArea.gameObject.SetActive(true);
        }
        public void SetFullLineSelection(TextLineUI lineUI)
        {
            Image selectArea = SelectArea;
            string txt = _text;
            if (lineUI != null)
            {
                selectArea = lineUI.SelectArea;
                txt = lineUI.RealText;
            }

            float startX = 0f;

            float endX =
                CharacterPositionToLocalPosition(txt, txt.Length);

            float width = endX - startX;

            if (width <= 0f)
                width = 1f;

            RectTransform rect = selectArea.rectTransform;

            rect.anchoredPosition =
                new Vector2(
                    startX,
                    rect.anchoredPosition.y);

            rect.sizeDelta =
                new Vector2(
                    width,
                    rect.sizeDelta.y);

            selectArea.gameObject.SetActive(true);
        }
        private void HandleLongPress()
        {
            if (ScriptEditor == null)
                return;
            
            ScriptEditor.SelectWord();
            Caret.enabled = false;
        }
    }
}