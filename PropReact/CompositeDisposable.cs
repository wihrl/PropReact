using PropReact.Reactivity;

namespace PropReact;

public class CompositeDisposable : ICompositeDisposable
{
    private List<IDisposable> _ownedDisposables = new();

    public void AddDisposable(IDisposable disposable) => _ownedDisposables.Add(disposable);

    public void Dispose()
    {
        foreach (var ownedDisposable in _ownedDisposables) ownedDisposable.Dispose();
        OnDispose();
        _ownedDisposables.Clear();
    }

    protected virtual void OnDispose()
    {
    }
}