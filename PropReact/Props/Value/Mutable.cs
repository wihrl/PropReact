namespace PropReact.Props.Value;

public sealed class Mutable<TValue> : ValuePropBase<TValue>, IMutable<TValue>
{
    public TValue Value
    {
        get => _value;
        set
        {
            if (Equals(_value, value))
                return;

            SetAndNotify(value);
        }
    }

    public TValue v
    {
        get => Value;
        set => Value = value;
    }

    public Mutable(TValue initialValue) : base(initialValue)
    {
    }

    public static implicit operator Mutable<TValue>(TValue value) => new(value);
}