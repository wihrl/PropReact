using System.Collections;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

public abstract class ChainNode<TValue> : IEnumerable<IChainNode<TValue>>
{
    public readonly IRootNode Root;
    protected ChainNode(IRootNode root) => Root = root;

    public List<IChainNode<TValue>> Next { protected get; init; } = new();
    
    public IEnumerator<IChainNode<TValue>> GetEnumerator() => Next.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(IChainNode<TValue> next) => Next.Add(next);
}

// todo: maybe remove?
public interface IChainNode<TSource>
{
    void ChangeSource(TSource? oldValue, TSource? newValue);
}