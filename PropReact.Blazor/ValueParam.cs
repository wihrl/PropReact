using PropReact.Props.Value;

namespace PropReact.Blazor;

public sealed class ValueParam<TValue> : ParamBase<TValue, IValue<TValue>>, IValue<TValue>
{
    public TValue Value => _prop!.Value;
    public TValue v => Value;
}