namespace CCompiler;

public static class GlobalDict
{
    // @formatter:off
    public enum TokenType
    {
        // 数据类型关键字
        Char, Double, Enum, Float, Int,
        Long, Short, Signed, Struct, Union,
        Unsigned, Void,

        // 控制流程关键字
        For, Do, While, Break, Continue,
        If, Else, Goto, Switch, Case,
        Default, Return,

        // 存储类型关键字
        Auto, Extern, Register, Static,

        // 其他保留字
        // Main, Const, Sizeof, Typedef, Volatile,

        // 符号
        Plus, Minus, Multiply, Divide, Assign,
        Less, Greater, And, Or, Not,
        BitAnd, BitOr, BitXor, BitNot, Mod,
        PlusPlus, MinusMinus, LeftShift, RightShift,
        Equal, NotEqual, LessEqual, GreaterEqual, Arrow,
        PlusAssign, MinusAssign, MultiplyAssign, DivideAssign, ModAssign,
        BitAndAssign, BitOrAssign, BitXorAssign, LeftShiftAssign, RightShiftAssign,
        Comma, Semicolon, Colon, Dot, LeftParenthesis,
        RightParenthesis, LeftBrace, RightBrace, LeftBracket, RightBracket,
        Sharp, Question, SingleQuote, DoubleQuote, Backslash,

        // 常量
        IntConst, FloatConst, CharConst, StringConst,

        // 标识符
        Identifier, Ignored,
        
        // 特殊终结符
        Empty, End,
        
        // 非终结符
        Program, Segment, FuncDef, BraceBlock, Block, Exp, FuncCall, DataType, UDataType, FormalParam, ActualParam, CalExpr, BoolExpr,
        AssignOp, CalOp, BoolOp, Value, SelfCalExp, SelfCalOp, IdentifierDef
    }
    

    
    public static readonly Dictionary<string, TokenType> ReservedWordDict = new()
    {
        {"char", TokenType.Char},
        {"double", TokenType.Double},
        {"enum", TokenType.Enum},
        {"float", TokenType.Float},
        {"int", TokenType.Int},
        {"long", TokenType.Long},
        {"short", TokenType.Short},
        {"signed", TokenType.Signed},
        {"struct", TokenType.Struct},
        {"union", TokenType.Union},
        {"unsigned", TokenType.Unsigned},
        {"void", TokenType.Void},
        {"for", TokenType.For},
        {"do", TokenType.Do},
        {"while", TokenType.While},
        {"break", TokenType.Break},
        {"continue", TokenType.Continue},
        {"if", TokenType.If},
        {"else", TokenType.Else},
        {"goto", TokenType.Goto},
        {"switch", TokenType.Switch},
        {"case", TokenType.Case},
        {"default", TokenType.Default},
        {"return", TokenType.Return},
        {"auto", TokenType.Auto},
        {"extern", TokenType.Extern},
        {"register", TokenType.Register},
        {"static", TokenType.Static},
        // {"main", TokenType.Main},
        // {"const", TokenType.Const},
        // {"sizeof", TokenType.Sizeof},
        // {"typedef", TokenType.Typedef},
        // {"volatile", TokenType.Volatile}
    };
    
    public static readonly Dictionary<string, TokenType> SymbolDict = new()
    {
        {"+", TokenType.Plus},
        {"-", TokenType.Minus},
        {"*", TokenType.Multiply},
        {"/", TokenType.Divide},
        {"=", TokenType.Assign},
        {">", TokenType.Greater},
        {"<", TokenType.Less},
        {"&&", TokenType.And},
        {"||", TokenType.Or},
        {"!", TokenType.Not},
        {"&", TokenType.BitAnd},
        {"|", TokenType.BitOr},
        {"^", TokenType.BitXor},
        {"~", TokenType.BitNot},
        {"%", TokenType.Mod},
        {"++", TokenType.PlusPlus},
        {"--", TokenType.MinusMinus},
        {"<<", TokenType.LeftShift},
        {">>", TokenType.RightShift},
        {"==", TokenType.Equal},
        {"!=", TokenType.NotEqual},
        {"<=", TokenType.LessEqual},
        {">=", TokenType.GreaterEqual},
        {"+=", TokenType.PlusAssign},
        {"-=", TokenType.MinusAssign},
        {"*=", TokenType.MultiplyAssign},
        {"/=", TokenType.DivideAssign},
        {"%=", TokenType.ModAssign},
        {"&=", TokenType.BitAndAssign},
        {"|=", TokenType.BitOrAssign},
        {"^=", TokenType.BitXorAssign},
        {"<<=", TokenType.LeftShiftAssign},
        {">>=", TokenType.RightShiftAssign},
        {"->", TokenType.Arrow},
        {",", TokenType.Comma},
        {";", TokenType.Semicolon},
        {":", TokenType.Colon},
        {".", TokenType.Dot},
        {"(", TokenType.LeftParenthesis},
        {")", TokenType.RightParenthesis},
        {"{", TokenType.LeftBrace},
        {"}", TokenType.RightBrace},
        {"[", TokenType.LeftBracket},
        {"]", TokenType.RightBracket},
        {"#", TokenType.Sharp},
        {"?", TokenType.Question},
        {"'", TokenType.SingleQuote},
        {"\"", TokenType.DoubleQuote},
        {"\\", TokenType.Backslash}
    };
    // @formatter:on

}