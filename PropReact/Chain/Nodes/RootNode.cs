namespace PropReact.Chain.Nodes;

class RootNode<TRoot>(TRoot root) : ChainNode<TRoot>(null!), IDisposable, IRootNode
{
    private Action? _changeCallback;

    public void Attach(Action reaction)
    {
        if (_changeCallback is not null)
            throw new("Chain is already attached.");
        
        _changeCallback = reaction;
        foreach (var chainNode in Next) chainNode.ChangeSource(default, root);
    }

    public void Dispose()
    {
        foreach (var chainNode in Next) chainNode.ChangeSource(root, default);
    }

    public void ChainChanged() => _changeCallback?.Invoke();
}

public interface IRootNode
{
    public void ChainChanged();
}