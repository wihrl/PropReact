﻿namespace PropReact.Props.Value;

/// <summary>
/// A mutable reactive property.
/// </summary>
public interface IMutable<TValue> : IValue<TValue>
{
    public new TValue Value { get; set; }
    
    // ReSharper disable once InconsistentNaming
    public new TValue v { get; set; }
}