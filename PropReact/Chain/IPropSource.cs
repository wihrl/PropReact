using PropReact.Props.Value;

namespace PropReact.Chain;

public interface IPropSource
{
    public static abstract IValueProp<TValue> GetBacking<TValue>(string expression);
}