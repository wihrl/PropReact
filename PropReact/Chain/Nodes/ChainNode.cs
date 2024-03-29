﻿namespace PropReact.Chain.Nodes;

abstract class ChainNode<TValue>(IRootNode root)
{
    protected readonly IRootNode Root = root;

    protected readonly List<INotifiableChainNode<TValue>> Next = new();
    internal void Chain(INotifiableChainNode<TValue> next) => Next.Add(next);
}


interface INotifiableChainNode<TSource>
{
    void ChangeSource(TSource? oldSource, TSource? newSource);
}