using PropReact.Chain.Nodes;

namespace PropReact.Chain.Reactions;

sealed class ImmediateReaction<TRoot> : Reaction<TRoot>
{
    protected override void Trigger() => RunReactions();

    public ImmediateReaction(RootNode<TRoot> root) : base(root)
    {
    }
}