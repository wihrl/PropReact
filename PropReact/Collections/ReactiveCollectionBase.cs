using PropReact.Properties;
using PropReact.Reactivity;

namespace PropReact.Collections;

internal abstract class ReactiveCollectionBase<TKey, TValue> : IValueOwner, IWatchableCollection<TKey, TValue>
    where TKey : notnull
{
    private readonly List<IChangeObserver> _observers = new();
    private readonly INavSetter<TValue>? _navSetter;

    protected ReactiveCollectionBase(INavSetter<TValue>? navSetter)
    {
        _navSetter = navSetter;
    }

    // todo: trigger only once for bulk changes
    protected void Added(TKey key, TValue newValue)
    {
        foreach (var changeObserver in _observers) changeObserver.OwnedValueChanged(null, newValue);
        _navSetter?.Update(default, newValue);
        UpdateWatchers(key, newValue);
    }

    protected void Removed(TKey key, TValue oldValue)
    {
        foreach (var changeObserver in _observers) changeObserver.OwnedValueChanged(oldValue, null);
        _navSetter?.Update(oldValue, default);
        UpdateWatchers(key, default);
    }

    protected void Replaced(TKey key, TValue oldValue, TValue newValue)
    {
        foreach (var changeObserver in _observers) changeObserver.OwnedValueChanged(oldValue, newValue);
        _navSetter?.Update(oldValue, newValue);
        UpdateWatchers(key, newValue);
    }

    void IValueOwner.Sub(IChangeObserver changeObserver) => _observers.Add(changeObserver);
    void IValueOwner.Unsub(IChangeObserver changeObserver) => _observers.Remove(changeObserver);

    private Dictionary<TKey, List<ICompProp<TValue?>>>? _watchers;
    public IViewProp<TKey, TValue?> WatchAt(TKey key)
    {
        _watchers ??= new();
        if (!_watchers.TryGetValue(key, out var list))
        {
            list ??= new();
            _watchers[key] = list;
        }

        IViewProp<TKey, TValue?> prop = new ViewProp<TKey, TValue?>(key);
        list.Add(prop);
        prop.Set(InternalGetter(key));
        return prop;
    }

    protected abstract TValue? InternalGetter(TKey key);

    void UpdateWatchers(TKey key, TValue? value)
    {
        if (_watchers is null) return;

        if (!_watchers.TryGetValue(key, out var list)) return;
        
        foreach (var comp in list)
            comp.Set(value);
    }
}