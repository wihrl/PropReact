namespace PropReact.Props;

internal interface IPropObserver<T>
{
    void PropChanged(T? oldValue, T? newValue);
}