using PropReact.Chain.Nodes;
using PropReact.Props.Value;

namespace PropReact.Chain.Reactions;

public interface IReactionBuilder<TRoot>
{
    IReactionBuilder<TRoot> React(Action reaction, bool runNow = false);
    IReactionBuilder<TRoot> ReactAsync(Action<CancellationToken> action, bool runNow = false);
    IReactionBuilder<TRoot> Compute<TValue>(Func<TValue> getter, out IDisposable disposable);
    IReactionBuilder<TRoot> ComputeAsync<TValue>(Func<CancellationToken, Task<TValue>> getter);
    IDisposable StartDisposable();
    void Start(ICompositeDisposable disposable);
}

abstract class Reaction<TRoot> : IReactionBuilder<TRoot>
{
    protected event Action? Reactions;
    protected event Action<CancellationToken>? AsyncReactions;
    private CancellationTokenSource _cts = new();

    private readonly RootNodeSource<TRoot> _root;
    internal Reaction(RootNodeSource<TRoot> root) => _root = root;

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.React(Action reaction, bool runNow)
    {
        Reactions += reaction;
        if (runNow)
            reaction();

        return this;
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.ReactAsync(Action<CancellationToken> action, bool runNow)
    {
        AsyncReactions += action;
        if (runNow)
            action(_cts.Token);

        return this;
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.Compute<TValue>(Func<TValue> getter, out IDisposable disposable)
    {
        throw new NotImplementedException();
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.ComputeAsync<TValue>(Func<CancellationToken, Task<TValue>> getter)
    {
        throw new NotImplementedException();
    }

    IDisposable IReactionBuilder<TRoot>.StartDisposable()
    {
        _root.Attach(Trigger);
        return _root;
    }

    void IReactionBuilder<TRoot>.Start(ICompositeDisposable disposable)
    {
        disposable.AddDisposable(((IReactionBuilder<TRoot>) this).StartDisposable());
    }

    protected abstract void Trigger();

    protected void TriggerReactions() => Reactions?.Invoke();

    protected void TriggerAsyncReactions()
    {
        if (AsyncReactions is null)
            return;

        _cts.Cancel();
        _cts = new();
        AsyncReactions.Invoke(_cts.Token);
    }
}