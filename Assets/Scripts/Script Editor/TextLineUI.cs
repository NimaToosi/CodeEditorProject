using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NTL.ScriptEditor
{
    public class TextLineUI : ScrollItem, IPointerDownHandler, IPointerUpHandler
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
            ScriptEditor.OnPointerDownTextLineUI(RealIndex, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ScriptEditor.OnPointerUpTextLineUI(eventData);
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
