using PropReact.Props;

namespace PropReact.Chain;

public interface IChainBuilder<TValue> : IChainBuilder
{
    IChainBuilder<TNext> ChainSingle<TNext>(Func<TValue, IValueProp<TNext>> selector);
    IChainBuilderSet<IEnumerable<TNext>, TNext> ChainSingle<TNext>(Func<TValue, IEnumerable<TNext>> selector);
    IChainBuilderSet<ISetProp<TNext>, TNext> ChainSingle<TNext>(Func<TValue, ISetProp<TNext>> selector);
}

public interface IChainBuilder
{
}

public abstract class ChainBase<TValue> : ChainNode, IChainBuilder<TValue>
{
    protected readonly Reaction Reaction;

    protected ChainBase(Reaction reaction)
    {
        Reaction = reaction;
    }

    IChainBuilder<TNext> IChainBuilder<TValue>.ChainSingle<TNext>(Func<TValue, IValueProp<TNext>> selector)
    {
        var node = new ValueNode<TValue, TNext>(selector, Reaction);
        _next.Add(node);
        return node;
    }

    IChainBuilderSet<IEnumerable<TNext>, TNext> IChainBuilder<TValue>.ChainSingle<TNext>(
        Func<TValue, IEnumerable<TNext>> selector)
    {
        var node = new SetNode<TValue, IEnumerable<TNext>, TNext>(selector, Reaction);
        _next.Add(node);
        return node;
    }

    IChainBuilderSet<ISetProp<TNext>, TNext> IChainBuilder<TValue>.ChainSingle<TNext>(
        Func<TValue, ISetProp<TNext>> selector)
    {
        var node = new SetNode<TValue, ISetProp<TNext>, TNext>(selector, Reaction);
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