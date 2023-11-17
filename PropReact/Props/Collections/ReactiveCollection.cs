using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PropReact.Props.Value;

namespace PropReact.Props.Collections;

public abstract class ReactiveCollection<TValue, TKey> : PropBase<TValue>, IKeyedCollectionProp<TValue, TKey>
    where TKey : notnull
{
    public abstract IEnumerator<TValue> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public abstract int Count { get; }

    protected void Added(TKey key, TValue newValue) => NotifyObservers(key, default, newValue);
    protected void Removed(TKey key, TValue oldValue) => NotifyObservers(key, oldValue, default);
    protected void Replaced(TKey key, TValue oldValue, TValue newValue) => NotifyObservers(key, oldValue, newValue);

    void NotifyObservers(TKey key, TValue? oldValue, TValue? newValue)
    {
        if (Observers is not null)
            foreach (var propObserver in Observers.Keys)
                propObserver.PropChanged(oldValue, newValue);

        if (_keyedObservers is null) return;
        if (!_keyedObservers.TryGetValue(key, out var keyed)) return;

        foreach (var observer in keyed)
            observer.PropChanged(oldValue, newValue);
    }


    private Dictionary<TKey, HashSet<IPropObserver<TValue>>>? _keyedObservers;
    TValue? IKeyedCollectionProp<TValue, TKey>.WatchAt(IPropObserver<TValue> observer, TKey key)
    {
        _keyedObservers ??= new();
        if (!_keyedObservers.TryGetValue(key, out var set))
            _keyedObservers[key] = set = new();

        set.Add(observer);
        
        return InternalGetter(key);
    }

    TValue? IKeyedCollectionProp<TValue, TKey>.StopWatchingAt(IPropObserver<TValue> observer, TKey key)
    {
        if (_keyedObservers is not null && _keyedObservers.TryGetValue(key, out var set))
            set.Remove(observer);

        return InternalGetter(key);
    }

    protected abstract TValue? InternalGetter(TKey key);
}