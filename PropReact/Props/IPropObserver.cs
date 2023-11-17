namespace PropReact.Props;

public interface IPropObserver<in T>
{
    void PropChanged(T? oldValue, T? newValue);
}