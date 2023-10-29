namespace PropReact.Props;

internal abstract class PropBase<TValue> : IProp<TValue>
{
    // do not create hashset until it is actually needed
    private HashSet<IPropObserver<TValue>>? _observers;
    void IProp<TValue>.Watch(IPropObserver<TValue> observer) => (_observers ??= new()).Add(observer);
    void IProp<TValue>.StopWatching(IPropObserver<TValue> observer) => _observers!.Remove(observer);

    protected void NotifyObservers(TValue? oldValue, TValue? newValue)
    {
        if (_observers is null) return;

        foreach (var reactionsMapKey in _observers)
            reactionsMapKey.PropChanged(oldValue, newValue);
    }
}