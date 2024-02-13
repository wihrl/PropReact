using System.Collections;

namespace PropReact.Props.Collections;

public abstract class ReactiveCollection<TValue, TKey> : PropBase<TValue>, IReactiveCollection<TValue, TKey>
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

        if (KeyedObservers is null) return;
        if (!KeyedObservers.TryGetValue(key, out var keyed)) return;

        foreach (var observer in keyed)
            observer.PropChanged(oldValue, newValue);
    }


    protected Dictionary<TKey, HashSet<IPropObserver<TValue>>>? KeyedObservers;
    TValue? IReactiveCollection<TValue, TKey>.WatchAt(IPropObserver<TValue> observer, TKey key)
    {
        KeyedObservers ??= new();
        if (!KeyedObservers.TryGetValue(key, out var set))
            KeyedObservers[key] = set = new();

        set.Add(observer);
        
        return InternalGetter(key);
    }

    TValue? IReactiveCollection<TValue, TKey>.StopWatchingAt(IPropObserver<TValue> observer, TKey key)
    {
        if (KeyedObservers is not null && KeyedObservers.TryGetValue(key, out var set))
            set.Remove(observer);

        return InternalGetter(key);
    }

    protected abstract TValue? InternalGetter(TKey key);
}