namespace PropReact.Properties;

/// <summary>
/// Computed property. Value is updated whenever a dependency changes.
/// </summary>
public interface IComputed<TValue> : IValueProp<TValue>
{
    TValue Value { get; }
    internal void Set(TValue value);
}