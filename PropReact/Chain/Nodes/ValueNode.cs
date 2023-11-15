using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

sealed class ValueNodeSource<TSource, TValue> : ChainNode<TValue>, IPropObserver<TValue>, ISourceOnlyChainNode<TSource>
{
    private readonly Func<TSource, IValueProp<TValue>> _getter;
    public ValueNodeSource(Func<TSource, IValueProp<TValue>> getter, IRootNode root) : base(root) => _getter = getter;

    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);

        Root.ChainChanged();
    }

    void ISourceOnlyChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
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