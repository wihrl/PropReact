using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using PropReact.Chain.Nodes;

namespace PropReact.Chain;

public static class Watch
{
    public static ChainBuilder<TRoot, RootBranch, TRoot> From<TRoot>(
        [NotNull] TRoot root,
        [CallerArgumentExpression(nameof(root))]
        string? rootExpression = null) where TRoot : notnull
    {
#if DEBUG
        if (rootExpression != "this")
            throw new ArgumentException("Observing must always start in the local object. Pass `this` as root.",
                nameof(root));
#endif

        var rootNode = new RootNode<TRoot>(root);
        return new(rootNode, rootNode);
    }
}