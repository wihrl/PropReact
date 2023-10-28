namespace PropReact;

internal interface IPropObserver<T>
{
    void PropChanged(T? oldValue, T? newValue);
}