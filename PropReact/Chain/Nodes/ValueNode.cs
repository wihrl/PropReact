using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

sealed class ValueNode<TSource, TValue> : ChainNode<TValue>, IPropObserver<TValue>, INotifiableChainNode<TSource>
{
    private readonly Func<TSource, IValue<TValue>> _getter;
    public ValueNode(Func<TSource, IValue<TValue>> getter, IRootNode root) : base(root) => _getter = getter;

    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);

        Root.ChainChanged();
    }

    void INotifiableChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        // todo: test nullability of API

        var oldValue = oldSource is null ? null : _getter(oldSource);
        var newValue = newSource is null ? null : _getter(newSource);

        oldValue?.StopWatching(this);
        newValue?.Watch(this);

        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue is null ? default : oldValue.Value,
                newValue is null ? default : newValue.Value);
    }
}