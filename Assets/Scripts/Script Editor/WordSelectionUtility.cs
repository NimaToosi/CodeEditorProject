namespace NTL.ScriptEditor
{
    public static class WordSelectionUtility
    {
        public static void GetWordRange(
            string text,
            int characterIndex,
            out int start,
            out int end)
        {
            start = 0;
            end = 0;

            if (string.IsNullOrEmpty(text))
                return;

            if (characterIndex < 0)
                characterIndex = 0;

            if (characterIndex > text.Length)
                characterIndex = text.Length;

            if (characterIndex == text.Length &&
                characterIndex > 0)
            {
                characterIndex--;
            }

            if (!IsWordCharacter(
                text[characterIndex]))
            {
                start = characterIndex;
                end = characterIndex + 1;

                return;
            }

            start = characterIndex;

            while (start > 0 &&
                   IsWordCharacter(
                       text[start - 1]))
            {
                start--;
            }

            end = characterIndex + 1;

            while (end < text.Length &&
                   IsWordCharacter(
                       text[end]))
            {
                end++;
            }
        }

        private static bool IsWordCharacter(
            char character)
        {
            return
                char.IsLetterOrDigit(character) ||
                character == '_';
        }
    }
}