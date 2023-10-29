
namespace PropReact.Properties;

internal class MutableValueProp<TValue> : ValuePropBase<TValue>, IMutable<TValue>
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