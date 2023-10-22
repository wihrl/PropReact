namespace PropReact.Properties;

internal abstract class PropBase<TValue> : IValueOwner
{
    protected TValue _value;
    private List<IChangeObserver> _watchers = new();

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
        foreach (var watcher in _watchers) watcher.OwnedValueChanged(oldValue, value);
    }

    public static implicit operator TValue(PropBase<TValue> prop) => prop._value;
    public void Sub(IChangeObserver changeObserver) => _watchers.Add(changeObserver);
    public void Unsub(IChangeObserver changeObserver) => _watchers.Remove(changeObserver);

    public override string? ToString() => _value?.ToString();
}