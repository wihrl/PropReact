
namespace PropReact.Utils;

public interface ICompositeDisposable : IDisposable
{
    void Add(IDisposable disposable);
}

public class CompositeDisposable : ICompositeDisposable
{
    private readonly List<IDisposable> _ownedDisposables = new();

    public void Add(IDisposable disposable) => _ownedDisposables.Add(disposable);

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