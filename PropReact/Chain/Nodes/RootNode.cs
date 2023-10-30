namespace PropReact.Chain.Nodes;

public class RootNode<TRoot> : ChainNodeBase<TRoot>, IDisposable
{
    private readonly TRoot _root;

    public RootNode(TRoot root, Reaction reaction) : base(reaction) => _root = root;

    public RootNode<TRoot> Initialize()
    {
        foreach (var chainNode in Next) chainNode.ChangeSource(default, _root);
        return this;
    }

    public void Dispose()
    {
        foreach (var chainNode in Next) chainNode.ChangeSource(_root, default);
    }
}