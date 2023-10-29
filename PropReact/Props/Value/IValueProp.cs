namespace PropReact;

public interface IValueProp<TValue> : IProp<TValue>
{
    public TValue Value { get; }
}