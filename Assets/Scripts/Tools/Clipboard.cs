using System.Collections.Generic;

public static class Clipboard
{
    public static List<string> ScriptLines = new List<string>();

    public static void CopyScript(IEnumerable<string> lines)
    {
        ScriptLines.Clear();
        ScriptLines.AddRange(lines);
    }
}
