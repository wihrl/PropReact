using System.ComponentModel;

namespace PropReact.Props.Value;

public interface IValue<out TValue> : IProp<TValue>
{
    public TValue Value { get; }
    
    // ReSharper disable once InconsistentNaming
    public TValue v => Value;
}