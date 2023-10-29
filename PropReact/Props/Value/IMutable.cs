﻿namespace PropReact.Properties;

/// <summary>
/// A mutable reactive property.
/// </summary>
public interface IMutable<TValue> : IValueProp<TValue>
{
    public new TValue Value { get; set; }
}