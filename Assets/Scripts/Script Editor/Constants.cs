using System.Globalization;
using System.Linq;

namespace NTL
{
    public static class Constants
    {
        public const string OPEN_COLOR_TAG = "<color=#ff>";
        public const string COLOR_PLACE_HOLDER = "ff";
        public const string END_COLOR_TAG = "</color>";

        //public const string HIGHLIGHT_SPLITER = " (){},=+-*/.\"\';!<>[]\t";
        public const string HIGHLIGHT_SPLITER = " =\"<>\t#+-*/%.()[],!;";
        public const string PARSER_SPLITER = " =\"<>\t#+-*/%";

        public const char START_ARG = '(';
        public const char END_ARG = ')';
        public const char START_GROUP = '{';
        public const char END_GROUP = '}';
        public const char COMMA = ',';
        public const char SPACE = ' ';
        public const char END_STATEMENT = ';';
        public const string ARRAY_MARK = "[]";
        public const char START_ARRAY = '[';
        public const char END_ARRAY = ']';
        public const char QUOTE = '"';
        public const char QUOTE1 = '\'';
        public const char EMPTY = '\0';
        public const char SLASH = '/';
        public const char SHARP = '#';
        public const char TILDE = '~';

        public const string CONDITIONAL = "=<>";
        public const string MATHEMATICAL = "+-*/%";

        public static string NULL_ACTION = END_ARG.ToString();

        public static char[] END_PARSE_ARRAY = { SPACE, END_STATEMENT, END_ARG, END_GROUP, '\n' };

        public static string[] CONDITIONAL_ACTIONS = { "and", "or" };
        public static string[] OPER_ACTIONS = { "+=", "-=", "*=", "/=", "%=", "^=" };
        public static string[] MATH_ACTIONS = { "==", "!=", "<=", ">=", "++", "--",
                                                "%", "*", "/", "+", "-", "^", "<", ">", "&", "="};
        // Actions: always decreasing by the number of characters.
        public static string[] ACTIONS = (OPER_ACTIONS.Union(MATH_ACTIONS)).ToArray();

        public const int MAX_CHARS_TO_SHOW = 45;

        public const NumberStyles FloatStyle = NumberStyles.Float;
        public static CultureInfo Culture { get; private set; }

        static Constants()
        {
            Culture = CultureInfo.CreateSpecificCulture("en-US");
        }
    } 
}
