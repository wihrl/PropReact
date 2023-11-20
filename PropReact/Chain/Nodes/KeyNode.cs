using PropReact.Props;
using PropReact.Props.Collections;

namespace PropReact.Chain.Nodes;

class KeyNode<TSet, TValue, TKey> : ChainNode<TValue>, INotifiableChainNode<TSet>, IPropObserver<TValue>
    where TSet : class, IReactiveCollection<TValue, TKey>
{
    private readonly TKey _key;
    public KeyNode(IRootNode root, TKey key) : base(root) => _key = key;

    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);

        Root.ChainChanged();
    }

    void INotifiableChainNode<TSet>.ChangeSource(TSet? oldSource, TSet? newSource)
    {
        var oldItem = oldSource is null ? default : oldSource.StopWatchingAt(this, _key);
        var newItem = newSource is null ? default : newSource.WatchAt(this, _key);
        
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldItem, newItem);
    }
}