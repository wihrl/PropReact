namespace PropReact.Props.Value;

internal class ComputedValueProp<T> : ValuePropBase<T>, IComputed<T>
{
    public T Value => _value;

    public ComputedValueProp(T value) : base(value)
    {
    }

    void IComputed<T>.Set(T value) => SetAndNotify(value);
}