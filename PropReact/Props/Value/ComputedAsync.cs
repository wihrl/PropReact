namespace PropReact.Props.Value;

public class ComputedAsync<T>(T value) : ValuePropBase<T>(value), IComputedAsync<T>
{
    public T Value => _value;
    public T v => Value;
    
    public IComputed<bool> IsRunning { get; } = new Computed<bool>(false);
    private int _running;

    public void IncrementRunning()
    {
        lock (IsRunning)
        {
            _running++;
            IsRunning.Set(true);
        }
    }

    public void DecrementRunning()
    {
        lock (IsRunning)
        {
            _running--;
            if (_running == 0)
                IsRunning.Set(false);
        }
    }

    void IComputed<T>.Set(T value) => SetAndNotify(value);
    internal void Set(T value) => SetAndNotify(value);

    public static implicit operator ComputedAsync<T>(T value) => new(value);
    public static implicit operator T(ComputedAsync<T> prop) => prop.Value;
}