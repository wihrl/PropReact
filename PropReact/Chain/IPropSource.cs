using PropReact.Props.Value;

namespace PropReact.Chain;

public interface IPropSource<TSelf>
{
    public static abstract Func<TSelf, IValueProp<TValue>> GetBackingFieldGetter<TValue>(string expression);
}