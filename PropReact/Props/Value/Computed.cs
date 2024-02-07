namespace PropReact.Props.Value;

public class Computed<T>(T value) : ValuePropBase<T>(value), IComputed<T>
{
    void IComputed<T>.Set(T value) => SetAndNotify(value);
    internal void Set(T value) => SetAndNotify(value);

    public static implicit operator Computed<T>(T value) => new(value);
    public static implicit operator T(Computed<T> prop) => prop.Value;
}