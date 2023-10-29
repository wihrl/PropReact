namespace PropReact.Props;

public interface IProp<TValue>
{
    internal void Sub(IPropObserver<TValue> observer);
    internal void Unsub(IPropObserver<TValue> observer);
}
