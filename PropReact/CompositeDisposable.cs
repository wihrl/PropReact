
namespace PropReact;

public interface ICompositeDisposable : IDisposable
{
    void AddDisposable(IDisposable disposable);
}

public class CompositeDisposable : ICompositeDisposable
{
    private readonly List<IDisposable> _ownedDisposables = new();

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