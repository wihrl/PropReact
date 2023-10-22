using System.Linq.Expressions;

namespace PropReact.Reactivity;

internal partial class PropChain : IDisposable
{
    
    public IReadOnlyCollection<ChainNode> StartingNodes => _startingNodes;
    private readonly List<ChainNode> _startingNodes = new();
    private readonly object _owner;

    internal PropChain(object owner) => _owner = owner;

    private ChainNode GetStartingNode(IMember member, Action action)
    {
        var node = _startingNodes.FirstOrDefault(x => x.Member == member);
        if (node is not null) return node;

        node = new(member, action);
        _startingNodes.Add(node);
        return node;
    }

    private void Init()
    {
        foreach (var startingNode in _startingNodes) startingNode.InvalidateSingle(null, startingNode.GetValue(_owner));
    }

    public void Dispose()
    {
        foreach (var startingNode in _startingNodes) startingNode.InvalidateSingle(startingNode.GetValue(_owner), null);
    }

    public static PropChain Parse(object owner, Expression expression, Action action) =>
        new ChainParser(owner, expression, action).ParseChain();
}