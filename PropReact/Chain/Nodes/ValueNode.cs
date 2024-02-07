using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

sealed class ValueNode<TSource, TValue>(Func<TSource, IValue<TValue>> getter, IRootNode root)
    : ChainNode<TValue>(root), IPropObserver<TValue>, INotifiableChainNode<TSource>
{
    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);

        Root.ChainChanged();
    }

    void INotifiableChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        // todo: test nullability of API

        var oldValue = oldSource is null ? null : getter(oldSource);
        var newValue = newSource is null ? null : getter(newSource);

        oldValue?.StopWatching(this);
        newValue?.Watch(this);

        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue is null ? default : oldValue.Value,
                newValue is null ? default : newValue.Value);
    }
}