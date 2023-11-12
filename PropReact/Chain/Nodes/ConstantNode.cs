using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

public sealed class ConstantNodeSource<TSource, TValue> : ChainNode<TValue>, IChainNodeSource<TSource>
{
    private readonly Func<TSource, TValue> _getter;
    public ConstantNodeSource(Func<TSource, TValue> getter, IRootNode root) : base(root) => _getter = getter;

    void IChainNodeSource<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        var oldValue = oldSource is null ? default : _getter(oldSource);
        var newValue = newSource is null ? default : _getter(newSource);

        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);
    }
}