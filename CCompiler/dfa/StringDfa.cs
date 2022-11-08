using CCompiler.utils;
using TokenType = CCompiler.GlobalDict.TokenType;

namespace CCompiler.dfa;

public class StringDfa : BaseDfa
{
    private enum Word
    {
        SingleQuote,
        DoubleQuote,
        Backslash,
        NewLine,
        Otr
    }
    
    private static Word ToWordType(char c)
    {
        return c switch
        {
            '\'' => Word.SingleQuote,
            '"' => Word.DoubleQuote,
            '\\' => Word.Backslash,
            '\n' => Word.NewLine,
            _ => Word.Otr
        };
    }
    
    private static readonly Word[] AllWords = { Word.SingleQuote, Word.DoubleQuote, Word.Backslash, Word.Otr };

    private static readonly DfaState State1 = new(1, false, true, null);
    private static readonly DfaState State2 = new(2, false, true, null);
    private static readonly DfaState State3 = new(3, false, true, null);
    private static readonly DfaState State4 = new(4, false, true, null);
    private static readonly DfaState State5 = new(5, false, true, null);
    private static readonly DfaState State6 = new(6, true, false, TokenType.CharConst);
    private static readonly DfaState State7 = new(7, true, false, TokenType.StringConst);

    private static readonly Dictionary<Word, DfaState>[] DfaTable =
    {
        new() {{Word.SingleQuote, State1}, {Word.DoubleQuote, State4}}, // #
        new() {{Word.SingleQuote, State6}, {Word.Backslash, State2}, {AllWords, State3}}, // '
        new() {{AllWords, State3}}, // '\
        new() {{Word.SingleQuote, State6}}, // 'w '" '\?
        new() {{Word.DoubleQuote, State7}, {Word.Backslash, State5}, {AllWords, State4}}, // "*
        new() {{AllWords, State4}},
    };

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