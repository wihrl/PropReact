﻿using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

public interface IChainBuilder<TValue> : IChainBuilder
{
    IChainBuilder<TNext> ChainValue<TNext>(Func<TValue, IValueProp<TNext>> selector);
    IChainBuilderSet<IEnumerable<TNext>, TNext> ChainSet<TNext>(Func<TValue, IEnumerable<TNext>> selector);
}

public interface IChainBuilder // used for extension methods
{
}

public abstract class ChainNodeBase<TValue> : ChainNode, IChainBuilder<TValue>
{
    protected readonly Reaction Reaction;

    protected ChainNodeBase(Reaction reaction)
    {
        Reaction = reaction;
    }

    IChainBuilder<TNext> IChainBuilder<TValue>.ChainValue<TNext>(Func<TValue, IValueProp<TNext>> selector)
    {
        var node = new ValueNode<TValue, TNext>(selector, Reaction);
        _next.Add(node);
        return node;
    }

    IChainBuilderSet<IEnumerable<TNext>, TNext> IChainBuilder<TValue>.ChainSet<TNext>(
        Func<TValue, IEnumerable<TNext>> selector)
    {
        var node = new SetNode<TValue, IEnumerable<TNext>, TNext>(selector, Reaction);
        _next.Add(node);
        return node;
    }

    protected readonly List<IChainNode<TValue>> _next = new();
}

public abstract class ChainNode
{
}

// todo: probably remove
public interface IChainNode<TSource>
{
    void ChangeSource(TSource? oldValue, TSource? newValue);
}

public static class SetNodeExtensions
{
    public static void Branch<TProp>(this TProp self, params Action<TProp>[] branches) where TProp : IChainBuilder
    {
        foreach (var branch in branches) branch(self);
    }
}