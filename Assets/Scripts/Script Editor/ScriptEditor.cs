using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace NTL.ScriptEditor
{
	public class ScriptEditor : MonoBehaviour
	{
		public ScriptLine ScriptLine;
		public ScrollWrapItem WrapItem;
		public ScriptingKeyboard Keyboard;
		public List<string> Lines { get; private set; }
		public List<string> HL_Lines { get; private set; }
		public int CaretPosition { get; private set; }
        public TextSelection Selection { get; private set; }
        private int curLineIndex;
		private ScrollItem curTextLine;

		#region Auto actions by press and hold keys

		//Auto actions by press and hold keys
		private bool keyPress;
		private float pressFrame;
		private float actionFrame;
		private float maxActionFrame;
		private System.Action pressAction;
		private void SetupPressAction(System.Action act, float maxActFrame = 0.08f)
		{
			act();
			keyPress = true;
			pressFrame = 0;
			maxActionFrame = maxActFrame;
			actionFrame = maxActFrame;
			pressAction = act;
		}

		#endregion

		public void Clear()
		{
			WrapItem.ResetContent();
			WrapItem.ItemCount = 1;
			Lines.Clear();
			HL_Lines.Clear();
			Lines.Add(string.Empty);
			HL_Lines.Add(string.Empty);
			curLineIndex = 0;
			curTextLine = WrapItem.ItemList[0];
			ScriptLine.Text = string.Empty;
			Vector2 pos = ScriptLine.transform.localPosition;
			pos.y = WrapItem.ItemList[0].transform.localPosition.y;
			ScriptLine.transform.localPosition = pos;
			ScriptLine.SetCaretPosition(0);
			CaretPosition = 0;
            if (Selection != null)
                Selection.Clear();
        }
		public void Load(string scriptText)
		{
			Clear();

			#region Load from file or player pref

			string[] split = scriptText.Split('\n');
			for (int i = 0; i < split.Length; i++)
			{
				if (i == 0)
				{
					Lines[0] = split[i];
					HL_Lines[0] = ScriptLine.DoHighlight(split[i]);
				}
				else
				{
					Lines.Add(split[i]);
					HL_Lines.Add(ScriptLine.DoHighlight(split[i]));
				}
			}

			#endregion

			WrapItem.ItemCount = Lines.Count;
			for (int i = 0; i < WrapItem.ItemList.Length && i < Lines.Count; i++)
			{
				WrapItem.ItemList[i].Set(i);
				WrapItem.ItemList[i].Visible = true;
				ScriptLine.SetTextForCalcRect(Lines[i]);
				SetWrapItemWidth();
			}

			ScriptLine.Text = Lines[0];
		}
		public string Save()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < Lines.Count; i++)
			{
				sb.Append(Lines[i] + (i == Lines.Count - 1 ? string.Empty : "\n"));
			}
			return sb.ToString();
		}
		
		public void Initialize()
		{
			if(Application.platform == RuntimePlatform.WindowsPlayer)
			{
				Keyboard.transform.parent.gameObject.SetActive(false);
				RectTransform rc = (RectTransform)transform;
				RectTransform prc = (RectTransform)transform.parent;
				rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, prc.rect.height - 45);
			}
			WrapItem.InitializeItems();
			Vector2 anchoredPosition = ScriptLine.Rect.anchoredPosition;
			ScriptLine.transform.SetParent(WrapItem.transform);
			ScriptLine.transform.localScale = Vector3.one;
			ScriptLine.Rect.anchoredPosition = anchoredPosition;
			ScriptLine.OnChanged = OnChangeLine;
			WrapItem.OnInitItem = OnInitLine;
			Lines = new List<string>();
			HL_Lines = new List<string>();
            Selection = new TextSelection();
            CaretPosition = -1;
			Keyboard.Initialize();
			Keyboard.InputStringCallback = InputString;
			Keyboard.BackSpaceCallback = BackSpace;
			Keyboard.LeftCallbak = Left;
			Keyboard.RightCallbak = Right;
			Keyboard.UpCallbak = Up;
			Keyboard.DownCallbak = Down;
			Keyboard.TabCallback = Tab;
			Keyboard.HomeCallback = Home;
			Keyboard.EndCallback = End;
			Keyboard.SubmitCallback = Submit;
			Keyboard.SnippetSpaceCallback = SnippetSpace;
			//Load();
		}

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		void Update()
		{
			#region Keys 

			if (Input.GetKeyDown(KeyCode.Return))
			{
				Submit();
			}

			else if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				Up();
			}

			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				Down();
			}

			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				Left();
			}

			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				Right();
			}

			else if (Input.GetKeyDown(KeyCode.Tab))
			{
				Tab();
				//InsertSnippet();
			}

			else if (Input.GetKeyDown(KeyCode.Home))
			{
				Home();
			}

			else if (Input.GetKeyDown(KeyCode.End))
			{
				End();
			}

			else if (Input.anyKey && !string.IsNullOrEmpty(Input.inputString))
			{
				if (Input.GetKey(KeyCode.Backspace))
				{
					BackSpace();
				}
				else if (Input.GetKey(KeyCode.Space))
				{
					SnippetSpace();
				}
                else
				{
					InputString(Input.inputString);
				}
			}

			if (keyPress)
			{
				if (!Input.anyKey)
				{
					keyPress = false;
					ScriptLine.BlinkOff = false;
					return;
				}

				pressFrame += Time.deltaTime;
				if (pressFrame >= 0.4)
				{
					ScriptLine.BlinkOff = true;
					actionFrame += Time.deltaTime;
					if (actionFrame >= maxActionFrame)
					{
						actionFrame -= maxActionFrame;
						pressAction();
					}
				}
			}

			#endregion

			//Scroll with mouse wheel
			if (Input.mouseScrollDelta.y != 0)
			{
				WrapItem.transform.localPosition -= new Vector3(0, Input.mouseScrollDelta.y * WrapItem.CellSize.y, 0);
				VerticalNormalizeScroll();
				WrapItem.OnScroll(Vector2.zero);
			}
        }
#endif

		private void SetWrapItemWidth()
		{
			ScriptLine.FitTextWidthSize();
			if (ScriptLine.TextRect.rect.xMax > WrapItem.Rect.rect.xMax - 40)
			{
				WrapItem.SetContentWidth(ScriptLine.TextRect.rect.xMax - WrapItem.Viewport.xMax + 40);
			}
		}
		private void HorizontalNormalizeScroll()
		{
			WrapItem.ScrollView.horizontalNormalizedPosition =
					Mathf.Clamp(WrapItem.ScrollView.horizontalNormalizedPosition, 0, 1);
		}
		private void VerticalNormalizeScroll()
		{
			WrapItem.ScrollView.verticalNormalizedPosition =
					Mathf.Clamp(WrapItem.ScrollView.verticalNormalizedPosition, 0, 1);
		}
		private void CaretToViewport()
		{
			if (ScriptLine.Caret.transform.localPosition.x + WrapItem.transform.localPosition.x > WrapItem.Viewport.xMax - 30)
			{
				Vector3 pos = WrapItem.transform.localPosition;
				pos.x = WrapItem.Viewport.xMax - 30 - ScriptLine.Caret.transform.localPosition.x;
				WrapItem.transform.localPosition = pos;
				HorizontalNormalizeScroll();
			}
			else if (ScriptLine.Caret.transform.localPosition.x + WrapItem.transform.localPosition.x < WrapItem.Viewport.xMin + 30)
			{
				Vector3 pos = WrapItem.transform.localPosition;
				pos.x = WrapItem.Viewport.xMin + 30 - ScriptLine.Caret.transform.localPosition.x;
				WrapItem.transform.localPosition = pos;
				HorizontalNormalizeScroll();
			}
		}
		private void ScriptlineToViewport()
		{
			float localPos = ScriptLine.transform.localPosition.y + WrapItem.transform.localPosition.y;
			if (localPos < WrapItem.Viewport.yMin || localPos > WrapItem.Viewport.yMax)
			{
				Vector2 pos = WrapItem.transform.localPosition;
				pos.y = -ScriptLine.transform.localPosition.y - WrapItem.CellSize.y;
				WrapItem.transform.localPosition = pos;
				VerticalNormalizeScroll();
				WrapItem.OnScroll(Vector2.zero);
			}
		}
		public void SetCurrentLine(int newIndex)
		{
			HL_Lines[curLineIndex] = ScriptLine.TextControl.text;
			if (curTextLine.RealIndex == curLineIndex)
				curTextLine.Set(curLineIndex);

			ScrollItem newTextLine = WrapItem.GetItemAt(newIndex);
			curLineIndex = newIndex;
			curTextLine = newTextLine;
			ScriptLine.Text = Lines[newIndex];
			Vector2 pos = ScriptLine.transform.localPosition;
			pos.y = newTextLine.transform.localPosition.y;
			ScriptLine.transform.localPosition = pos;
		}
		public void SetCaretPosition()
		{
			CaretPosition = ScriptLine.CaretPosition;
		}
		private void OneLineScrollUp()
		{
			if (ScriptLine.transform.localPosition.y + WrapItem.transform.localPosition.y > WrapItem.Viewport.yMax - WrapItem.CellSize.y)
			{
				WrapItem.transform.localPosition -= new Vector3(0, WrapItem.CellSize.y, 0);
				VerticalNormalizeScroll();
			}
		}
		private void OneLineScrollDown()
		{
			OneLineScrollDown(ScriptLine.transform.localPosition.y);
		}
		private void OneLineScrollDown(float lineY)
		{
			if (lineY + WrapItem.transform.localPosition.y < WrapItem.Viewport.yMin + WrapItem.CellSize.y)
			{
				WrapItem.transform.localPosition += new Vector3(0, WrapItem.CellSize.y, 0);
				VerticalNormalizeScroll();
			}
		}
        #region Text Selection

		public void RefreshTextLineSelectionVisual(TextLineUI lineUI)
		{
            if (Selection == null ||
                Selection.IsEmpty)
            {
				ScriptLine.HideSelectionVisual(lineUI);
                return;
            }

            TextPosition start = Selection.Start;
            TextPosition end = Selection.End;

            int firstLine = start.LineIndex;
            int lastLine = end.LineIndex;

            int index = lineUI.RealIndex;
            string text = lineUI.RealText;

            if (index < firstLine || index > lastLine)
            {
                ScriptLine.HideSelectionVisual(lineUI);
				return;
            }

			if (curLineIndex == index || firstLine == lastLine)
				return;

            if (index == firstLine)
            {
                ScriptLine.SetSelectionVisual(
                    start.CharacterIndex,
                    text.Length, lineUI);
            }
            else if (index == lastLine)
            {
                ScriptLine.SetSelectionVisual(
                    0,
                    end.CharacterIndex, lineUI);
            }
            else
            {
                ScriptLine.SetFullLineSelection(lineUI);
            }
        }

        public void RefreshMultiLineSelectionVisual()
        {
            if (Selection == null ||
                Selection.IsEmpty)
            {
                HideAllSelectionVisuals();
                return;
            }

            TextPosition start = Selection.Start;
            TextPosition end = Selection.End;

            int firstLine = start.LineIndex;
            int lastLine = end.LineIndex;
            
            for (int i = 0; i < WrapItem.ItemList.Length; i++)
            {
				TextLineUI lineUI = (TextLineUI)WrapItem.ItemList[i];
				int index = lineUI.RealIndex;
				string text = lineUI.RealText;

				if(index < firstLine || index > lastLine)
				{
					ScriptLine.HideSelectionVisual(lineUI);
					continue;
				}

				if (curLineIndex == index)
				{
					lineUI = null;
					text = ScriptLine.Text;
				}
				
                if (firstLine == lastLine)
                {
					ScriptLine.SetSelectionVisual(
						start.CharacterIndex,
						end.CharacterIndex, null);
				}
                else if (index == firstLine)
                {
                    ScriptLine.SetSelectionVisual(
                        start.CharacterIndex,
                        text.Length, lineUI);
                }
                else if (index == lastLine)
                {
					ScriptLine.SetSelectionVisual(
						0,
						end.CharacterIndex, lineUI);
				}
                else
                {
					ScriptLine.SetFullLineSelection(lineUI);
				}
            }
        }

        public void RefreshSelectionVisual()
        {
            RefreshMultiLineSelectionVisual();
        }

        public void BeginSelection()
        {
            Selection.SetAnchor(new TextPosition(curLineIndex, ScriptLine.CaretPosition));
			RefreshSelectionVisual();
        }

        public void UpdateSelection()
        {
			Selection.SetActive(new TextPosition(curLineIndex, ScriptLine.CaretPosition));
            RefreshSelectionVisual();
        }

        public void ClearSelection()
        {
            if (Selection == null)
                return;

            Selection.Clear();
			HideAllSelectionVisuals();
        }

        public bool HasSelection()
        {
            return Selection != null &&
                   !Selection.IsEmpty;
        }

        private void HideAllSelectionVisuals()
        {
			ScriptLine.HideSelectionVisual(null);
            for (int i = 0; i < WrapItem.ItemList.Length; i++)
            {
                TextLineUI lineUI = (TextLineUI)WrapItem.ItemList[i];
                ScriptLine.HideSelectionVisual(lineUI);
            }
        }

        public TextPosition SelectionStart
        {
            get
            {
                return Selection.Start;
            }
        }

        public TextPosition SelectionEnd
        {
            get
            {
                return Selection.End;
            }
        }

        public void SelectWord()
        {
            string text = ScriptLine.Text;

            if (string.IsNullOrEmpty(text))
                return;

            int caret =
                ScriptLine.CaretPosition;

            int start;
            int end;

            WordSelectionUtility.GetWordRange(
                text,
                caret,
                out start,
                out end);

            Selection.Set(
                new TextPosition(
                    curLineIndex,
                    start),

                new TextPosition(
                    curLineIndex,
                    end)
            );

            RefreshSelectionVisual();
        }

        #endregion
        public void Left()
		{
            if (HasSelection())
            {
                ClearSelection();
                return;
            }

            ScriptlineToViewport();
			SetupPressAction(() =>
			{
				ScriptLine.ResetCaretBlink();
				ScriptLine.MoveCaretLeft();
				CaretPosition = ScriptLine.CaretPosition;
				CaretToViewport();
			}, 0.02f);
		}
		public void Right()
		{
            if (HasSelection())
            {
                ClearSelection();
                return;
            }

            ScriptlineToViewport();
			SetupPressAction(() =>
			{
				ScriptLine.ResetCaretBlink();
				ScriptLine.MoveCaretRight();
				CaretPosition = ScriptLine.CaretPosition;
				CaretToViewport();
			}, 0.02f);
		}
		public void Up()
		{
            if (HasSelection())
            {
                ClearSelection();
                return;
            }

            SetupPressAction(() => 
			{
				ScriptLine.ResetCaretBlink();
				ScriptlineToViewport();
				if (curLineIndex - 1 == -1)
					return;

				SetCurrentLine(curLineIndex - 1);
				if (CaretPosition >= 0) ScriptLine.SetCaretPosition(CaretPosition);
				OneLineScrollUp();
				CaretToViewport();
            });
		}
		public void Down()
		{
            if (HasSelection())
            {
                ClearSelection();
                return;
            }

            SetupPressAction(() => 
			{
				ScriptLine.ResetCaretBlink();
				ScriptlineToViewport();
				if (curLineIndex + 1 == Lines.Count)
					return;

				SetCurrentLine(curLineIndex + 1);
				if (CaretPosition >= 0) ScriptLine.SetCaretPosition(CaretPosition);
				OneLineScrollDown();
				CaretToViewport();
			});
		}
		public void Tab()
		{
			ScriptLine.ResetCaretBlink();
			ScriptlineToViewport();
			ScriptLine.InputString("    ");
			CaretPosition = ScriptLine.CaretPosition;
			SetWrapItemWidth();
			CaretToViewport();
		}
		public void InsertSnippet()
		{
			switch (ScriptLine.SnippetToken)
			{
				case ScriptLine.SnippetTokens.@if:
					#region if snippet

					int cp = CaretPosition + 1;
					int lp = curLineIndex;
					InputString("()");
					Submit();
					for (int i = 0; i <= cp - 3; i++)
					{
						InputString(" ");
					}
					Submit();
					for (int i = 0; i < cp - 3; i++)
					{
						InputString(" ");
					}
					InputString(Keywords.END);
					SetCurrentLine(lp);
					CaretPosition = cp;
					ScriptLine.SetCaretPosition(CaretPosition);
					
					#endregion
					break;
				case ScriptLine.SnippetTokens.@while:
					#region while snippet

					cp = CaretPosition + 1;
                    lp = curLineIndex;
                    InputString("()");
                    Submit();
                    for (int i = 0; i <= cp - 6; i++)
                    {
                        InputString(" ");
                    }
                    Submit();
                    for (int i = 0; i <= cp - 6; i++)
                    {
                        InputString(" ");
                    }
                    InputString($"i += 1{Constants.END_STATEMENT}");
                    Submit();
                    for (int i = 0; i < cp - 6; i++)
                    {
                        InputString(" ");
                    }
                    InputString(Keywords.END);
                    SetCurrentLine(lp);
					Home(); Submit(); SetCurrentLine(lp);
                    for (int i = 0; i < cp - 6; i++)
                    {
                        InputString(" ");
                    }
                    InputString($"{Keywords.FLOAT} i = 0{Constants.END_STATEMENT}");
                    SetCurrentLine(lp + 1);
                    CaretPosition = cp;
                    ScriptLine.SetCaretPosition(CaretPosition);

                    #endregion
                    break;
				case ScriptLine.SnippetTokens.@func:
                    #region func snippet

                    cp = CaretPosition + 1;
                    lp = curLineIndex;
                    InputString(" ()");
                    Submit();
                    for (int i = 0; i <= cp - 5; i++)
                    {
                        InputString(" ");
                    }
                    Submit();
                    for (int i = 0; i < cp - 5; i++)
                    {
                        InputString(" ");
                    }
                    InputString(Keywords.END);
                    SetCurrentLine(lp);
                    CaretPosition = cp;
                    ScriptLine.SetCaretPosition(CaretPosition);

                    #endregion
                    break;
			}
		}
		public void SnippetSpace()
		{
			if (ScriptLine.SnippetToken != ScriptLine.SnippetTokens.None)
                InsertSnippet();
			else
				InputString(" ");
		}
		public void Home()
		{
            if (HasSelection())
            {
                ClearSelection();
                return;
            }

            ScriptlineToViewport();
			ScriptLine.Home();
			CaretPosition = ScriptLine.CaretPosition;
			CaretToViewport();
		}
		public void End()
		{
            if (HasSelection())
            {
                ClearSelection();
                return;
            }

            ScriptlineToViewport();
			ScriptLine.End();
			CaretPosition = ScriptLine.CaretPosition;
			CaretToViewport();
		}
		public void BackSpace()
		{
			ScriptLine.ResetCaretBlink();
			if (ScriptLine.CaretPosition > 0)
			{
				ScriptlineToViewport();
				ScriptLine.BackSpace();
				CaretPosition = ScriptLine.CaretPosition;
			}
			else if (curLineIndex > 0)
			{
				WrapItem.ItemCount--;
				ScriptlineToViewport();
				string str = ScriptLine.Text;
				SetCurrentLine(curLineIndex - 1);
				CaretPosition = ScriptLine.CaretPosition;
				ScriptLine.Text += str;
				SetWrapItemWidth();
				ScriptLine.SetCaretPosition(CaretPosition);
				Lines.RemoveAt(curLineIndex + 1);
				HL_Lines.RemoveAt(curLineIndex + 1);
				OneLineScrollUp();
				WrapItem.OnScroll(Vector2.zero);
				ShiftDownTexts(curLineIndex + 1);
			}
			CaretToViewport();
		}
		public void InputString(string input)
		{
			ScriptLine.ResetCaretBlink();
			ScriptlineToViewport();
			ScriptLine.InputString(input);
			CaretPosition = ScriptLine.CaretPosition;
			SetWrapItemWidth();
			CaretToViewport();
		}
		public void Submit()
		{
			ScriptLine.ResetCaretBlink();
			string strPart1 = ScriptLine.Text.Substring(0, ScriptLine.CaretPosition);
			string strPart2 = ScriptLine.Text.Substring(ScriptLine.CaretPosition, ScriptLine.Text.Length - ScriptLine.CaretPosition);
			Lines.Insert(curLineIndex + 1, strPart2);
			HL_Lines.Insert(curLineIndex + 1, ScriptLine.DoHighlight(strPart2));
			ScriptLine.Text = strPart1;
			WrapItem.ItemCount++;
			ScriptlineToViewport();
			OneLineScrollDown(ScriptLine.transform.localPosition.y - WrapItem.CellSize.y);
			WrapItem.OnScroll(Vector2.zero);
			SetCurrentLine(curLineIndex + 1);
			ShiftDownTexts(curLineIndex + 1);
			CaretPosition = 0;
			ScriptLine.SetCaretPosition(0);
			CaretToViewport();
		}
        public void OnPointerDownTextLineUI(int realIndex, PointerEventData eventData)
		{
            SetCurrentLine(realIndex);
            ScriptLine.OnPointerDown(eventData);
			ScriptLine.touchSelectionController.OnPointerDown(eventData);
            SetCaretPosition();
        }
        public void OnPointerUpTextLineUI(PointerEventData eventData)
		{
			ScriptLine.touchSelectionController.OnPointerUp(eventData);
		}
        private void ShiftDownTexts(int realIndex)
		{
			if (realIndex >= Lines.Count)
				return;

			int id = WrapItem.GetItemAt(realIndex).ID;
			int nextID = id + 1;
			if (nextID == WrapItem.ItemList.Length) nextID = 0;
			if (WrapItem.ItemList[nextID].transform.localPosition.y <
				WrapItem.ItemList[id].transform.localPosition.y)
			{
				ShiftDownTexts(realIndex + 1);
			} 

			WrapItem.ItemList[id].Set(realIndex);
		}
		private void OnInitLine(int index, ScrollItem line)
		{
			line.Set(index);
		}
		private void OnChangeLine(string text)
		{
			Lines[curLineIndex] = text;
		}
	}
}
