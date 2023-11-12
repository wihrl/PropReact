using System.Collections;
using PropReact.Props.Value;

namespace PropReact.Props.Collections;

internal abstract class CollectionPropBase<TValue, TKey> : IKeyedCollectionProp<TValue, TKey>
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
        if (_observers is not null)
            foreach (var propObserver in _observers)
                propObserver.PropChanged(oldValue, newValue);

        if (_keyedObservers is null) return;
        if (!_keyedObservers.TryGetValue(key, out var keyed)) return;

        foreach (var observer in keyed)
            observer.PropChanged(oldValue, newValue);
    }


    private HashSet<IPropObserver<TValue>>? _observers;
    private Dictionary<TKey, HashSet<IPropObserver<TValue>>>? _keyedObservers;

    public void WatchAt(IPropObserver<TValue> observer, TKey key)
    {
        _keyedObservers ??= new();
        if (!_keyedObservers.TryGetValue(key, out var set))
            _keyedObservers[key] = set = new();

        set.Add(observer);
    }

    public void StopWatchingAt(IPropObserver<TValue> observer, TKey key)
    {
        if (_keyedObservers is not null && _keyedObservers.TryGetValue(key, out var set))
            set.Remove(observer);
    }

    public void Watch(IPropObserver<TValue> observer) => (_observers ??= new()).Add(observer);
    public void StopWatching(IPropObserver<TValue> observer) => _observers!.Remove(observer);

    protected abstract TValue? InternalGetter(TKey key);
}