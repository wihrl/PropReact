namespace PropReact.Props.Collections;

public interface IReactiveMap<TValue, TKey> : IReactiveCollection<TValue, TKey> where TKey : notnull
{
    bool ContainsKey(TKey key);
    bool TryGet(TKey key, out TValue value);
    TValue? this[TKey key] { get; }
    IEnumerable<TKey> Keys { get; }
    IEnumerable<TValue> Values { get; }
    int Count { get; }
    bool Add(TValue value);
    bool AddOrReplace(TValue value);
    bool RemoveAt(TKey key);
    bool Remove(TValue value);
    void Clear();
}