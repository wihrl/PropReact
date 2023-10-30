using PropReact.Chain;
using PropReact.Chain.Nodes;

namespace PropReact.Selectors;

public static class SelectorParser
{
    private static readonly Dictionary<string, ChainFactory> _parsedExpressions = new();

    public static void RegisterModule<T>() where T : ISelectorParserModule, new() =>
        new T().CreateFactories(_parsedExpressions);

    internal static RootNode<TRoot> CreateChain<TRoot>(TRoot root, Reaction reaction, string expression)
        where TRoot : notnull
    {
        if (!_parsedExpressions.TryGetValue(expression, out var factory))
            throw new("Cannot parse selector. Make sure all selector modules were registered.");

        return (RootNode<TRoot>) factory(root, reaction);
    }
}

public interface ISelectorParserModule
{
    internal void CreateFactories(Dictionary<string, ChainFactory> dictionary);
}

public delegate object ChainFactory(object root, Reaction reaction);