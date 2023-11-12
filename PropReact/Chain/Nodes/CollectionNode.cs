using PropReact.Props;

namespace PropReact.Chain.Nodes;

public class CollectionNodeSource<TSet, TValue> : ChainNode<TValue>, IChainNodeSource<TSet>, IPropObserver<TValue>
    where TSet : class, IEnumerable<TValue>
{
    public CollectionNodeSource(IRootNode root) : base(root)
    {
    }

    // todo: same in all nodes, put to base
    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        // an item was added or removed from the set
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);
        
        Root.ChainChanged();
    }

    void IChainNodeSource<TSet>.ChangeSource(TSet? oldSource, TSet? newSource)
    {
        var oldProp = oldSource as IProp<TValue>;
        var newProp = newSource as IProp<TValue>;

        // resubscribe if set is a prop
        oldProp?.StopWatching(this);
        newProp?.Watch(this);

        // unsubscribe from all previous set items
        if (oldSource is not null)
            foreach (var oldItem in oldSource)
                PropChanged(oldItem, default);

        // subscribe to all new set items
        if (newSource is not null)
            foreach (var newItem in newSource)
                PropChanged(default, newItem);
    }
}