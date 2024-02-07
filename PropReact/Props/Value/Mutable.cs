namespace PropReact.Props.Value;

public sealed class Mutable<TValue>(TValue initialValue) : ValuePropBase<TValue>(initialValue), IMutable<TValue>
{
    public new TValue Value
    {
        get => _value;
        set
        {
            if (Equals(_value, value))
                return;

            SetAndNotify(value);
        }
    }

    public new TValue v
    {
        get => Value;
        set => Value = value;
    }

    public static implicit operator Mutable<TValue>(TValue value) => new(value);
    public static implicit operator TValue(Mutable<TValue> prop) => prop.Value;
}