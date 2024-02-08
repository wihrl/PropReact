using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Blazor;

public abstract class ParamBase<TValue, TProp> : PropBase<TValue>, IPropObserver<TValue> where TProp : IValue<TValue>
{
    protected TProp? _prop;

    public void Update(TProp prop)
    {
        if (_prop?.Equals(prop) ?? false)
            return;
        
        _prop?.StopWatching(this);

        var oldProp = _prop;
        _prop = prop;

        if (oldProp != null)
        {
            oldProp.StopWatching(this);
            PropChanged(oldProp.Value, _prop.Value);
        }
        else
            PropChanged(default, _prop.Value);

        _prop.Watch(this);
    }

    public void PropChanged(TValue? oldValue, TValue? newValue) => NotifyObservers(oldValue, newValue);
}