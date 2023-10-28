namespace PropReact.Properties;

internal abstract class ValuePropBase<TValue> : IProp<TValue>
{
    protected TValue _value;

    protected ValuePropBase(TValue initialValue)
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

    public static implicit operator TValue(ValuePropBase<TValue> valueProp) => valueProp._value;
    public override string? ToString() => _value?.ToString();
}