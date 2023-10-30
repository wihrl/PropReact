using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Chain.Nodes;

public interface IChainBuilder<TValue> : IChainBuilder
{
    IChainBuilder<TNext> Chain<TNext>(Func<TValue, IValueProp<TNext>> selector);
    IKeyableChainBuilder<TNext> Chain<TNext, TKey>(Func<TValue, IKeyedCollectionProp<TNext, TKey>> selector);
    ICollectionChainBuilder<IEnumerable<TNext>, TNext> ChainMany<TNext>(Func<TValue, IEnumerable<TNext>> selector);
}

public interface IKeyableChainBuilder<TValue>
{
    IChainBuilder<TNext> ChainAt<TNext, TKey>(Func<TValue, IKeyedCollectionProp<TNext, TKey>> selector);
    IKeyableChainBuilder<TNext> ChainAt<TNext, TKey1, TKey2>(
        Func<TValue, IKeyedCollectionProp<IKeyedCollectionProp<TNext, TKey2>, TKey1>> selector);
}

public interface IChainBuilder // used for extension methods
{
}

public abstract class ChainNodeBase<TValue> : ChainNode, IChainBuilder<TValue>
{
    protected readonly Reaction Reaction;
    protected ChainNodeBase(Reaction reaction) => Reaction = reaction;

    IChainBuilder<TNext> IChainBuilder<TValue>.Chain<TNext>(Func<TValue, IValueProp<TNext>> selector)
    {
        var node = new ValueNode<TValue, TNext>(selector, Reaction);
        _next.Add(node);
        return node;
    }
    
    IKeyableChainBuilder<TNext> IChainBuilder<TValue>.Chain<TNext, TKey>(Func<TValue, IKeyedCollectionProp<TNext, TKey>> selector)
    {
        var node = new ValueNode<IKeyedCollectionProp<TNext, TKey>, TNext>(selector, Reaction);
        _next.Add(node);
        return node;
    }

    ICollectionChainBuilder<IEnumerable<TNext>, TNext> IChainBuilder<TValue>.ChainMany<TNext>(
        Func<TValue, IEnumerable<TNext>> selector)
    {
        var node = new CollectionNode<TValue, IEnumerable<TNext>, TNext>(selector, Reaction);
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