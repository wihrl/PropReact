﻿namespace PropReact.Chain;

public sealed class SetNode<TSource, TSet, TValue> : ChainNode<TSource, TSet>, IChainNode<TSource>, IPropObserver<TValue> where TSet : class, IEnumerable<TValue>
{
    private readonly InnerSetNode _innerProxy;
    private readonly Func<TSource, TSet> _getter;
    
    public SetNode(Func<TSource, TSet> getter)
    {
        _getter = getter;
        _innerProxy = new(this);
    }

    public ChainNode<TValue, TValue> Enter() => _innerProxy;
    
    
    // todo
    // public ChainableNodeBase<TValue> Enter(TKey key)
    // {
    //     throw new InvalidOperationException();
    // }
    
    public void PropChanged(TValue? oldValue, TValue? newValue)
    {
        // an item was added or removed from the set
        _innerProxy.PropagateChange(oldValue, newValue);
    }

    void IChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        var oldValue = oldSource is null ? null : _getter(oldSource);
        var newValue = newSource is null ? null : _getter(newSource);

        var oldProp = oldValue as IProp<TValue>;
        var newProp = newValue as IProp<TValue>;

        // resubscribe if set is a prop
        oldProp?.Unsub(this);
        newProp?.Sub(this);
        
        // notify following nodes without entering (for example .Count)
        foreach (var chainNode in _next)
            chainNode.ChangeSource(oldValue, newValue);
        
        // unsubscribe from all previous set items
        if(oldValue is not null)
            foreach (var oldItem in oldValue)
                PropChanged(oldItem, default);
        
        // subscribe to all new set items
        if(newValue is not null)
            foreach (var newItem in newValue)
                PropChanged(default, newItem);
    }
    
    class InnerSetNode : ChainNode<TValue, TValue>
    {
        private SetNode<TSource, TSet, TValue> _owner;
        public InnerSetNode(SetNode<TSource, TSet, TValue> owner) => _owner = owner;

        internal void PropagateChange(TValue? oldValue, TValue? newValue)
        {
            foreach (var chainNode in _next)
                chainNode.ChangeSource(oldValue, newValue);
        }
    }
}