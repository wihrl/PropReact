using PropReact.Properties;
using PropReact.Props;
using PropReact.Reactivity;

namespace PropReact.Chain;

public abstract class ChainNode<TValue> : ChainNode
{
    public ValueNode<TValue, TNext> ChainSingle<TNext>(Func<TValue, IValueProp<TNext>> selector)
    {
        var node = new ValueNode<TValue, TNext>(selector);
        _next.Add(node);
        return node;
    }

    public SetNode<TValue, IEnumerable<TNext>, TNext> ChainSingle<TNext>(Func<TValue, IEnumerable<TNext>> selector)
    {
        var node = new SetNode<TValue, IEnumerable<TNext>, TNext>(selector);
        _next.Add(node);
        return node;
    }

    public SetNode<TValue, ISetProp<TNext>, TNext> ChainSingle<TNext>(Func<TValue, ISetProp<TNext>> selector)
    {
        var node = new SetNode<TValue, ISetProp<TNext>, TNext>(selector);
        _next.Add(node);
        return node;
    }

    protected readonly List<IChainNode<TValue>> _next = new();
}

public abstract class ChainNode
{
}

public interface IChainNode<TSource>
{
    void ChangeSource(TSource? oldValue, TSource? newValue, IReadOnlyCollection<Action> reactions);
}

public static class SetNodeExtensions
{
    public static void Branch<TProp>(this TProp self, params Action<TProp>[] branches) where TProp : ChainNode
    {
        foreach (var branch in branches) branch(self);
    }
}