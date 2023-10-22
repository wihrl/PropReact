namespace PropReact.Properties;

/// <summary>
/// Navigation property. Used on the other side of an IProp or a reactive collection to access the parent.
/// </summary>
public interface INavProp<TValue> : IValueOwner
{
    TValue V { get; }
    internal void Set(TValue value);
}