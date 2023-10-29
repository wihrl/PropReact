namespace PropReact.Chain.Nodes;

public class RootNode<TRoot> : ChainBase<TRoot>, IDisposable
{
    private readonly TRoot _root;

    public RootNode(TRoot root, Reaction reaction) : base(reaction)
    {
        _root = root;
    }

    public void Initialize()
    {
        foreach (var chainNode in _next) chainNode.ChangeSource(default, _root);
    }

    public void Dispose()
    {
        foreach (var chainNode in _next) chainNode.ChangeSource(_root, default);
    }
}