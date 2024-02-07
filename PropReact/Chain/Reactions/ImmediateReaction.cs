using PropReact.Chain.Nodes;

namespace PropReact.Chain.Reactions;

sealed class ImmediateReaction<TRoot>(RootNode<TRoot> root) : Reaction<TRoot>(root)
{
    protected override void Trigger() => RunReactions();
}