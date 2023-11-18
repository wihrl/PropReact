namespace PropReact.Props.Value;

/// <summary>
/// Computed property. Value is updated whenever a dependency changes.
/// </summary>
public interface IComputed<TValue> : IValue<TValue>
{
    internal void Set(TValue value);
}