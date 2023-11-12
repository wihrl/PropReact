namespace PropReact.Props.Value;

/// <summary>
/// A mutable reactive property.
/// </summary>
public interface IMutable<TValue> : IValueProp<TValue>
{
    public new TValue Value { get; set; }
    
    // ReSharper disable once InconsistentNaming
    public new TValue v
    {
        get => Value;
        set => Value = value;
    }
}