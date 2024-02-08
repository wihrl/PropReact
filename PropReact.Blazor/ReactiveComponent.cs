using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Blazor;

public class ReactiveComponent : ComponentBase, IDisposable
{
    protected CompositeDisposable Disposables { get; } = new();
    private bool _firstRender = true;
    internal int _dependencyChanges = 1;

    public void Dispose()
    {
        Disposables.Dispose();
        OnDispose();
    }

    protected virtual void OnDispose()
    {
    }

    protected sealed override void OnAfterRender(bool firstRender)
    {
        if (_firstRender) _firstRender = false;
        OnAfterRenderInternal(firstRender);
    }

    protected virtual void OnAfterRenderInternal(bool firstRender)
    {
    }

    protected TValue Watch<TValue>(IValue<TValue> prop, [CallerArgumentExpression(nameof(prop))] string expression = "")
    {
        if (!_firstRender) return prop.Value;
        ValidateExpression(expression);
        ObserveProp(prop);
        return prop.Value;
    }

    protected IMutable<TValue> Bind<TValue>(IMutable<TValue> prop, [CallerArgumentExpression(nameof(prop))] string expression = "")
    {
        if (!_firstRender) return prop;
        ValidateExpression(expression);
        ObserveProp(prop);
        return prop;
    }

    private void ObserveProp<TValue>(IValue<TValue> prop)
    {
        var observer = new GenericObserver<TValue>(prop, (_, _) =>
        {
            Interlocked.Increment(ref _dependencyChanges);
            InvokeAsync(StateHasChanged);
        });
        observer.Start();
        Disposables.Add(observer);
    }

    void ValidateExpression(string expression)
    {
#if DEBUG
        var count = expression.Count(x => x == '.');
        if (count == 0)
            return;

        if (count == 1 && expression.EndsWith(nameof(IComputedAsync<object>.IsRunning)))
            return;

        throw new ArgumentException(
            """
            Only locally defined readonly props can be watched.
            Expressions such as Prop1.Value.Prop2.Value are not allowed and should be replaced with a computed prop.
            Example: Chain.From(this).ChainValue(x => x.Prop1).ChainValue(x => x.Prop2).Compute(...).Start(...);
            """);
#endif
    }
}