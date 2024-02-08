namespace PropReact.Props.Value;

public interface IMutable<TValue> : IValue<TValue>
{
    public new TValue Value { get; set; }
    
    // ReSharper disable once InconsistentNaming
    public new TValue v { get => Value; set => Value = value; }
}