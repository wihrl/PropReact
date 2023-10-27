namespace PropReact.Chain;

public sealed class ValueNode<TSource, TValue> : ChainNode<TSource, TValue>, IPropObserver<TValue>, IChainNode<TSource>
{
    private readonly Func<TSource, IValueProp<TValue>> _getter;
    public ValueNode(Func<TSource, IValueProp<TValue>> getter) => _getter = getter;

    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        throw new NotImplementedException();
    }

    void IChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        // todo: test nullability of API

        var oldValue = oldSource is null ? null : _getter(oldSource);
        var newValue = newSource is null ? null : _getter(newSource);

        oldValue?.Unsub(this);
        newValue?.Sub(this);

        foreach (var chainNode in _next)
            chainNode.ChangeSource(oldValue is null ? default : oldValue.Value, newValue is null ? default : newValue.Value);
    }
}