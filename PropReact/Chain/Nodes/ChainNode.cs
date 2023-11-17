using System.Collections;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

abstract class ChainNode<TValue>
{
    protected readonly IRootNode Root;
    protected ChainNode(IRootNode root) => Root = root;

    protected readonly List<INotifiableChainNode<TValue>> Next = new();
    internal void Chain(INotifiableChainNode<TValue> next) => Next.Add(next);
}


interface INotifiableChainNode<TSource>
{
    void ChangeSource(TSource? oldSource, TSource? newSource);
}