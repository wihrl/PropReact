using PropReact.Props.Value;

namespace PropReact.Props.Collections;

public interface IReactiveCollection<out TValue, in TKey> : IProp<TValue>, IEnumerable<TValue>
{
    internal TValue? WatchAt(IPropObserver<TValue> observer, TKey key);
    internal TValue? StopWatchingAt(IPropObserver<TValue> observer, TKey key);
}