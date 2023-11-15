using PropReact.Chain.Nodes;

namespace PropReact.Chain.Reactions;

internal class ImmediateReaction<TRoot> : Reaction<TRoot>
{
    protected override void Trigger()
    {
        TriggerReactions();
        TriggerAsyncReactions();
    }

    public ImmediateReaction(RootNodeSource<TRoot> root) : base(root)
    {
    }
}