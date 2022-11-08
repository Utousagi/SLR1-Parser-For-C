using System.Text;
using CCompiler.utils;
using TokenType = CCompiler.GlobalDict.TokenType;

namespace CCompiler.dfa;

public abstract class BaseDfa
{
    protected class DfaState
    {
        public DfaState(int id, bool isFinal, bool hasMore, TokenType? returnType)
        {
            Id = id;
            IsFinal = isFinal;
            HasMore = hasMore;
            ReturnType = returnType;
        }

        public int Id { get; }
        public bool IsFinal { get; }
        public bool HasMore { get; }
        public TokenType? ReturnType { get; }
    }

    protected DfaState State = new (0, false, true, null);
    private readonly StringBuilder _sb = new();
    
    public (string, TokenType) GetToken(MCharEnumerator it)
    {
        State = new DfaState(0, false, true, null);
        _sb.Clear();
        DfaState? latestFinalState = null;
        var latestFinalStateStr = "";
        do
        {
            var c = it.Current;
            if (MoveToNextState(c))
            {
                _sb.Append(c);
                // 记录最后一个合法状态
                if (State.IsFinal)
                {
                    latestFinalState = State;
                    latestFinalStateStr = _sb.ToString();
                    it.Record();
                }

                if (!State.HasMore) break;
            }
            else
            {
                // 无法匹配, 回退到最后一个合法状态
                if (!State.IsFinal)
                {
                    State = latestFinalState ?? throw new Exception("Invalid Format " + _sb);
                    it.Rollback();
                }
                else
                {
                    latestFinalStateStr = _sb.ToString();
                    it.MovePre();
                }

                break;
            }
        } while (it.MoveNext());

        var returnType = State.ReturnType == TokenType.Identifier &&
                          GlobalDict.ReservedWordDict.ContainsKey(latestFinalStateStr)
            ? GlobalDict.ReservedWordDict[latestFinalStateStr]
            : State.ReturnType!;
        if(returnType is TokenType.StringConst or TokenType.CharConst)
        {
            latestFinalStateStr = latestFinalStateStr[1..^1];
        }

        return ((string, TokenType)) (latestFinalStateStr, returnType);
    }

    protected abstract bool MoveToNextState(char c);
}