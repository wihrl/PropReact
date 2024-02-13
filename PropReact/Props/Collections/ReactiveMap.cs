using System.Runtime.InteropServices;

namespace PropReact.Props.Collections;

public class ReactiveMap<TValue, TKey> : ReactiveCollection<TValue, TKey>, IReactiveMap<TValue, TKey> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary;
    private readonly Func<TValue, TKey> _keySelector;

    public ReactiveMap(Func<TValue, TKey> keySelector)
    {
        _keySelector = keySelector;
        _dictionary = new();
    }

    public ReactiveMap(Func<TValue, TKey> keySelector, IEnumerable<TValue> existing)
    {
        _keySelector = keySelector;
        _dictionary = existing.ToDictionary(keySelector);
    }

    public ReactiveMap(Func<TValue, TKey> keySelector, Dictionary<TKey, TValue> existing)
    {
        _keySelector = keySelector;
        _dictionary = existing;
    }

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public bool TryGet(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value!);

    public TValue? this[TKey key] => TryGet(key, out var value) ? value : default;
    public IEnumerable<TKey> Keys => _dictionary.Keys;
    public IEnumerable<TValue> Values => _dictionary.Values;

    public bool Add(TValue value)
    {
        var key = _keySelector(value);

        ref var storage = ref CollectionsMarshal.GetValueRefOrAddDefault(_dictionary, key, out var exists);
        if (!exists)
        {
            storage = value;
            Added(key, value);
        }

        return !exists;
    }

    public bool AddOrReplace(TValue value)
    {
        var key = _keySelector(value);

        ref var storage = ref CollectionsMarshal.GetValueRefOrAddDefault(_dictionary, key, out var exists);
        var oldValue = storage;
        storage = value;

        if (exists)
            Replaced(key, oldValue!, value);
        else
            Added(key, value);

        return exists;
    }

    public bool RemoveAt(TKey key)
    {
        if (!_dictionary.Remove(key, out var value)) return false;
        Removed(key, value);
        return true;
    }

    public bool Remove(TValue value)
    {
        var key = _keySelector(value);
        if (!_dictionary.Remove(key)) return false;

        Removed(key, value);
        return true;
    }

    public void Clear()
    {
        while (_dictionary.Count > 0) RemoveAt(_dictionary.Keys.First());
    }

    public override IEnumerator<TValue> GetEnumerator() => _dictionary.Values.GetEnumerator();
    public override int Count => _dictionary.Count;

    protected override TValue? InternalGetter(TKey key) =>
        _dictionary.TryGetValue(key, out var value) ? value : default;
}