namespace PropReact.Props;

interface IPropObserver<in T>
{
    void PropChanged(T? oldValue, T? newValue);
}