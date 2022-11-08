using System.CodeDom.Compiler;
using System.Collections;
using System.Text;
using System.Text.Unicode;
using CCompiler.dfa;
using CCompiler.utils;
using TokenType = CCompiler.GlobalDict.TokenType;

namespace CCompiler;

public class SyntaxAnalyzer
{
    // 产生式
    private class Product
    {
        public Product(TokenType left, TokenType[] right)
        {
            Left = left;
            Right = right;
        }

        public TokenType Left { get; }
        public TokenType[] Right { get; }

        public override string ToString()
        {
            return $"{string.Join(" ", Right)} -> {Left}";
        }
    }

    // 项目
    private class Item
    {
        public Item(Product product)
        {
            Product = product;
            Idx = 0;
        }

        public Product Product { get; }
        private int Idx { get; set; }

        public bool MoveNext()
        {
            if (Idx == Product.Right.Length - 1) Idx++;
            if (Idx >= Product.Right.Length - 1) return false;
            Idx++;
            return true;
        }

        public bool IsFinal()
        {
            return Idx == Product.Right.Length;
        }

        public TokenType Current => Product.Right[Idx];

        public override bool Equals(object? obj)
        {
            if (obj is Item item)
                return Equals(item);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Product);
        }

        private bool Equals(Item other)
        {
            return Product.Equals(other.Product) && Idx == other.Idx;
        }

        public Item Copy()
        {
            var item = new Item(Product)
            {
                Idx = Idx
            };
            return item;
        }

        public override string ToString()
        {
            return
                $"{Product.Left} -> {string.Join(" ", Product.Right.Take(Idx))}·{string.Join(" ", Product.Right.Skip(Idx))}";
        }
    }

    private enum State
    {
        S,
        R,
        Acc,
        Error
    }

    private readonly LexicalAnalyzer _lexicalAnalyzer = new();
    private static readonly StringBuilder Sb = new();

    private static class G
    {
        public const TokenType S = TokenType.Program;

        public static readonly TokenType[] V =
        {
            TokenType.Program, TokenType.Segment, TokenType.FuncDef, TokenType.BraceBlock, TokenType.Block,
            TokenType.Exp, TokenType.FuncCall, TokenType.DataType, TokenType.UDataType, TokenType.FormalParam,
            TokenType.ActualParam, TokenType.CalExpr, TokenType.BoolExpr,
            TokenType.AssignOp, TokenType.CalOp, TokenType.BoolOp, TokenType.Value, TokenType.SelfCalExp,
            TokenType.SelfCalOp, TokenType.IdentifierDef
        };

        public static readonly TokenType[] T =
        {
            TokenType.Char, TokenType.Double, TokenType.Enum, TokenType.Float, TokenType.Int,
            TokenType.Long, TokenType.Short, TokenType.Signed, TokenType.Struct, TokenType.Union,
            TokenType.Unsigned, TokenType.Void,
            TokenType.For,
            TokenType.Do, TokenType.While, TokenType.Break, TokenType.Continue,
            TokenType.If, TokenType.Else, TokenType.Goto, TokenType.Switch, TokenType.Case,
            TokenType.Default, TokenType.Return,
            TokenType.Auto,
            TokenType.Extern, TokenType.Register, TokenType.Static,
            TokenType.Plus,
            TokenType.Minus, TokenType.Multiply, TokenType.Divide, TokenType.Assign,
            TokenType.Less, TokenType.Greater, TokenType.And, TokenType.Or, TokenType.Not,
            TokenType.BitAnd, TokenType.BitOr, TokenType.BitXor, TokenType.BitNot, TokenType.Mod,
            TokenType.PlusPlus, TokenType.MinusMinus, TokenType.LeftShift, TokenType.RightShift,
            TokenType.Equal, TokenType.NotEqual, TokenType.LessEqual, TokenType.GreaterEqual, TokenType.Arrow,
            TokenType.PlusAssign, TokenType.MinusAssign, TokenType.MultiplyAssign, TokenType.DivideAssign,
            TokenType.ModAssign,
            TokenType.BitAndAssign, TokenType.BitOrAssign, TokenType.BitXorAssign, TokenType.LeftShiftAssign,
            TokenType.RightShiftAssign,
            TokenType.Comma, TokenType.Semicolon, TokenType.Colon, TokenType.Dot, TokenType.LeftParenthesis,
            TokenType.RightParenthesis, TokenType.LeftBrace, TokenType.RightBrace, TokenType.LeftBracket,
            TokenType.RightBracket,
            TokenType.Question, TokenType.SingleQuote, TokenType.DoubleQuote, TokenType.Backslash,
            TokenType.IntConst,
            TokenType.FloatConst, TokenType.CharConst, TokenType.StringConst,
            TokenType.Identifier, TokenType.Empty
        };

        public static IEnumerable<TokenType> Vt => V.Concat(T);

        // Empty的产生式暂时没做，用替他方式替代
        // @formatter:off
        public static readonly List<Product> P = new()
        {
            new Product(TokenType.Program, new[] {TokenType.Segment}),
            new Product(TokenType.Segment, new[] {TokenType.FuncDef}),
            new Product(TokenType.Segment, new[] {TokenType.DataType, TokenType.IdentifierDef, TokenType.Semicolon}),
            new Product(TokenType.Segment, new[] {TokenType.Segment, TokenType.Segment}),
            new Product(TokenType.FuncDef, new[] {TokenType.DataType, TokenType.Identifier, TokenType.LeftParenthesis, TokenType.RightParenthesis, TokenType.BraceBlock}),
            new Product(TokenType.FuncDef, new[] {TokenType.DataType, TokenType.Identifier, TokenType.LeftParenthesis, TokenType.FormalParam, TokenType.RightParenthesis, TokenType.BraceBlock}),
            // new Product(TokenType.FormalParam, new[] {TokenType.Empty}),
            new Product(TokenType.FormalParam, new[] {TokenType.DataType, TokenType.Identifier}),
            new Product(TokenType.FormalParam, new[] {TokenType.DataType, TokenType.Identifier, TokenType.Comma, TokenType.FormalParam}),

            new Product(TokenType.Block, new[] {TokenType.BraceBlock}),
            new Product(TokenType.Block, new[] {TokenType.Exp, TokenType.Semicolon}),
            new Product(TokenType.Block, new[] {TokenType.Block, TokenType.Block}),
            new Product(TokenType.Block, new[] {TokenType.While, TokenType.LeftParenthesis, TokenType.BoolExpr, TokenType.RightParenthesis, TokenType.Block}),
            new Product(TokenType.Block, new[] {TokenType.Do, TokenType.Block, TokenType.While, TokenType.LeftParenthesis, TokenType.BoolExpr, TokenType.RightParenthesis, TokenType.Semicolon}),
            new Product(TokenType.Block, new[] {TokenType.If, TokenType.LeftParenthesis, TokenType.BoolExpr, TokenType.RightParenthesis, TokenType.Block}),
            new Product(TokenType.Block, new[] {TokenType.If, TokenType.LeftParenthesis, TokenType.BoolExpr, TokenType.RightParenthesis, TokenType.Block, TokenType.Else, TokenType.Block}),
            new Product(TokenType.BraceBlock, new[] {TokenType.LeftBrace, TokenType.RightBrace}),
            new Product(TokenType.BraceBlock, new[] {TokenType.LeftBrace, TokenType.Block, TokenType.RightBrace}),
            new Product(TokenType.Exp, new[] {TokenType.DataType, TokenType.IdentifierDef}),
            new Product(TokenType.Exp, new[] {TokenType.Identifier, TokenType.AssignOp, TokenType.CalExpr}),
            new Product(TokenType.Exp, new[] {TokenType.FuncCall}),
            new Product(TokenType.Exp, new[] {TokenType.Return}),
            new Product(TokenType.Exp, new[] {TokenType.SelfCalExp}),
            new Product(TokenType.Exp, new[] {TokenType.Return, TokenType.CalExpr}),
            
            new Product(TokenType.IdentifierDef, new[] {TokenType.Identifier}),
            new Product(TokenType.IdentifierDef, new[] {TokenType.Identifier, TokenType.Assign, TokenType.CalExpr}),
            new Product(TokenType.IdentifierDef, new[] {TokenType.IdentifierDef, TokenType.Comma, TokenType.IdentifierDef}),

            new Product(TokenType.DataType, new[] {TokenType.Int}),
            new Product(TokenType.DataType, new[] {TokenType.Char}),
            new Product(TokenType.DataType, new[] {TokenType.Float}),
            new Product(TokenType.DataType, new[] {TokenType.Double}),
            new Product(TokenType.DataType, new[] {TokenType.Long}),
            new Product(TokenType.DataType, new[] {TokenType.Long, TokenType.Long}),
            new Product(TokenType.DataType, new[] {TokenType.Short}),
            new Product(TokenType.DataType, new[] {TokenType.Void}),
            new Product(TokenType.DataType, new[] {TokenType.UDataType}),
            new Product(TokenType.UDataType, new[] {TokenType.Unsigned, TokenType.Char}),
            new Product(TokenType.UDataType, new[] {TokenType.Unsigned, TokenType.Int}),
            new Product(TokenType.UDataType, new[] {TokenType.Unsigned, TokenType.Long}),
            new Product(TokenType.UDataType, new[] {TokenType.Unsigned, TokenType.Long, TokenType.Long}),
            new Product(TokenType.UDataType, new[] {TokenType.Unsigned, TokenType.Short}),
            new Product(TokenType.UDataType, new[] {TokenType.Signed, TokenType.Int}),
            new Product(TokenType.UDataType, new[] {TokenType.Signed, TokenType.Char}),
            new Product(TokenType.UDataType, new[] {TokenType.Signed, TokenType.Long}),
            new Product(TokenType.UDataType, new[] {TokenType.Signed, TokenType.Long, TokenType.Long}),
            new Product(TokenType.UDataType, new[] {TokenType.Signed, TokenType.Short}),

            new Product(TokenType.CalExpr, new[] {TokenType.CalExpr, TokenType.CalOp, TokenType.CalExpr}),
            new Product(TokenType.CalExpr, new[] {TokenType.Minus, TokenType.CalExpr}),
            new Product(TokenType.CalExpr, new[] {TokenType.LeftParenthesis, TokenType.CalExpr, TokenType.RightParenthesis}),
            new Product(TokenType.CalExpr, new[] {TokenType.SelfCalExp}),
            new Product(TokenType.CalExpr, new[] {TokenType.Value}),
            new Product(TokenType.CalExpr, new[] {TokenType.FuncCall}),
            // SLR(1) 无法识别三元表达式
            // new Product(TokenType.CalExpr, new[] {TokenType.BoolExpr, TokenType.Question, TokenType.CalExpr, TokenType.Colon, TokenType.CalExpr}),
            new Product(TokenType.SelfCalExp, new[] {TokenType.Identifier}),
            new Product(TokenType.SelfCalExp, new[] {TokenType.Identifier, TokenType.SelfCalOp}),
            new Product(TokenType.SelfCalExp, new[] {TokenType.SelfCalOp, TokenType.Identifier}),

            new Product(TokenType.BoolExpr, new[] {TokenType.CalExpr, TokenType.BoolOp, TokenType.CalExpr}),
            new Product(TokenType.BoolExpr, new[] {TokenType.BoolExpr, TokenType.And, TokenType.BoolExpr}),
            new Product(TokenType.BoolExpr, new[] {TokenType.BoolExpr, TokenType.Or, TokenType.BoolExpr}),
            new Product(TokenType.BoolExpr, new[] {TokenType.Not, TokenType.BoolExpr}),
            new Product(TokenType.BoolExpr, new[] {TokenType.LeftParenthesis, TokenType.BoolExpr, TokenType.RightParenthesis}),

            new Product(TokenType.FuncCall, new[] {TokenType.Identifier, TokenType.LeftParenthesis, TokenType.RightParenthesis}),
            new Product(TokenType.FuncCall, new[] {TokenType.Identifier, TokenType.LeftParenthesis, TokenType.ActualParam, TokenType.RightParenthesis}),
            // new Product(TokenType.ActualParam, new[] {TokenType.Empty}),
            new Product(TokenType.ActualParam, new[] {TokenType.CalExpr}),
            new Product(TokenType.ActualParam, new[] {TokenType.CalExpr, TokenType.Comma, TokenType.ActualParam}),

            new Product(TokenType.Value, new[] {TokenType.IntConst}),
            new Product(TokenType.Value, new[] {TokenType.FloatConst}),
            new Product(TokenType.Value, new[] {TokenType.CharConst}),
            new Product(TokenType.Value, new[] {TokenType.StringConst}),

            new Product(TokenType.AssignOp, new[] {TokenType.Assign}),
            new Product(TokenType.AssignOp, new[] {TokenType.PlusAssign}),
            new Product(TokenType.AssignOp, new[] {TokenType.MinusAssign}),
            new Product(TokenType.AssignOp, new[] {TokenType.MultiplyAssign}),
            new Product(TokenType.AssignOp, new[] {TokenType.DivideAssign}),
            new Product(TokenType.AssignOp, new[] {TokenType.ModAssign}),
            new Product(TokenType.AssignOp, new[] {TokenType.BitAndAssign}),
            new Product(TokenType.AssignOp, new[] {TokenType.BitOrAssign}),
            new Product(TokenType.AssignOp, new[] {TokenType.BitXorAssign}),
            new Product(TokenType.AssignOp, new[] {TokenType.LeftShiftAssign}),
            new Product(TokenType.AssignOp, new[] {TokenType.RightShiftAssign}),
            new Product(TokenType.CalOp, new[] {TokenType.Plus}),
            new Product(TokenType.CalOp, new[] {TokenType.Minus}),
            new Product(TokenType.CalOp, new[] {TokenType.Multiply}),
            new Product(TokenType.CalOp, new[] {TokenType.Divide}),
            new Product(TokenType.CalOp, new[] {TokenType.Mod}),
            new Product(TokenType.CalOp, new[] {TokenType.BitAnd}),
            new Product(TokenType.CalOp, new[] {TokenType.BitOr}),
            new Product(TokenType.CalOp, new[] {TokenType.BitXor}),
            new Product(TokenType.CalOp, new[] {TokenType.LeftShift}),
            new Product(TokenType.CalOp, new[] {TokenType.RightShift}),
            new Product(TokenType.SelfCalOp, new[] {TokenType.PlusPlus}),
            new Product(TokenType.SelfCalOp, new[] {TokenType.MinusMinus}),
            new Product(TokenType.BoolOp, new[] {TokenType.Equal}),
            new Product(TokenType.BoolOp, new[] {TokenType.NotEqual}),
            new Product(TokenType.BoolOp, new[] {TokenType.Greater}),
            new Product(TokenType.BoolOp, new[] {TokenType.GreaterEqual}),
            new Product(TokenType.BoolOp, new[] {TokenType.Less}),
            new Product(TokenType.BoolOp, new[] {TokenType.LessEqual}),
        };
        // @formatter:on
    }

    private static readonly (Dictionary<TokenType, (State s, int t)>[] ACTION, Dictionary<TokenType, int>[] GOTO) Table = GenSlr1Table();

    /// <summary>
    /// 生成SLR(1)分析表
    /// </summary>
    /// <returns></returns>
    private static (Dictionary<TokenType, (State, int)>[] actionTable, Dictionary<TokenType, int>[] gotoTable) GenSlr1Table()
    {
        var follow = GetFollow();
        var c = GetLr0Collection();

        // ACTION表初始化
        var actionRow = G.T.Append(TokenType.End).ToDictionary(t => t, _ => (State.Error, -1));
        var actionTable = Array.Empty<Dictionary<TokenType, (State, int)>>();
        for (var i = 0; i < c.Count; i++)
            actionTable = actionTable.Append(new Dictionary<TokenType, (State, int)>(actionRow)).ToArray();

        // GOTO表初始化
        var gotoRow = G.V.ToDictionary(t => t, _ => -1);
        var gotoTable = Array.Empty<Dictionary<TokenType, int>>();
        for (var i = 0; i < c.Count; i++)
            gotoTable = gotoTable.Append(new Dictionary<TokenType, int>(gotoRow)).ToArray();

        // 生成ACTION表和GOTO表
        for (var k = 0; k < c.Count; k++)
        {
            var I = c[k];
            foreach (var item in I)
            {
                // dot不在最后，需要移进
                if (!item.IsFinal())
                {
                    var t = item.Current;
                    var j = Go(I, t);
                    var to = c.FindIndex(i => i.SetEquals(j));
                    if (G.T.Contains(t))
                        actionTable[k][t] = (State.S, to);
                    else
                        gotoTable[k][t] = to;
                }
                // dot在最后，需要规约
                else
                {
                    if (item.Product.Left == G.S)
                    {
                        actionTable[k][TokenType.End] = (State.Acc, -1);
                        continue;
                    }

                    var to = G.P.IndexOf(item.Product);
                    foreach (var t in G.T.Append(TokenType.End))
                        if (follow[item.Product.Left].Contains(t))
                            actionTable[k][t] = (State.R, to);
                }
            }
        }
        
        File.OpenWrite("../../../info.txt").Write(Encoding.UTF8.GetBytes(Sb.ToString()));

        return (actionTable, gotoTable);
    }

    /// <summary>
    /// 求follow集
    /// </summary>
    /// <returns></returns>
    private static Dictionary<TokenType, ISet<TokenType>> GetFollow()
    {
        var first = GetFirst();
        var follow = new Dictionary<TokenType, ISet<TokenType>>
        {
            // 将结束符加入到S的follow集中
            [G.S] = new HashSet<TokenType> {TokenType.End}
        };

        // 对每个非终结符求follow集
        foreach (var v in G.V)
        {
            follow[v] = UnionFollow(v);
        }

        ISet<TokenType> UnionFollow(TokenType v)
        {
            var f = follow.GetValueOrDefault(v, new HashSet<TokenType>());

            // 找出所有右部含v的产生式
            var product = from p in G.P where p.Right.Contains(v) select p;

            foreach (var p in product)
            {
                // b为右部中在v之后的符号
                var b = p.Right.SkipWhile(x => x != v).Skip(1).ToList();
                // 如果v不是右部的最后一个符号，将v右边第一个符号的first集加入到v的follow集中
                if (b.Any())
                    f.UnionWith(first[b.First()]);
                // 如果v是右部的最后一个符号或v之后的符号均能被最终推导为空，将p的左部的follow集加入到v的follow集中
                if ((!b.Any() || b.TrueForAll(t => first[t].Contains(TokenType.Empty))) && p.Left != v)
                    f.UnionWith(follow.ContainsKey(p.Left) ? follow[p.Left] : UnionFollow(p.Left));
            }

            f.Remove(TokenType.Empty);

            return f;
        }

        Sb.AppendLine("---------------Follow---------------");
        foreach (var (key, value) in follow)
        {
            Sb.AppendLine($"{key}: {string.Join(", ", value)}");
        }

        Sb.AppendLine("-----------------------------------");
        Sb.AppendLine();

        return follow;
    }

    /// <summary>
    /// 求first集
    /// </summary>
    /// <returns></returns>
    private static Dictionary<TokenType, ISet<TokenType>> GetFirst()
    {
        var first = new Dictionary<TokenType, ISet<TokenType>>();

        // 终结符的first集为其自身
        foreach (var x in G.T)
        {
            first[x] = new HashSet<TokenType> {x};
        }

        // 非终结符集
        foreach (var x in G.V)
        {
            first[x] = UnionFirst(x);
        }

        ISet<TokenType> UnionFirst(TokenType x)
        {
            var f = new HashSet<TokenType>();
            // 对每个左部为x的产生式，考察其右部第一个符号（该符号不能为x否则会产生无限递归）
            var right = from p in G.P where p.Left == x && p.Left != p.Right[0] select p.Right;
            foreach (var y in right)
            {
                // 如果是终结符，直接加入
                if (G.T.Contains(y[0]))
                    f.Add(y[0]);
                // 若为非终结符y，且y的first集已经得出，直接加入，否则先求出y的first集再加入
                else
                    f.UnionWith(first.ContainsKey(y[0]) ? first[y[0]] : UnionFirst(y[0]));
            }

            return f;
        }

        
        Sb.AppendLine("---------------First---------------");
        foreach (var (key, value) in first)
        {
            Sb.AppendLine($"{key}: {string.Join(", ", value)}");
        }

        Sb.AppendLine("-----------------------------------");
        Sb.AppendLine();

        return first;
    }

    /// <summary>
    /// 求项目集规范族
    /// </summary>
    /// <returns></returns>
    private static List<ISet<Item>> GetLr0Collection()
    {
        // G已经是拓广文法了
        // 先求出最开始的项目闭包
        var firstI = Closure(new[] {new Item(G.P[0])});
        // 项目集规范族
        var c = new List<ISet<Item>> {firstI};
        // 待处理队列
        var q = new Queue<ISet<Item>>();
        q.Enqueue(firstI);
        while (q.TryDequeue(out var i))
        {
            // 对于当前的项目闭包，求出其接受每个vt中符号后转移的项目闭包
            foreach (var x in G.Vt)
            {
                var j = Go(i, x);
                // 非空且尚未被加入过的闭包才有加入的必要
                if (!j.Any()) continue;
                if (c.Any(set => set.SetEquals(j))) continue;
                c.Add(j);
                q.Enqueue(j);
            }
        }

        Sb.AppendLine("---------------LR(0) Collection---------------");
        for (var i = 0; i < c.Count; i++)
        {
            Sb.AppendLine($"I({i+1})");
            foreach (var item in c[i])
            {
                Sb.AppendLine(item.ToString());
            }
            Sb.AppendLine();
        }
        Sb.AppendLine("---------------------------------------------");
        Sb.AppendLine();
        
        return c;
    }

    private static ISet<Item> Go(IEnumerable<Item> I, TokenType x)
    {
        // r为能够接受x的项目在接受了x之后转移到的项目集合
        var r = new HashSet<Item>();
        foreach (var i in I)
        {
            if (i.IsFinal()) continue;

            var b = i.Current;
            if (b == x && G.Vt.Contains(b))
            {
                var j = i.Copy();
                // dot右移一位之后添加
                j.MoveNext();
                r.Add(j);
            }
        }

        return Closure(r);
    }

    private static ISet<Item> Closure(IEnumerable<Item> I)
    {
        // 闭包中的项目
        var r = new HashSet<Item>();
        // 待处理队列
        var q = new Queue<Item>();
        // 已经加入到闭包中的项目左部
        var added = new HashSet<TokenType>();
        foreach (var item in I) q.Enqueue(item);
        while (q.TryDequeue(out var i))
        {
            // 深拷贝
            r.Add(i.Copy());
            if (i.IsFinal()) continue;
            var t = i.Current;
            // 当前符号为非终结符且尚未加入到闭包中才能继续
            if (!G.V.Contains(t) || added.Contains(t)) continue;
            // 将以当前符号为左部的所有产生式生成的新项目加入到队列（队列中每个项目最终都会被加入闭包）
            foreach (var newI in from p in G.P where p.Left == t select new Item(p))
                q.Enqueue(newI);

            // 标记左部，之后所有左部相同的项目都不会再加入到队列中
            added.Add(t);
        }

        return r;
    }

    public void Process(string txt)
    {
        var sb = new StringBuilder();
        var s = new Stack<(int state, TokenType token)>();
        s.Push((0, TokenType.End));
        
        _lexicalAnalyzer.Process(txt);
        var tokens = _lexicalAnalyzer.GetTokens();
        while (tokens.MoveNext())
        {
            var (_, token) = tokens.Current;
            var (state, _) = s.Peek();
            while (Table.ACTION[state][token].s == State.R)
            {
                var p = G.P[Table.ACTION[state][token].t];
                for (var i = 0; i < p.Right.Length; i++)
                    s.Pop();
                var (top, _) = s.Peek();
                s.Push((Table.GOTO[top][p.Left], p.Left));
                sb.AppendLine($"规约：{p}");
                (_, token) = tokens.Current;
                (state, _) = s.Peek();
            }

            switch (Table.ACTION[state][token].s)
            {
                case State.S:
                {
                    s.Push((Table.ACTION[state][token].t, token));
                    sb.AppendLine($"移进：{token}");
                    break;
                }
                case State.Acc:
                {
                    File.OpenWrite("../../../output.txt").Write(Encoding.UTF8.GetBytes(sb.ToString()));
                    Console.WriteLine("Success");
                    return;
                }
                case State.R:
                case State.Error:
                default:
                    throw new Exception("语法错误");
            }
        }
    }
}