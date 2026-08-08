public static class ScriptSyntaxData
{
    //Colors
    public const string KEYWORD_COLOR = "569CD0";
    public const string TYPE_COLOR = "4CC8B0";
    public const string FUNCTION_COLOR = "DCDCAA";
    public const string PARAMETER_COLOR = "9CDCFE";
    public const string COMMENT_COLOR = "57A64A";
    public const string STRING_COLOR = "01D68F";
}

public static class Keywords
{
    public const string
        IF          = "if",
        ELSE        = "else",
        ELSEIF      = "elif",
        WHILE       = "while",
        BREAK       = "break",
        CONTINUE    = "continue",
        RETURN      = "return",
        FUNC        = "func",
        END         = "end",
        UPDATE      = "update",
        LATE_UPDATE = "late_update",
        BOOL        = "bool",
        FLOAT       = "num",
        STRING      = "string",
        AND         = "and",
        OR          = "or",
        TRUE        = "true",
        FALSE       = "false",
        EXTEND      = "extends",
        EXPORT      = "export",
        BASE        = "base",
        GLOBAL      = "global";

    public static readonly string[] All =
    {
        IF, ELSE, ELSEIF, WHILE, BREAK, CONTINUE, RETURN, FUNC, END, UPDATE, LATE_UPDATE,
        BOOL, FLOAT, STRING, AND, OR, TRUE, FALSE, EXTEND, EXPORT, BASE, GLOBAL
    };
}
public static class Types
{
    public const string
        OBJECT      = "object",
        TRANSFORM   = "transform",
        SPRITE      = "sprite",
        SOUND       = "sound",
        SCRIPT      = "script";

    public static readonly string[] All =
    {
        OBJECT, TRANSFORM, SPRITE, SOUND, SCRIPT
    };
}
public static class Functions
{
    public const string
        FLOOR       = "int",
        ROUND       = "round",
        RAND        = "rand",
        POS         = "pos",
        WPOS        = "wpos",
        SCALE       = "scale",
        COLOR       = "color",
        CLONE       = "clone",
        DESTROY     = "destroy",
        SIN         = "sin",
        COS         = "cos",
        LOG         = "log",
        CLAMP       = "clamp",
        CLAMP01     = "clamp01",
        LERP        = "lerp",
        PLAY        = "play",
        STOP        = "stop",
        PLAYSHOT    = "playshot";
    
    public static readonly string[] All =
    {
        FLOOR, ROUND, RAND, POS, WPOS, SCALE, COLOR, CLONE, DESTROY, SIN, COS, LOG, CLAMP, CLAMP01, LERP,
        PLAY, STOP, PLAYSHOT
    };
}
public static class Parameters
{
    public const string
        OBJ         = "obj",
        TRS         = "trs",
        SPR         = "spr",
        SFX         = "sfx",
        PX          = "px",
        PY          = "py",
        PZ          = "pz",
        WPX         = "wpx",
        WPY         = "wpy",
        WPZ         = "wpz",
        SX          = "sx",
        SY          = "sy",
        RZ          = "rz",
        DTIME       = "dtime",
        CR          = "cr",
        CG          = "cg",
        CB          = "cb",
        CA          = "ca",
        LYR         = "lyr",
        IMG         = "img",
        FLX         = "flx",
        FLY         = "fly",
        ADDITIVE    = "additive",
        JOYX        = "joyx",
        JOYY        = "joyy",
        XBTN        = "xbtn",
        YBTN        = "ybtn",
        ZBTN        = "zbtn",
        MBTN        = "mbtn",
        CAMX        = "camx",
        CAMY        = "camy",
        CAMZ        = "camz",
        CAMSIZE     = "camsize",
        DEG2RAD     = "d2r",
        RAD2DEG     = "r2d",
        CLIP        = "clip",
        PLAYONSTART = "playonstart",
        LOOP        = "loop",
        VOLUME      = "volume",
        PITCH       = "pitch",
        ACTIVE      = "active";

    public static readonly string[] All =
    {
        OBJ, TRS, SPR, SFX, PX, PY, PZ, WPX, WPY, WPZ, SX, SY, RZ, DTIME, CR, CG, CB, CA, LYR, IMG, FLX, FLY, ADDITIVE, JOYX, JOYY,
        XBTN, YBTN, ZBTN, MBTN, CAMX, CAMY, CAMZ, CAMSIZE, DEG2RAD, RAD2DEG, CLIP, PLAYONSTART, LOOP, VOLUME, PITCH, ACTIVE
    };
}
public static class HiddenParameters
{
    public const string
        COM         = "COM",
        OBJ         = "OBJ",
        SCRDATA     = "SCRDATA",
        METAID      = "METAID",
        UPDATEID    = "UPDATEID",
        LUPDATEID   = "LUPDATEID",
        AUDIO       = "AUDIO",
        ANDOR       = "ANDOR";

    public static readonly string[] All =
    {
        COM, OBJ, SCRDATA, METAID, UPDATEID, LUPDATEID, AUDIO, ANDOR
    };
}
