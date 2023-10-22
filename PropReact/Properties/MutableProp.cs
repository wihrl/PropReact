using PropReact.Reactivity;

namespace PropReact.Properties;

internal class MutableProp<TValue> : PropBase<TValue>, IProp<TValue>
{
    private INavSetter<TValue>? _navSetter;

    public TValue V
    {
        get => _value;
        set
        {
            if(Equals(_value, value))
                return;
            
            var oldValue = _value;
            SetAndNotify(value);
            _navSetter?.Update(oldValue, value);
        }
    }

    internal MutableProp(TValue initialValue, INavSetter<TValue>? navSetter = null) : base(initialValue)
    {
        _navSetter = navSetter;
        _navSetter?.Update(default, initialValue);
    }
}