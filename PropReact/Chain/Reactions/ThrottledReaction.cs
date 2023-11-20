using PropReact.Chain.Nodes;

namespace PropReact.Chain.Reactions;

class ThrottledReaction<TRoot> : Reaction<TRoot>
{
    public required int Delay { get; init; }
    public required bool RunFirst { get; init; }

    public ThrottledReaction(RootNode<TRoot> root) : base(root)
    {
    }

    protected override void Trigger()
    {
        throw new NotImplementedException();// todo
    }
}