using PropReact.Chain.Nodes;

namespace PropReact.Chain;

public class ReactiveChain<TRoot>
{
    private readonly Action<RootNode<TRoot>> _builder;

    public ReactiveChain(Action<IChainBuilder<TRoot>> builder) => _builder = builder;


    public IDisposable Attach(TRoot root, Reaction reaction)
    {
        var rootNode = new RootNode<TRoot>(root, reaction);
        _builder(rootNode);
        return rootNode;
    }
}