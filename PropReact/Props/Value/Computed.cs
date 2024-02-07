namespace PropReact.Props.Value;

internal class Computed<T> : ValuePropBase<T>, IComputed<T>
{
    public T Value => _value;

    public Computed(T value) : base(value)
    {
    }

    void IComputed<T>.Set(T value) => SetAndNotify(value);
    internal void Set(T value) => SetAndNotify(value);
    
    public static implicit operator T(Computed<T> prop) => prop.Value;
}