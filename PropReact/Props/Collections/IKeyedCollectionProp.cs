using PropReact.Props.Value;

namespace PropReact.Props.Collections;

public interface IKeyedCollectionProp<out TValue, in TKey> : IProp<TValue>, IEnumerable<TValue>
{
    internal void WatchAt(IPropObserver<TValue> observer, TKey key);
    internal void StopWatchingAt(IPropObserver<TValue> observer, TKey key);
}