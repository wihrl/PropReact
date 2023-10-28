namespace PropReact.Chain;

public sealed class ValueNode<TSource, TValue> : ChainNode<TValue>, IPropObserver<TValue>, IChainNode<TSource>
{
    private readonly Func<TSource, IValueProp<TValue>> _getter;
    public ValueNode(Func<TSource, IValueProp<TValue>> getter) => _getter = getter;

    public void PropChanged(TValue? oldValue, TValue? newValue, IReadOnlyCollection<Action> reactions)
    {
        foreach (var chainNode in _next)
            chainNode.ChangeSource(oldValue, newValue, reactions);
    }

    void IChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource, IReadOnlyCollection<Action> reactions)
    {
        // todo: test nullability of API

        var oldValue = oldSource is null ? null : _getter(oldSource);
        var newValue = newSource is null ? null : _getter(newSource);

        oldValue?.Unsub(this, reactions);
        newValue?.Sub(this, reactions);

        foreach (var chainNode in _next)
            chainNode.ChangeSource(oldValue is null ? default : oldValue.Value, newValue is null ? default : newValue.Value, reactions);
    }
}