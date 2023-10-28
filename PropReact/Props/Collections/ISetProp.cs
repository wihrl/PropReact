namespace PropReact.Props;

public interface ISetProp<TValue> : IProp<TValue>, IEnumerable<TValue>
{
    IValueProp<int> Count { get; }
}