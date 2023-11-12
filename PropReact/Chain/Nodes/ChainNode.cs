using System.Collections;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

public abstract class ChainNode<TValue> : IEnumerable<IChainNodeSource<TValue>>
{
    public readonly IRootNode Root;
    protected ChainNode(IRootNode root) => Root = root;

    public List<IChainNodeSource<TValue>> Next { protected get; init; } = new();
    
    public IEnumerator<IChainNodeSource<TValue>> GetEnumerator() => Next.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // todo: remove?
    public void Add(IChainNodeSource<TValue> next) => Next.Add(next);
}

// todo: maybe remove?
public interface IChainNodeSource<TSource>
{
    void ChangeSource(TSource? oldValue, TSource? newValue);
}