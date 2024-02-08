using PropReact.Chain.Nodes;
using PropReact.Props.Value;

namespace PropReact.Chain.Reactions;

using AsyncAction = Func<CancellationToken, ValueTask>;

public interface IReactionBuilder<TRoot>
{
    IReactionBuilder<TRoot> React(Action reaction, bool runNow = false);
    IReactionBuilder<TRoot> ReactAsync(Func<CancellationToken, ValueTask> action, bool runNow = false);
    IReactionBuilder<TRoot> Compute<TValue>(Func<TValue> getter, out IComputed<TValue> prop);
    IReactionBuilder<TRoot> Compute<TValue>(Func<TValue> getter, IComputed<TValue> prop);
    IReactionBuilder<TRoot> ComputeAsync<TValue>(Func<CancellationToken, ValueTask<TValue>> getter, out IComputedAsync<TValue> prop, TValue defaultValue);
    IReactionBuilder<TRoot> ComputeAsync<TValue>(Func<CancellationToken, ValueTask<TValue>> getter, IComputedAsync<TValue> prop);
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
        prop = new Computed<TValue>(getter());
        ((IReactionBuilder<TRoot>)this).Compute(getter, prop);
        return this;
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.Compute<TValue>(Func<TValue> getter, IComputed<TValue> prop)
    {
        var setter = () => prop.Set(getter());
        setter.Invoke();
        Reactions += setter;
        return this;
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.ComputeAsync<TValue>(
        Func<CancellationToken, ValueTask<TValue>> getter, out IComputedAsync<TValue> prop, TValue defaultValue)
    {
        prop = new ComputedAsync<TValue>(defaultValue);
        ((IReactionBuilder<TRoot>)this).ComputeAsync(getter, prop);
        return this;
    }

    IReactionBuilder<TRoot> IReactionBuilder<TRoot>.ComputeAsync<TValue>(
        Func<CancellationToken, ValueTask<TValue>> getter, IComputedAsync<TValue> prop)
    {
        AsyncAction wrapped = async ct =>
        {
            prop.IncrementRunning();
            try
            {
                prop.Set(await getter(ct));
            }
            catch (Exception e)
            {
                AsyncExceptionHandler?.Invoke(e);
            }
            finally
            {
                prop.DecrementRunning();
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

    void IReactionBuilder<TRoot>.Start(ICompositeDisposable disposable) => disposable.Add(((IReactionBuilder<TRoot>)this).StartAsDisposable());

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