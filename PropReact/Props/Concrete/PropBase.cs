using PropReact.Chain;

namespace PropReact.Properties;

public abstract class PropBase<TValue> : IProp<TValue>
{
    private readonly HashSet<IPropObserver<TValue>> _observers = new();
    void IProp<TValue>.Sub(IPropObserver<TValue> observer) => _observers.Add(observer);
    void IProp<TValue>.Unsub(IPropObserver<TValue> observer) => _observers.Remove(observer);

    protected void NotifyObservers(TValue? oldValue, TValue? newValue)
    {
        foreach (var reactionsMapKey in _observers)
            reactionsMapKey.PropChanged(oldValue, newValue);
    }
}