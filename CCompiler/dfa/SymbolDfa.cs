using TokenType = CCompiler.GlobalDict.TokenType;
namespace CCompiler.dfa;

public class SymbolDfa : BaseDfa
{
    // @formatter:off
    private enum Word
    {
        Plus, Minus, Multiply, Divide, Equal, Not,
        Greater, Less, Mod, BitAnd, BitOr, BitXor, BitNot,
        Comma, Semicolon, Colon, Dot, LeftParenthesis, Sharp, Question,
        RightParenthesis, LeftBrace, RightBrace, LeftBracket, RightBracket,
        Otr
    }
    // @formatter:on

    private static Word ToWordType(char c)
    {
        return c switch
        {
            '+' => Word.Plus,
            '-' => Word.Minus,
            '*' => Word.Multiply,
            '/' => Word.Divide,
            '=' => Word.Equal,
            '!' => Word.Not,
            '<' => Word.Less,
            '>' => Word.Greater,
            '&' => Word.BitAnd,
            '|' => Word.BitOr,
            '^' => Word.BitXor,
            '~' => Word.BitNot,
            '%' => Word.Mod,
            ',' => Word.Comma,
            ';' => Word.Semicolon,
            ':' => Word.Colon,
            '.' => Word.Dot,
            '#' => Word.Sharp,
            '?' => Word.Question,
            '(' => Word.LeftParenthesis,
            ')' => Word.RightParenthesis,
            '{' => Word.LeftBrace,
            '}' => Word.RightBrace,
            '[' => Word.LeftBracket,
            ']' => Word.RightBracket,
            _ => Word.Otr
        };
    }

    private static readonly DfaState State1 = new(1, true, true, TokenType.Plus);
    private static readonly DfaState State2 = new(2, true, true, TokenType.Minus);
    private static readonly DfaState State3 = new(3, true, true, TokenType.Multiply);
    private static readonly DfaState State4 = new(4, true, true, TokenType.Divide);
    private static readonly DfaState State5 = new(5, true, true, TokenType.Mod);
    private static readonly DfaState State6 = new(6, true, true, TokenType.Assign);
    private static readonly DfaState State7 = new(7, true, true, TokenType.Less);
    private static readonly DfaState State8 = new(8, true, true, TokenType.Greater);
    private static readonly DfaState State9 = new(9, true, true, TokenType.Not);
    private static readonly DfaState State10 = new(10, true, true, TokenType.BitAnd);
    private static readonly DfaState State11 = new(11, true, true, TokenType.BitOr);
    private static readonly DfaState State12 = new(12, true, true, TokenType.BitXor);
    private static readonly DfaState State13 = new(13, true, true, TokenType.LeftShift);
    private static readonly DfaState State14 = new(14, true, true, TokenType.RightShift);
    
    private static readonly DfaState State15 = new(13, true, false, TokenType.BitNot);
    private static readonly DfaState State16 = new(14, true, false, TokenType.Comma);
    private static readonly DfaState State17 = new(15, true, false, TokenType.Semicolon);
    private static readonly DfaState State18 = new(16, true, false, TokenType.Colon);
    private static readonly DfaState State19 = new(17, true, false, TokenType.Dot);
    private static readonly DfaState State20 = new(18, true, false, TokenType.LeftParenthesis);
    private static readonly DfaState State21 = new(19, true, false, TokenType.RightParenthesis);
    private static readonly DfaState State22 = new(20, true, false, TokenType.LeftBrace);
    private static readonly DfaState State23 = new(21, true, false, TokenType.RightBrace);
    private static readonly DfaState State24 = new(22, true, false, TokenType.LeftBracket);
    private static readonly DfaState State25 = new(23, true, false, TokenType.RightBracket);
    private static readonly DfaState State26 = new(24, true, false, TokenType.Sharp);
    private static readonly DfaState State27 = new(25, true, false, TokenType.Question);
    
    private static readonly DfaState State28 = new(26, true, false, TokenType.PlusPlus);
    private static readonly DfaState State29 = new(27, true, false, TokenType.PlusAssign);
    private static readonly DfaState State30 = new(28, true, false, TokenType.MinusMinus);
    private static readonly DfaState State31 = new(29, true, false, TokenType.Arrow);
    private static readonly DfaState State32 = new(30, true, false, TokenType.MinusAssign);
    private static readonly DfaState State33 = new(31, true, false, TokenType.MultiplyAssign);
    private static readonly DfaState State34 = new(32, true, false, TokenType.DivideAssign);
    private static readonly DfaState State35 = new(33, true, false, TokenType.ModAssign);
    private static readonly DfaState State36 = new(34, true, false, TokenType.Equal);
    private static readonly DfaState State37 = new(36, true, false, TokenType.LessEqual);
    private static readonly DfaState State38 = new(38, true, false, TokenType.GreaterEqual);
    private static readonly DfaState State39 = new(39, true, false, TokenType.NotEqual);
    private static readonly DfaState State40 = new(40, true, false, TokenType.And);
    private static readonly DfaState State41 = new(41, true, false, TokenType.BitAndAssign);
    private static readonly DfaState State42 = new(42, true, false, TokenType.Or);
    private static readonly DfaState State43 = new(43, true, false, TokenType.BitOrAssign);
    private static readonly DfaState State44 = new(44, true, false, TokenType.BitXorAssign);
    private static readonly DfaState State45 = new(45, true, false, TokenType.LeftShiftAssign);
    private static readonly DfaState State46 = new(46, true, false, TokenType.RightShiftAssign);

    // @formatter:off
    private static readonly Dictionary<Word, DfaState>[] DfaTable =
    {
        new() {{Word.Plus, State1}, {Word.Minus, State2}, {Word.Multiply, State3}, {Word.Divide, State4}, {Word.Mod, State5},
            {Word.Equal, State6}, {Word.Less, State7}, {Word.Greater, State8}, {Word.Not, State9}, {Word.BitAnd, State10},
            {Word.BitOr, State11}, {Word.BitXor, State12}, {Word.BitNot, State15}, {Word.Comma, State16}, {Word.Semicolon, State17},
            {Word.Colon, State18}, {Word.Dot, State19}, {Word.LeftParenthesis, State20}, {Word.RightParenthesis, State21},
            {Word.LeftBrace, State22}, {Word.RightBrace, State23}, {Word.LeftBracket, State24}, {Word.RightBracket, State25},
            {Word.Sharp, State26}, {Word.Question, State27}}, // [0] #
        new() {{Word.Plus, State28}, {Word.Equal, State29}}, // [1] +
        new() {{Word.Minus, State30}, {Word.Greater, State31}, {Word.Equal, State32}}, // [2] -
        new() {{Word.Equal, State33}}, // [3] *
        new() {{Word.Equal, State34}}, // [4] /
        new() {{Word.Equal, State35}}, // [5] %
        new() {{Word.Equal, State36}}, // [6] =
        new() {{Word.Less, State13}, {Word.Equal, State37}}, // [7] <
        new() {{Word.Greater, State14}, {Word.Equal, State38}}, // [8] >
        new() {{Word.Equal, State39}}, // [9] !
        new() {{Word.BitAnd, State40}, {Word.Equal, State41}}, // [10] &
        new() {{Word.BitOr, State42}, {Word.Equal, State43}}, // [11] |
        new() {{Word.Equal, State44}}, // [12] ^
        new() {{Word.Equal, State45}}, // [13] <<
        new() {{Word.Equal, State46}}, // [14] >>
    };
    // @formatter:on

    protected override bool MoveToNextState(char c)
    {
        var word = ToWordType(c);
        if (DfaTable[State.Id].ContainsKey(word))
        {
            State = DfaTable[State.Id][word];
            return true;
        }

        return false;
    }
}