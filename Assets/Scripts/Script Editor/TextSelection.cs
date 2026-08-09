using UnityEngine;

namespace NTL.ScriptEditor
{
    /// <summary>
    /// Represents a position inside the ScriptEditor text.
    /// LineIndex = line number
    /// CharacterIndex = character position inside the line
    /// </summary>
    [System.Serializable]
    public struct TextPosition
    {
        public int LineIndex;
        public int CharacterIndex;

        public TextPosition(int lineIndex, int characterIndex)
        {
            LineIndex = lineIndex;
            CharacterIndex = characterIndex;
        }

        public bool Equals(TextPosition other)
        {
            return LineIndex == other.LineIndex &&
                   CharacterIndex == other.CharacterIndex;
        }

        public override string ToString()
        {
            return "(" + LineIndex + ", " + CharacterIndex + ")";
        }
    }

    /// <summary>
    /// Stores the current text selection.
    ///
    /// Anchor = position where selection started.
    /// Active = current position of selection.
    /// </summary>
    [System.Serializable]
    public class TextSelection
    {
        [SerializeField]
        private TextPosition anchor;

        [SerializeField]
        private TextPosition active;

        public TextPosition Anchor
        {
            get { return anchor; }
        }

        public TextPosition Active
        {
            get { return active; }
        }

        public TextPosition Start
        {
            get
            {
                if (Compare(anchor, active) <= 0)
                    return anchor;

                return active;
            }
        }

        public TextPosition End
        {
            get
            {
                if (Compare(anchor, active) <= 0)
                    return active;

                return anchor;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return anchor.Equals(active);
            }
        }

        public bool IsReversed
        {
            get
            {
                return Compare(anchor, active) > 0;
            }
        }

        public TextSelection()
        {
            anchor = new TextPosition(0, 0);
            active = new TextPosition(0, 0);
        }

        public void SetAnchor(TextPosition position)
        {
            anchor = position;
            active = position;
        }

        public void SetActive(TextPosition position)
        {
            active = position;
        }

        public void Set(TextPosition start, TextPosition end)
        {
            anchor = start;
            active = end;
        }

        public void Clear()
        {
            anchor = active;
        }

        public bool Contains(TextPosition position)
        {
            if (IsEmpty)
                return false;

            return Compare(position, Start) >= 0 &&
                   Compare(position, End) <= 0;
        }

        public bool ContainsCharacter(int lineIndex, int characterIndex)
        {
            return Contains(
                new TextPosition(lineIndex, characterIndex)
            );
        }

        private static int Compare(
            TextPosition a,
            TextPosition b)
        {
            if (a.LineIndex < b.LineIndex)
                return -1;

            if (a.LineIndex > b.LineIndex)
                return 1;

            if (a.CharacterIndex < b.CharacterIndex)
                return -1;

            if (a.CharacterIndex > b.CharacterIndex)
                return 1;

            return 0;
        }

        public override string ToString()
        {
            return "Anchor: " + anchor +
                   " Active: " + active;
        }
    }
}