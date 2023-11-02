using PropReact.Props;
using PropReact.Props.Collections;

namespace PropReact.Chain.Nodes;

public sealed class KeyedNode<TSource, TValue, TKey> : ChainNode<TValue>, IPropObserver<TValue>, IChainNode<TSource>
    where TKey : notnull
{
    private readonly TKey _key;
    private readonly Func<TSource, IKeyedCollectionProp<TValue, TKey>> _getter;
    public KeyedNode(Func<TSource, IKeyedCollectionProp<TValue, TKey>> getter, IRootNode root, TKey key) : base(root)
    {
        _key = key;
        _getter = getter;
    }

    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);

        Root.Changed();
    }

    void IChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        var oldValue = oldSource is null ? null : _getter(oldSource);
        var newValue = newSource is null ? null : _getter(newSource);

        // resubscribe if set is a prop
        oldValue?.StopWatchingAt(this, _key);
        newValue?.WatchAt(this, _key);

        // unsubscribe from all previous set items
        if (oldValue is not null)
            foreach (var oldItem in oldValue)
                PropChanged(oldItem, default);

        // subscribe to all new set items
        if (newValue is not null)
            foreach (var newItem in newValue)
                PropChanged(default, newItem);
    }
}