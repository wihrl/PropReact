namespace PropReact;

public interface IProp<T>
{
    internal void Sub(IPropObserver<T> propObserver);
    internal void Unsub(IPropObserver<T> propObserver);
}
