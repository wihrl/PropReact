namespace PropReact.Props.Value;

internal sealed class Computed<T> : ValuePropBase<T>, IComputed<T>
{
    public T Value => _value;

    public Computed(T value) : base(value)
    {
    }

    void IComputed<T>.Set(T value) => SetAndNotify(value);
}