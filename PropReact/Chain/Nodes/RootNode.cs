namespace PropReact.Chain.Nodes;

class RootNode<TRoot> : ChainNode<TRoot>, IDisposable, IRootNode
{
    private readonly TRoot _root;
    private Action? _changeCallback;

    public RootNode(TRoot root) : base(null!) => _root = root;

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

    public void ChainChanged() => _changeCallback?.Invoke();
}

public interface IRootNode
{
    public void ChainChanged();
}