using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NTL.ScriptEditor
{
    public class TextLineUI : ScrollItem, IPointerDownHandler
    {
        public ScriptEditor ScriptEditor;
        public Text Text;
        public Image SelectArea;
        public string RealText { get; set; }

        public override bool Visible
        {
            get
            {
                return Text.enabled;
            }

            set
            {
                Text.enabled = value;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ScriptEditor.SetCurrentLine(RealIndex);
            ScriptEditor.ScriptLine.OnPointerDown(eventData);
            ScriptEditor.SetCaretPosition();
        }
        public override void Set(int index)
        {
            base.Set(index);
            Text.text = ScriptEditor.HL_Lines[index];
            RealText  = ScriptEditor.Lines[index];
            ScriptEditor.RefreshTextLineSelectionVisual(this);
        }
    } 
}
