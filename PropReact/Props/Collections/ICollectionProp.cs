using PropReact.Props.Value;

namespace PropReact.Props.Collections;

public interface ICollectionProp<TValue, TKey> : IProp<TValue>, IEnumerable<TValue>, ICollectionProp<TValue>
{
    internal void WatchAt(IPropObserver<TValue> observer, TKey key);
}

public interface ICollectionProp<TValue>
{
    int Count { get; }
}