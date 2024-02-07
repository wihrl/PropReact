using PropReact.Props;

namespace PropReact.Chain.Nodes;

class CollectionNode<TSet, TValue>(IRootNode root) : ChainNode<TValue>(root), INotifiableChainNode<TSet>, IPropObserver<TValue>
    where TSet : class, IEnumerable<TValue>
{
    // todo: same in all nodes, put to base
    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        PropagateChange(oldValue, newValue);
        Root.ChainChanged();
    }

    void PropagateChange(TValue? oldValue, TValue? newValue)
    {
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);
    }

    void INotifiableChainNode<TSet>.ChangeSource(TSet? oldSource, TSet? newSource)
    {
        var oldProp = oldSource as IProp<TValue>;
        var newProp = newSource as IProp<TValue>;

        // resubscribe if set is a prop
        oldProp?.StopWatching(this);
        newProp?.Watch(this);

        // unsubscribe from all previous set items
        if (oldSource is not null)
            foreach (var oldItem in oldSource)
                PropagateChange(oldItem, default);

        // subscribe to all new set items
        if (newSource is not null)
            foreach (var newItem in newSource)
                PropagateChange(default, newItem);
    }
}