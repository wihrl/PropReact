using System.Collections;

namespace PropReact.Reactivity;

// todo: Support property.list.property (list.count, ...) (only sub to list if it is at the end)
internal class ChainNode : IPropObserver<>
{
    internal List<ChainNode> _next = new();
    public readonly IMember Member;
    private readonly Action _onChange;
    private readonly bool _isCollection;

    public ChainNode(IMember member, Action onChange)
    {
        Member = member;
        _onChange = onChange;
        _isCollection =
            member.MemberType.IsAssignableTo(typeof(IEnumerable));
    }

    internal object? GetValue(object? root) => root is null ? null : Member.GetValue(root);

    internal ChainNode Continue(IMember member)
    {
        // todo: prevent duplicates
        var node = new ChainNode(member, _onChange);
        _next.Add(node);
        return node;
    }

    public void OwnedValueChanged(object? oldValue, object? newValue) // values of the next prop down the chain
    {
        // invalidate dependent nodes
        if (!_isCollection)
            foreach (var item in _next)
                item.InvalidateSingle(oldValue, newValue);
        else // owned value in the case of collections does not correspond to the next property of the chain - it's only the root
            foreach (var item in _next)
                item.InvalidateSingle(item.GetValue(oldValue), item.GetValue(newValue));

        Transaction.PostOrExecute(_onChange);
    }

    // todo: will not work if multiple same values are in the collection
    internal void InvalidateSingle(object? oldValue, object? newValue) // values of current prop
    {
        if (oldValue == newValue)
            return;

        (oldValue as IProp<>)?.Unsub(this);
        (newValue as IProp<>)?.Sub(this);

        if (_isCollection)
        {
            if (oldValue is not null)
            {
                foreach (var node in _next)
                foreach (var oldItem in (IEnumerable) oldValue)
                    node.InvalidateSingle(node.GetValue(oldItem), null);
            }

            if (newValue is not null)
            {
                foreach (var node in _next)
                foreach (var newItem in (IEnumerable) newValue)
                    node.InvalidateSingle(null, node.GetValue(newItem));
            }
        }
        else
            foreach (var node in _next)
                node.InvalidateSingle(node.GetValue(oldValue), node.GetValue(newValue));
    }

    public override string ToString()
    {
        var followup = _next.Count switch
        {
            0 => "",
            1 => " - " + _next[0],
            > 1 => $" - [{string.Join(", ", _next)}]",
            _ => throw new NotImplementedException()
        };


        return Member.MemberInfo.Name + followup;
    }
}