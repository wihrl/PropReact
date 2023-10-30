using PropReact.Props;

namespace PropReact.Chain.Nodes;

public interface ICollectionChainBuilder<TSet, TValue> : IChainBuilder<TSet>
{
    IChainBuilder<TValue> Enter();
}

public class CollectionNode<TSource, TSet, TValue> : ChainNodeBase<TSet>, IChainNode<TSource>, IPropObserver<TValue>,
    ICollectionChainBuilder<TSet, TValue> where TSet : class, IEnumerable<TValue>
{
    private readonly InnerSet _innerProxy;
    private readonly Func<TSource, TSet> _getter;

    public CollectionNode(Func<TSource, TSet> getter, Reaction reaction) : base(reaction)
    {
        _getter = getter;
        _innerProxy = new(reaction);
    }
    
    public IChainBuilder<TValue> Enter() => _innerProxy;

    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        // an item was added or removed from the set
        _innerProxy.PropagateChange(oldValue, newValue);
        Reaction();
    }

    void IChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        var oldValue = oldSource is null ? null : _getter(oldSource);
        var newValue = newSource is null ? null : _getter(newSource);

        var oldProp = oldValue as IProp<TValue>;
        var newProp = newValue as IProp<TValue>;

        // resubscribe if set is a prop
        oldProp?.StopWatching(this);
        newProp?.Watch(this);

        // notify following nodes without entering (for example .Count)
        foreach (var chainNode in _next)
            chainNode.ChangeSource(oldValue, newValue);

        // unsubscribe from all previous set items
        if (oldValue is not null)
            foreach (var oldItem in oldValue)
                PropChanged(oldItem, default);

        // subscribe to all new set items
        if (newValue is not null)
            foreach (var newItem in newValue)
                PropChanged(default, newItem);
    }

    class InnerSet : ChainNodeBase<TValue>
    {
        internal void PropagateChange(TValue? oldValue, TValue? newValue)
        {
            foreach (var chainNode in _next)
                chainNode.ChangeSource(oldValue, newValue);
        }

        public InnerSet(Reaction reaction) : base(reaction)
        {
        }
    }
}