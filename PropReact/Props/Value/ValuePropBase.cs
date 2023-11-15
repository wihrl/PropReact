namespace PropReact.Props.Value;

public abstract class ValuePropBase<TValue> : PropBase<TValue>
{
    protected TValue _value;

    protected ValuePropBase(TValue initialValue)
    {
        _value = initialValue;
    }

    protected void SetAndNotify(TValue newValue)
    {
        if (newValue?.Equals(_value) ?? false)
            return;

        NotifyObservers(_value, _value = newValue);
    }

    public static implicit operator TValue(ValuePropBase<TValue> valueProp) => valueProp._value;
    public override string? ToString() => _value?.ToString();
}