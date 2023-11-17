using PropReact.Chain.Nodes;

namespace PropReact.Chain.Reactions;

class ImmediateReaction<TRoot> : Reaction<TRoot>
{
    protected override void Trigger()
    {
        TriggerReactions();
        TriggerAsyncReactions();
    }

    public ImmediateReaction(RootNode<TRoot> root) : base(root)
    {
    }
}