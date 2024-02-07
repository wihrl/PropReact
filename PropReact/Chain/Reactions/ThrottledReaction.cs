using System.Diagnostics;
using PropReact.Chain.Nodes;

namespace PropReact.Chain.Reactions;

sealed class ThrottledReaction<TRoot> : Reaction<TRoot>
{
    public required bool Immediate { get; init; }
    public required int Timeout { get; init; }
    public required bool ResetTimeoutOnTrigger { get; init; }

    private object _lock = new();
    private long _triggerStamp;

    public ThrottledReaction(RootNode<TRoot> root) : base(root)
    {
    }

    private Task? _task;

    protected override void Trigger()
    {
        if (!Monitor.TryEnter(_lock)) return;

        try
        {
            _triggerStamp = Stopwatch.GetTimestamp();

            if (_task is null)
            {
                if (Immediate) TriggerReactions();
                _task = StartTimeout();
            }
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    async Task StartTimeout()
    {
        long lastStamp;
        do
        {
            lastStamp = _triggerStamp;

            var elapsed = Stopwatch.GetElapsedTime(lastStamp).Milliseconds;

            if (elapsed >= Timeout)
                break;

            await Task.Delay(Timeout - elapsed);
        } while (ResetTimeoutOnTrigger && lastStamp != _triggerStamp);
        
        _task = null;
        TriggerReactions();
    }
}