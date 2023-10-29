
namespace PropReact.Props.Value;

internal sealed class MutableValueProp<TValue> : ValuePropBase<TValue>, IMutable<TValue>
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

    internal MutableValueProp(TValue initialValue) : base(initialValue)
    {
    }
}