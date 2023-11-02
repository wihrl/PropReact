namespace PropReact.Chain.Nodes;

public class RootNode<TRoot> : ChainNode<TRoot>, IDisposable, IChainNode<TRoot>, IRootNode
{
    public Reaction? Reaction { get; private set; }
    private readonly TRoot _root;

    public RootNode(TRoot root) : base(null!) => _root = root;

    public void Initialize(Reaction reaction)
    {
        Reaction = reaction;
        foreach (var chainNode in Next) chainNode.ChangeSource(default, _root);
    }

    public void Dispose()
    {
        foreach (var chainNode in Next) chainNode.ChangeSource(_root, default);
    }

    public void ChangeSource(TRoot? oldValue, TRoot? newValue) => throw new NotImplementedException();
    public void Changed() => Reaction?.Invoke();
}

public interface IRootNode
{
    public void Changed();
}