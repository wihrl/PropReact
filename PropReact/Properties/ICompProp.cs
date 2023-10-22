namespace PropReact.Properties;

/// <summary>
/// Computed property. Value is updated whenever a dependency changes.
/// </summary>
public interface ICompProp<TValue> : IValueOwner
{
    TValue V { get; }
    internal void Set(TValue value);
}