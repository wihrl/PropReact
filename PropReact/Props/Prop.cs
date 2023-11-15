using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using PropReact.Chain;
using PropReact.Chain.Nodes;
using PropReact.Props.Value;

namespace PropReact.Props;

public static class Prop
{
    public static ChainBuilder<TRoot, RootBranch, TRoot> Watch<TRoot>(
        [NotNull] TRoot root,
        [CallerArgumentExpression(nameof(root))]
        string? rootExpression = null) where TRoot : notnull
    {
#if DEBUG
        if (rootExpression != "this")
            throw new ArgumentException("Observing must always start in the local object. Pass `this` as root.",
                nameof(root));
#endif

        var rootNode = new RootNodeSource<TRoot>(root);
        return new(rootNode, rootNode);
    }
}

