using PropReact.Props.Value;

namespace PropReact.Props;

public static class WatchExtensions
{
    public static IDisposable Watch<TValue>(this IValue<TValue> prop, Action<TValue?, TValue?> action)
    {
        var observer = new SimpleObserver<TValue>(action, prop);
        observer.Subscribe();
        return observer;
    }

    public static void Watch<TValue>(this IValue<TValue> prop, Action<TValue?, TValue?> action, ICompositeDisposable disposable) =>
        disposable.AddDisposable(Watch(prop, action));
}

file class SimpleObserver<T>(Action<T?, T?> action, IValue<T> prop) : IPropObserver<T>, IDisposable
{
    internal void Subscribe() => prop.Watch(this);

    public void PropChanged(T? oldValue, T? newValue) => action(oldValue, newValue);

    public void Dispose()
    {
        prop.StopWatching(this);
    }
}