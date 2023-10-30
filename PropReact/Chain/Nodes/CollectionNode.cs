using PropReact.Props;

namespace PropReact.Chain.Nodes;

public class CollectionNode<TSource, TSet, TValue> : ChainNodeBase<TSet>, IChainNode<TSource>, IPropObserver<TValue>
    where TSet : class, IEnumerable<TValue>
{
    public List<IChainNode<TValue>> Inner { get; } = new();
    private readonly Func<TSource, TSet> _getter;

    public CollectionNode(Func<TSource, TSet> getter, Reaction reaction) : base(reaction)
    {
        _getter = getter;
    }

    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        // an item was added or removed from the set
        PropagateChange(oldValue, newValue);
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
        foreach (var chainNode in Next)
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

    void PropagateChange(TValue? oldValue, TValue? newValue)
    {
        foreach (var chainNode in Inner)
            chainNode.ChangeSource(oldValue, newValue);
    }
}