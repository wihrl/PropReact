namespace PropReact.Reactivity;

public interface ICompositeDisposable : IDisposable
{
    void AddDisposable(IDisposable disposable);
}