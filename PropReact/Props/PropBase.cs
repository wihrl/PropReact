namespace PropReact.Props;

public abstract class PropBase<TValue> : IProp<TValue>
{
    // do not create hashset until it is actually needed
    private HashSet<IPropObserver<TValue>>? _observers;
    void IProp<TValue>.Sub(IPropObserver<TValue> observer) => (_observers ??= new()).Add(observer);
    void IProp<TValue>.Unsub(IPropObserver<TValue> observer) => (_observers ??= new()).Remove(observer);

    protected void NotifyObservers(TValue? oldValue, TValue? newValue)
    {
        if (_observers is not null)
            foreach (var reactionsMapKey in _observers)
                reactionsMapKey.PropChanged(oldValue, newValue);
    }
}