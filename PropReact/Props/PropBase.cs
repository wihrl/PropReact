namespace PropReact.Properties;

internal abstract class PropBase<TValue> : IProp<TValue>
{
    protected TValue _value;
    private readonly List<IPropObserver<TValue>> _observers = new();

    protected PropBase(TValue initialValue)
    {
        _value = initialValue;
    }

    protected void SetAndNotify(TValue value)
    {
        if (value?.Equals(_value) ?? false)
            return;

        var oldValue = _value;
        _value = value;
        foreach (var watcher in _observers) watcher.PropChanged(oldValue, value);
    }

    public static implicit operator TValue(PropBase<TValue> prop) => prop._value;
    public void Sub(IPropObserver<TValue> propObserver) => _observers.Add(propObserver);
    public void Unsub(IPropObserver<TValue> propObserver) => _observers.Remove(propObserver);

    public override string? ToString() => _value?.ToString();
}