namespace PropReact.Props.Value;

public interface IValueProp<TValue> : IProp<TValue>
{
    public TValue Value { get; }
}