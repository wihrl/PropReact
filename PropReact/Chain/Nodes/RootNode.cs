namespace PropReact.Chain.Nodes;

class RootNodeSource<TRoot> : ChainNode<TRoot>, IDisposable, ISourceOnlyChainNode<TRoot>, IRootNode
{
    private readonly TRoot _root;
    private Action? _changeCallback;

    public RootNodeSource(TRoot root) : base(null!) => _root = root;

    public void Attach(Action reaction)
    {
        if (_changeCallback is not null)
            throw new("Chain is already attached.");
        
        _changeCallback = reaction;
        foreach (var chainNode in Next) chainNode.ChangeSource(default, _root);
    }

    public void Dispose()
    {
        foreach (var chainNode in Next) chainNode.ChangeSource(_root, default);
    }

    void ISourceOnlyChainNode<TRoot>.ChangeSource(TRoot? oldValue, TRoot? newValue) => throw new NotImplementedException();
    public void ChainChanged() => _changeCallback?.Invoke();
}

public interface IRootNode
{
    public void ChainChanged();
}