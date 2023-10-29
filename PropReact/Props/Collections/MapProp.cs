using System.Collections;
using PropReact.Props.Value;

namespace PropReact.Props.Collections;

public interface IMap<TKey, TValue> : IWatchableCollection<TKey, TValue?>, IEnumerable<TValue>
    where TKey : notnull
{
    bool ContainsKey(TKey key);
    bool TryGetValue(TKey key, out TValue value);
    TValue this[TKey key] { get; }
    int Count { get; }
    void Add(TValue value);
    void AddOrReplace(TValue value, Action<TValue> conflictAction);
    bool Remove(TValue value);
}

internal class MapProp<TKey, TValue> : ReactiveCollectionBase<TKey, TValue>, IMap<TKey, TValue> where TKey : notnull
{
    readonly Dictionary<TKey, TValue> _dictionary = new();
    readonly Func<TValue, TKey> _keySelector;

    public MapProp(Func<TValue, TKey> keySelector)
    {
        _keySelector = keySelector;
    }

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value!);

    public TValue this[TKey key] => _dictionary[key];
    public IEnumerable<TKey> Keys => _dictionary.Keys;
    public IEnumerable<TValue> Values => _dictionary.Values;

    public void Add(TValue value) => AddOrReplace(value, x => throw new("Key already present!"));

    public void AddOrReplace(TValue value, Action<TValue> conflictAction)
    {
        var key = _keySelector(value);
        
        if (_dictionary.TryGetValue(key, out var existingValue))
        {
            conflictAction(existingValue);
            return;
        }
        
        _dictionary[key] = value;
        Added(key, value);
    }

    public bool Remove(TValue value)
    {
        var key = _keySelector(value);
        if (!_dictionary.Remove(key)) return false;

        Removed(key, value);
        return true;
    }

    public IEnumerator<TValue> GetEnumerator() => _dictionary.Values.GetEnumerator();
    public int Count => _dictionary.Count;
    protected override TValue? InternalGetter(TKey key) => _dictionary.TryGetValue(key, out var val) ? val : default;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IComputed<TKey?> WatchAt(TValue key)
    {
        throw new NotImplementedException();
    }
}