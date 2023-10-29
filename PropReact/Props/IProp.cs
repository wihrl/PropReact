namespace PropReact.Props;

public interface IProp<T>
{
    // props should not concern themselves with anything other than notifying a list observers of changes
    // this interface is thus feature-complete! - do not change
    internal void Sub(IPropObserver<T> observer);
    internal void Unsub(IPropObserver<T> observer);
}
