using CCompiler.utils;
using TokenType = CCompiler.GlobalDict.TokenType;
namespace CCompiler.dfa;

public class RemarkAndPreProcessorDfa : BaseDfa
{
    // @formatter:off
    private enum Word
    {
        Slash, Star, NewLine, Sharp,
        Otr
    }
    // @formatter:on

    private static Word ToWordType(char c)
    {
        return c switch
        {
            '/' => Word.Slash,
            '*' => Word.Star,
            '#' => Word.Sharp,
            '\n' => Word.NewLine,
            _ => Word.Otr
        };
    }

    private static readonly Word[] AllWords = Enum.GetValues(typeof(Word)).Cast<Word>().ToArray();
    
    private static readonly DfaState State1 = new(1, false, true, null);
    private static readonly DfaState State2 = new(2, false, true, null);
    private static readonly DfaState State3 = new(3, false, true, null);
    private static readonly DfaState State4 = new(4, false, true, null);
    private static readonly DfaState State5 = new(5, true, false, TokenType.Ignored);
    
    // @formatter:off
    private static readonly Dictionary<Word, DfaState>[] DfaTable =
    {
        new() {{Word.Slash, State1}, {Word.Sharp, State2}}, // [0] #
        new() {{Word.Slash, State2}, {Word.Star, State3}}, // [1] /
        new() {{Word.NewLine, State5}, {AllWords, State2}}, // [2] // #
        new() {{Word.Star, State4}, {AllWords, State3}}, // [3] /*
        new() {{Word.Star, State4}, {Word.Slash, State5}, {AllWords, State3}}, // [4] /*
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