using PropReact.Chain.Nodes;
using PropReact.Chain.Reactions;

public class ThrottledReaction<TRoot> : Reaction<TRoot>
{
    public required int Delay { get; init; }
    public required bool RunFirst { get; init; }

    public ThrottledReaction(RootNodeSource<TRoot> root) : base(root)
    {
    }

    protected override void Trigger()
    {
        throw new NotImplementedException();// todo
    }
}