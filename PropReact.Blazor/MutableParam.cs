using PropReact.Props.Value;

namespace PropReact.Blazor;

public sealed class MutableParam<TValue> : ParamBase<TValue, IMutable<TValue>>, IMutable<TValue>
{
    public TValue Value
    {
        get => _prop!.Value;
        set => _prop!.Value = value;
    }

    public TValue v
    {
        get => Value;
        set => Value = value;
    }
}