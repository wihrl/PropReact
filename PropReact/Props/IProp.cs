namespace PropReact.Props;

public interface IProp<TValue>
{
    internal void Watch(IPropObserver<TValue> observer);
    internal void StopWatching(IPropObserver<TValue> observer);
}
