using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

public sealed class ConstantNode<TSource, TValue> : ChainNode<TValue>, IChainNode<TSource>
{
    private readonly Func<TSource, TValue> _getter;
    public ConstantNode(Func<TSource, TValue> getter, IRootNode root) : base(root) => _getter = getter;

    void IChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        var oldValue = oldSource is null ? default : _getter(oldSource);
        var newValue = newSource is null ? default : _getter(newSource);

        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);
    }
}