namespace PropReact.Props.Value;

public interface IComputedAsync<TValue> : IComputed<TValue>
{
    public IComputed<bool> IsRunning { get; }
    internal void IncrementRunning();
    internal void DecrementRunning();
}