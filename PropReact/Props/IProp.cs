namespace PropReact;

public interface IProp<T>
{
    internal void Sub(IPropObserver<T> propObserver, IReadOnlyCollection<Action> actions);
    internal void Unsub(IPropObserver<T> propObserver, IReadOnlyCollection<Action> actions);
}
