namespace PropReact;

public interface IValueOwner
{
    internal void Sub(IChangeObserver changeObserver);
    internal void Unsub(IChangeObserver changeObserver);
}