using PropReact.Chain.Nodes;
using PropReact.Props.Value;

namespace PropReact.Chain.Reactions;

using AsyncAction = Func<CancellationToken, ValueTask>;

public interface IReactionBuilder<TRoot>
{
    IReactionBuilder<TRoot> React(Action reaction, bool runNow = false);
    IReactionBuilder<TRoot> ReactAsync(Func<CancellationToken, ValueTask> action, bool runNow = false);
    IReactionBuilder<TRoot> Compute<TValue>(Func<TValue> getter, out IComputed<TValue> prop);
    IReactionBuilder<TRoot> ComputeAsync<TValue>(Func<CancellationToken, ValueTask<TValue>> getter, out IComputedAsync<TValue> prop, TValue defaultValue);
    IDisposable StartAsDisposable();
    void Start(ICompositeDisposable disposable);
    IReactionBuilder<TRoot> CatchAsync(Action<Exception> handler);
}

abstract class Reaction<TRoot> : IReactionBuilder<TRoot>, IDisposable
{
    private event Action? Reactions;
    private event Func<CancellationToken, ValueTask>? AsyncReactions;
    private Action<Exception>? AsyncExceptionHandler;

    private CancellationTokenSource _cts = new();

    private readonly RootNode<TRoot> _root;
    internal Reaction(RootNode<TRoot> root) => _root = root;

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.React(Action reaction, bool runNow)
    {
        Reactions += reaction;
        if (runNow)
            reaction();

        return this;
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.ReactAsync(Func<CancellationToken, ValueTask> action, bool runNow)
    {
        Func<CancellationToken, ValueTask> wrappedAction = async token =>
        {
            try
            {
                await action(token);
            }
            catch (Exception e)
            {
                AsyncExceptionHandler?.Invoke(e);
            }
        };

        if (runNow)
            wrappedAction.Invoke(_cts.Token);

        AsyncReactions += wrappedAction;

        return this;
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.Compute<TValue>(Func<TValue> getter, out IComputed<TValue> prop)
    {
        var local = prop = new Computed<TValue>(getter());
        Reactions += () => local.Set(getter());

        return this;
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.ComputeAsync<TValue>(
        Func<CancellationToken, ValueTask<TValue>> getter,
        out IComputedAsync<TValue> prop,
        TValue defaultValue)
    {
        var computed = prop = new ComputedAsync<TValue>(defaultValue);

        AsyncAction wrapped = async ct =>
        {
            computed.IncrementRunning();
            try
            {
                computed.Set(await getter(ct));
            }
            catch (Exception e)
            {
                AsyncExceptionHandler?.Invoke(e);
            }
            finally
            {
                computed.DecrementRunning();
            }
        };

        wrapped.Invoke(_cts.Token);
        AsyncReactions += wrapped;

        return this;
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.CatchAsync(Action<Exception> handler)
    {
        AsyncExceptionHandler += handler;
        return this;
    }

    IDisposable IReactionBuilder<TRoot>.StartAsDisposable()
    {
        _root.Attach(Trigger);
        return this;
    }

    void IReactionBuilder<TRoot>.Start(ICompositeDisposable disposable) => disposable.AddDisposable(((IReactionBuilder<TRoot>)this).StartAsDisposable());

    protected abstract void Trigger();

    protected void RunReactions()
    {
        Reactions?.Invoke();

        if (AsyncReactions is null)
            return;

        _cts.Cancel();
        _cts = new();
        AsyncReactions.Invoke(_cts.Token);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _root.Dispose();
    }
}