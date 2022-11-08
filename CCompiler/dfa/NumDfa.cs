using CCompiler.utils;
using TokenType = CCompiler.GlobalDict.TokenType;

namespace CCompiler.dfa;

public class NumDfa : BaseDfa
{
    // @formatter:off
    private enum Word : uint
    {
        Zero, One, OtrDigit,
        A, B, C, D, E, F, U, X,
        Dot, Op,
        Otr
    }
    // @formatter:on

    private static Word ToWordType(char c)
    {
        return c switch
        {
            '0' => Word.Zero,
            '1' => Word.One,
            >= '2' and <= '9' => Word.OtrDigit,
            'a' or 'A' => Word.A,
            'b' or 'B' => Word.B,
            'c' or 'C' => Word.C,
            'd' or 'D' => Word.D,
            'e' or 'E' => Word.E,
            'f' or 'F' => Word.F,
            'u' or 'U' => Word.U,
            'x' or 'X' => Word.X,
            '.' => Word.Dot,
            '+' or '-' => Word.Op,
            _ => Word.Otr
        };
    }

    private static readonly Word[] DecDigits = {Word.Zero, Word.One, Word.OtrDigit};

    private static readonly Word[] HexDigits = {Word.Zero, Word.One, Word.OtrDigit, Word.A, Word.B, Word.C, Word.D, Word.E, Word.F};

    private static readonly DfaState State1 = new(1, true, true, TokenType.IntConst);
    private static readonly DfaState State2 = new(2, false, true, null);
    private static readonly DfaState State3 = new(3, true, true, TokenType.IntConst);
    private static readonly DfaState State4 = new(4, false, true, null);
    private static readonly DfaState State5 = new(5, true, true, TokenType.IntConst);
    private static readonly DfaState State6 = new(6, true, true, TokenType.IntConst);
    private static readonly DfaState State7 = new(7, false, true, null);
    private static readonly DfaState State8 = new(8, true, true, TokenType.FloatConst);
    private static readonly DfaState State9 = new(9, false, true, null);
    private static readonly DfaState State10 = new(10, false, true, null);
    private static readonly DfaState State11 = new(11, true, true, TokenType.FloatConst);
    private static readonly DfaState State12 = new(12, true, false, TokenType.IntConst);
    private static readonly DfaState State13 = new(13, true, false, TokenType.FloatConst);

    // @formatter:off
    private static readonly Dictionary<Word, DfaState>[] DfaTable =
    {
        new() {{Word.Zero, State1}, {DecDigits, State6}, {Word.Dot, State7}}, // [0] #
        new() {{Word.B, State2}, {Word.X, State4}, {Word.Zero, State6}, {Word.One, State6}, {Word.OtrDigit, State6}, {Word.Dot, State7}, {Word.E, State9}}, // [1] 0
        new() {{HexDigits, State3}}, // [2] 0x
        new() {{HexDigits, State3}, {Word.U, State12}}, // [3] 0x*
        new() {{Word.Zero, State5}, {Word.One, State5}}, // [4] 0b
        new() {{Word.Zero, State5}, {Word.One, State5}, {Word.U, State12}}, // [5] 0b*
        new() {{DecDigits, State6}, {Word.Dot, State7}, {Word.E, State9}, {Word.U, State12}, {Word.F, State13}, {Word.D, State13}}, // [6] *
        new() {{DecDigits, State8}}, // [7] *.
        new() {{DecDigits, State8}, {Word.E, State9}, {Word.U, State12}, {Word.F, State13}, {Word.D, State13}}, // [8] *.*
        new() {{Word.Op, State10}, {DecDigits, State11}}, // [9] [*,*.*]e
        new() {{DecDigits, State11}}, // [10] [*,*.*]e[+,-]
        new() {{DecDigits, State11}, {Word.F, State13}, {Word.D, State13}}, // [11] [*,*.*]e[+,-]*
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