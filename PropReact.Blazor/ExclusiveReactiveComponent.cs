namespace PropReact.Blazor;

/// <summary>
/// Same as <see cref="ReactiveComponent"/>, but only renders when a dependency changes.
/// It will not rerender if the parent component re-renders.
/// </summary>
public class ExclusiveReactiveComponent : ReactiveComponent
{
    protected sealed override bool ShouldRender() => Interlocked.Exchange(ref _dependencyChanges, 0) > 0;

    protected override void OnAfterRenderInternal(bool firstRender)
    {
        // make sure no changes are missed
        if (Interlocked.Exchange(ref _dependencyChanges, 0) > 0)
        {
            Interlocked.Increment(ref _dependencyChanges);
            InvokeAsync(StateHasChanged);
        }
    }
}