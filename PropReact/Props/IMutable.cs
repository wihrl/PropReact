﻿namespace PropReact.Properties;

public interface IMutable<TValue> : IValueProp<TValue>
{
    public new TValue Value { get; set; }

    TValue IValueProp<TValue>.Value => Value;
}