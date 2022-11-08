namespace CCompiler.dfa;

public static class DfaFactory
{
    private static readonly IdentDfa IdentDfa = new();
    private static readonly NumDfa NumberDfa = new();
    private static readonly SymbolDfa SymbolDfa = new();
    private static readonly RemarkAndPreProcessorDfa RemarkAndPreProcessorDfa = new();
    private static readonly StringDfa StringDfa = new();
    
    public enum DfaType
    {
        Ident,
        Number,
        Symbol,
        RemarkOrPreProcessor,
        String
    }
    
    public static BaseDfa GetDfa(DfaType type)
    {
        return type switch
        {
            DfaType.Ident => IdentDfa,
            DfaType.Number => NumberDfa,
            DfaType.Symbol => SymbolDfa,
            DfaType.RemarkOrPreProcessor => RemarkAndPreProcessorDfa,
            DfaType.String => StringDfa,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}