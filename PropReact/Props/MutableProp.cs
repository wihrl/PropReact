using PropReact.Reactivity;

namespace PropReact.Properties;

internal class MutableProp<TValue> : PropBase<TValue>, IMutable<TValue>
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

    internal MutableProp(TValue initialValue) : base(initialValue)
    {
    }
}