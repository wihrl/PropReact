using PropReact.Props;
using PropReact.Props.Collections;

namespace PropReact.Chain.Nodes;

class KeyNode<TSet, TValue, TKey>(IRootNode root, TKey key) : ChainNode<TValue>(root), INotifiableChainNode<TSet>, IPropObserver<TValue>
    where TSet : class, IReactiveCollection<TValue, TKey>
{
    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);

        Root.ChainChanged();
    }

    void INotifiableChainNode<TSet>.ChangeSource(TSet? oldSource, TSet? newSource)
    {
        var oldItem = oldSource is null ? default : oldSource.StopWatchingAt(this, key);
        var newItem = newSource is null ? default : newSource.WatchAt(this, key);
        
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldItem, newItem);
    }
}