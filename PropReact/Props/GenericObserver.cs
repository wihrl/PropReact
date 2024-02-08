﻿using PropReact.Props.Value;

namespace PropReact.Props;

internal class GenericObserver<T>(IValue<T> prop, Action<T?, T?> action) : IPropObserver<T>, IDisposable
{
    public void Start() => prop.Watch(this);

    public void PropChanged(T? oldValue, T? newValue) => action(oldValue, newValue);

    public void Dispose()
    {
        prop.StopWatching(this);
    }
}