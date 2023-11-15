using System.Collections;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

abstract class ChainNode<TValue>
{
    protected readonly IRootNode Root;
    protected ChainNode(IRootNode root) => Root = root;

    protected readonly List<ISourceOnlyChainNode<TValue>> Next = new();
    internal void Chain(ISourceOnlyChainNode<TValue> next) => Next.Add(next);
}


interface ISourceOnlyChainNode<TSource>
{
    void ChangeSource(TSource? oldValue, TSource? newValue);
}