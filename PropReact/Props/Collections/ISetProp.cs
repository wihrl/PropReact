using PropReact.Props.Value;

namespace PropReact.Props.Collections;

public interface ICollectionProp<TValue> : IProp<TValue>, IEnumerable<TValue>
{
    IValueProp<int> Count { get; }
}