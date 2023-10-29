﻿using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

public sealed class ValueNode<TSource, TValue> : ChainNodeBase<TValue>, IPropObserver<TValue>, IChainNode<TSource>
{
    private readonly Func<TSource, IValueProp<TValue>> _getter;
    public ValueNode(Func<TSource, IValueProp<TValue>> getter, Reaction reaction) : base(reaction) => _getter = getter;

    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        foreach (var chainNode in _next)
            chainNode.ChangeSource(oldValue, newValue);

        Reaction();
    }

    void IChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        // todo: test nullability of API

        var oldValue = oldSource is null ? null : _getter(oldSource);
        var newValue = newSource is null ? null : _getter(newSource);

        oldValue?.StopWatching(this);
        newValue?.Watch(this);

        foreach (var chainNode in _next)
            chainNode.ChangeSource(oldValue is null ? default : oldValue.Value, newValue is null ? default : newValue.Value);
    }
}