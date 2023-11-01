namespace PropReact.Props;

public interface IProp<out TValue>
{
    internal void Watch(IPropObserver<TValue> observer);
    internal void StopWatching(IPropObserver<TValue> observer);
}
