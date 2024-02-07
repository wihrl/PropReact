using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PropReact.Props.Value;

public abstract class ValuePropBase<TValue>(TValue initialValue) : PropBase<TValue>, IValue<TValue>
{
    protected TValue _value = initialValue;
    public TValue Value => _value;
    public TValue v => _value;

    protected void SetAndNotify(TValue newValue)
    {
        if (newValue?.Equals(_value) ?? false)
            return;

        NotifyObservers(_value, _value = newValue);
    }

    public static implicit operator TValue(ValuePropBase<TValue> valueProp) => valueProp._value;
    public override string? ToString() => _value?.ToString();
}