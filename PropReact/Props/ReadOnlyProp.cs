namespace PropReact.Properties;

internal class ReadOnlyProp<T> : PropBase<T>, IComputed<T>
{
    public T Value => _value;

    public ReadOnlyProp(T value) : base(value)
    {
    }


    void IComputed<T>.Set(T value) => SetAndNotify(value);
}