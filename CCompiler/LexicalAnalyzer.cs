using System.Text;
using CCompiler.dfa;
using CCompiler.utils;
using TokenType = CCompiler.GlobalDict.TokenType;
using DfaType = CCompiler.dfa.DfaFactory.DfaType;

namespace CCompiler;

public class LexicalAnalyzer
{
    private readonly List<(string, TokenType)> _tokenList = new(); // 产生的token序列

    private void Init()
    {
        _tokenList.Clear();
    }

    public void Process(string txt)
    {
        Init();
        using var it = txt.GetMEnumerator();
        while (it.MoveNext())
        {
            var c = it.Current;
            // 忽略状态机外的空白字符
            if (c is ' ' or '\t' or '\r' or '\n') continue;

            BaseDfa dfa;
            if (StringUtil.IsIdentPre(c))
            {
                // 读取到a-z A-Z _，为标识符或关键字
                dfa = DfaFactory.GetDfa(DfaType.Ident);
            }

            else if (StringUtil.IsDigit(c))
            {
                // 读取到0-9，为数字
                dfa = DfaFactory.GetDfa(DfaType.Number);
            }

            else switch (c)
            {
                case '#':
                    // 预处理指令，直接忽略（摆烂了
                    dfa = DfaFactory.GetDfa(DfaType.RemarkOrPreProcessor); 
                    break;
                case '.':
                {
                    // 向后看一位，如果是数字则是浮点数，否则是单独的点号
                    it.MoveNext();
                    dfa = DfaFactory.GetDfa(StringUtil.IsDigit(it.Current) ? DfaType.Number : DfaType.Symbol);
                    it.MovePre();
                    break;
                }
                case '/':
                {
                    // 向后看一位，如果是/或*则是注释，否则是除号
                    it.MoveNext();
                    dfa = DfaFactory.GetDfa(it.Current is '/' or '*' ? DfaType.RemarkOrPreProcessor : DfaType.Symbol);
                    it.MovePre();
                    break;
                }
                case '"' or '\'':
                    // 字符串或字符
                    dfa = DfaFactory.GetDfa(DfaType.String);
                    break;
                default:
                    // 其余情况均为符号
                    dfa = DfaFactory.GetDfa(DfaType.Symbol);
                    break;
            }
            
            var (token, type) = dfa.GetToken(it);
            if (type is not TokenType.Ignored)
            {
                _tokenList.Add((token, type));
            }
        }

        var sb = new StringBuilder();
        _tokenList.ForEach(t => sb.AppendLine(t.ToString()));
        File.OpenWrite("../../../tokens.txt").Write(Encoding.UTF8.GetBytes(sb.ToString()));
    }

    public IEnumerator<(string, TokenType)> GetTokens()
    {
        // 在最后加上一个结束符
        return _tokenList.Append(("#", TokenType.End)).GetEnumerator();
    }
}