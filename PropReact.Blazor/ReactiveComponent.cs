using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using PropReact.Props;
using PropReact.Props.Collections;
using PropReact.Props.Value;
using PropReact.Utils;

namespace PropReact.Blazor;

public class ReactiveComponent : ComponentBase, IDisposable
{
    protected CompositeDisposable Disposables { get; private set; } = new();
    internal int _dependencyChanges = 1;

    public void Dispose()
    {
        Disposables.Dispose();
        OnDispose();
    }

    protected virtual void OnDispose()
    {
    }

    // Because rendering can happen conditionally, gathering dependencies during the first render is not sufficient.
    // Instead, each dependency must be tracked individually.
    readonly List<object> _dependencies = new(); // List.Contains is marginally faster than HashSet.Contains for small collections

    protected TValue Watch<TValue>(IValue<TValue> prop, [CallerArgumentExpression(nameof(prop))] string expression = "")
    {
        if (_dependencies.Contains(prop)) return prop.Value;
        ValidateExpression(expression);
        ObserveProp(prop);
        return prop.Value;
    }

    protected IReactiveCollection<TValue, TKey> Watch<TValue, TKey>(IReactiveCollection<TValue, TKey> prop,
        [CallerArgumentExpression(nameof(prop))] string expression = "")
    {
        if (_dependencies.Contains(prop)) return prop;
        ValidateExpression(expression);
        ObserveProp(prop);
        return prop;
    }

    protected IMutable<TValue> Bind<TValue>(IMutable<TValue> prop, [CallerArgumentExpression(nameof(prop))] string expression = "")
    {
        if (_dependencies.Contains(prop)) return prop;
        ValidateExpression(expression);
        ObserveProp(prop);
        return prop;
    }

    private void ObserveProp<TValue>(IProp<TValue> prop)
    {
        var observer = new GenericObserver<TValue>(prop, (_, _) =>
        {
            Interlocked.Increment(ref _dependencyChanges);
            InvokeAsync(StateHasChanged);
        });
        observer.Start();
        _dependencies.Add(prop);
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
            Example: ChainBuilder.From(this).ChainValue(x => x.Prop1).ChainValue(x => x.Prop2).Compute(...).Start(...);
            """);
#endif
    }
}