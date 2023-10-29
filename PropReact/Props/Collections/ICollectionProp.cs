using PropReact.Props.Value;

namespace PropReact.Props.Collections;

public interface ICollectionProp<TValue, TKey> : IProp<TValue>, IEnumerable<TValue>, ICollectionProp<TValue>
{
    internal void SubAt(IPropObserver<TValue> observer, TKey key);
}

public interface ICollectionProp<TValue>
{
    int Count { get; }
}