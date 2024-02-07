namespace PropReact.Props.Value;

internal class ComputedAsync<T> : ValuePropBase<T>, IComputedAsync<T>
{
    public T Value => _value;
    public IComputed<bool> IsRunning { get; } = new Computed<bool>(false);
    public int _running = 1;
    
    public void IncrementRunning()
    {
        Interlocked.Increment(ref _running);
        IsRunning.Set(true);
    }

    public void DecrementRunning()
    {
        if (Interlocked.Decrement(ref _running) == 0)
            IsRunning.Set(false);
    }

    public ComputedAsync(T value) : base(value)
    {
    }

    void IComputed<T>.Set(T value) => SetAndNotify(value);
    internal void Set(T value) => SetAndNotify(value);

    public static implicit operator T(ComputedAsync<T> prop) => prop.Value;
}