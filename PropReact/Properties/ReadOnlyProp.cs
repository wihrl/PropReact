namespace PropReact.Properties;

internal class ReadOnlyProp<T> : PropBase<T>, INavProp<T>, ICompProp<T>
{
    public T V => _value;

    public ReadOnlyProp() : base(default)
    {
    }

    void INavProp<T>.Set(T value) => SetAndNotify(value);
    void ICompProp<T>.Set(T value) => SetAndNotify(value);
}