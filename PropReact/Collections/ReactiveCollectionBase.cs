using PropReact.Properties;
using PropReact.Reactivity;

namespace PropReact.Collections;

internal abstract class ReactiveCollectionBase<TKey, TValue> : IProp<>, IWatchableCollection<TKey, TValue>
    where TKey : notnull
{
    private readonly List<IPropObserver<>> _observers = new();

    // todo: trigger only once for bulk changes
    protected void Added(TKey key, TValue newValue)
    {
        foreach (var changeObserver in _observers) changeObserver.PropChanged(null, newValue);
        UpdateWatchers(key, newValue);
    }

    protected void Removed(TKey key, TValue oldValue)
    {
        foreach (var changeObserver in _observers) changeObserver.PropChanged(oldValue, null);
        UpdateWatchers(key, default);
    }

    protected void Replaced(TKey key, TValue oldValue, TValue newValue)
    {
        foreach (var changeObserver in _observers) changeObserver.PropChanged(oldValue, newValue);
        UpdateWatchers(key, newValue);
    }

    void IProp<>.Sub(IPropObserver<> propObserver) => _observers.Add(propObserver);
    void IProp<>.Unsub(IPropObserver<> propObserver) => _observers.Remove(propObserver);

    private Dictionary<TKey, List<IComputed<TValue?>>>? _watchers;
    // public IViewProp<TKey, TValue?> WatchAt(TKey key)
    // {
    //     _watchers ??= new();
    //     if (!_watchers.TryGetValue(key, out var list))
    //     {
    //         list ??= new();
    //         _watchers[key] = list;
    //     }
    //
    //     IViewProp<TKey, TValue?> prop = new ViewProp<TKey, TValue?>(key);
    //     list.Add(prop);
    //     prop.Set(InternalGetter(key));
    //     return prop;
    // }

    protected abstract TValue? InternalGetter(TKey key);

    void UpdateWatchers(TKey key, TValue? value)
    {
        if (_watchers is null) return;

        if (!_watchers.TryGetValue(key, out var list)) return;
        
        foreach (var comp in list)
            comp.Set(value);
    }
}