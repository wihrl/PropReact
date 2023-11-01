namespace PropReact.Props;

internal interface IPropObserver<in T>
{
    void PropChanged(T? oldValue, T? newValue);
}