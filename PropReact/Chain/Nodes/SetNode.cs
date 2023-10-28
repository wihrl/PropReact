namespace PropReact.Chain;

public sealed class SetNode<TSource, TSet, TValue> : ChainNode<TSet>, IChainNode<TSource>, IPropObserver<TValue> where TSet : class, IEnumerable<TValue>
{
    private readonly InnerSetNode _innerProxy = new();
    private readonly Func<TSource, TSet> _getter;
    
    public SetNode(Func<TSource, TSet> getter) => _getter = getter;

    public ChainNode<TValue> Enter() => _innerProxy;
    
    
    // todo
    // public ChainableNodeBase<TValue> Enter(TKey key)
    // {
    //     throw new InvalidOperationException();
    // }
    
    public void PropChanged(TValue? oldValue, TValue? newValue, IReadOnlyCollection<Action> reactions)
    {
        // an item was added or removed from the set
        _innerProxy.PropagateChange(oldValue, newValue, reactions);
    }

    void IChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource, IReadOnlyCollection<Action> reactions)
    {
        var oldValue = oldSource is null ? null : _getter(oldSource);
        var newValue = newSource is null ? null : _getter(newSource);

        var oldProp = oldValue as IProp<TValue>;
        var newProp = newValue as IProp<TValue>;

        // resubscribe if set is a prop
        oldProp?.Unsub(this, reactions);
        newProp?.Sub(this, reactions);
        
        // notify following nodes without entering (for example .Count)
        foreach (var chainNode in _next)
            chainNode.ChangeSource(oldValue, newValue, reactions);
        
        // unsubscribe from all previous set items
        if(oldValue is not null)
            foreach (var oldItem in oldValue)
                PropChanged(oldItem, default, reactions);
        
        // subscribe to all new set items
        if(newValue is not null)
            foreach (var newItem in newValue)
                PropChanged(default, newItem, reactions);
    }
    
    class InnerSetNode : ChainNode<TValue>
    {
        internal void PropagateChange(TValue? oldValue, TValue? newValue, IReadOnlyCollection<Action> reactions)
        {
            foreach (var chainNode in _next)
                chainNode.ChangeSource(oldValue, newValue, reactions);
        }
    }
}