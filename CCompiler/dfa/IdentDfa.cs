using CCompiler.utils;
using TokenType = CCompiler.GlobalDict.TokenType;

namespace CCompiler.dfa;

public class IdentDfa : BaseDfa
{
    private enum Word : uint
    {
        Letter,
        Digit,
        Otr
    }

    private static Word ToWordType(char c)
    {
        return StringUtil.IsIdentPre(c) ? Word.Letter : StringUtil.IsDigit(c) ? Word.Digit : Word.Otr;
    }

    private static readonly DfaState State1 = new(1, true, true, TokenType.Identifier);

    // @formatter:off
    private static readonly Dictionary<Word, DfaState>[] DfaTable =
    {
        new() {{Word.Letter, State1}},
        new() {{Word.Letter, State1}, {Word.Digit, State1}}
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