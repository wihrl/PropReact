namespace PropReact.Blazor;

/// <summary>
/// Same as <see cref="ReactiveComponent"/>, but only renders when a dependency changes.
/// It will not rerender if the parent component re-renders (in most cases).
/// </summary>
public class ExclusiveReactiveComponent : ReactiveComponent
{
    // only render if change counter > 0 and if rendering was allowed, reset to 0
    protected sealed override bool ShouldRender() => Interlocked.Exchange(ref _dependencyChanges, 0) > 0;

    protected sealed override void OnAfterRender(bool firstRender)
    {
        // if dependencies changed while rendering, re-render
        if (Interlocked.Exchange(ref _dependencyChanges, 0) > 0)
        {
            // make sure ShouldRender returns true
            Interlocked.Increment(ref _dependencyChanges);
            InvokeAsync(StateHasChanged);
        }
        
        OnAfterRenderInternal(firstRender);
    }
    
    protected virtual void OnAfterRenderInternal(bool firstRender)
    {
    } 
}