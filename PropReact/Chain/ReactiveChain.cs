namespace PropReact.Chain;

public class ReactiveChain<TRoot>
{
    private readonly Action<Root<TRoot>> _builder;

    public ReactiveChain(Action<IChainBuilder<TRoot>> builder) => _builder = builder;


    public IDisposable Attach(TRoot root, Reaction reaction)
    {
        var rootNode = new Root<TRoot>(root, reaction);
        _builder(rootNode);
        return rootNode;
    }
}