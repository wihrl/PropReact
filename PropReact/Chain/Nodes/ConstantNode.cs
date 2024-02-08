namespace PropReact.Chain.Nodes;

class ConstantNode<TSource, TValue>(Func<TSource, TValue> getter, IRootNode root) : ChainNode<TValue>(root), INotifiableChainNode<TSource>
{
    void INotifiableChainNode<TSource>.ChangeSource(TSource? oldSource, TSource? newSource)
    {
        var oldValue = oldSource is null ? default : getter(oldSource);
        var newValue = newSource is null ? default : getter(newSource);

        foreach (var chainNode in Next)
            chainNode.ChangeSource(oldValue, newValue);
    }
}