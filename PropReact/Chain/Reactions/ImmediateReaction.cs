using PropReact.Chain.Nodes;

namespace PropReact.Chain.Reactions;

public class ImmediateReaction<TRoot> : Reaction<TRoot>
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