﻿using PropReact.Chain.Nodes;
using PropReact.Props;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Chain;

// the idea would be to have something 
// todo: public API: Prop.Watch().Chain(...).Branch(...).Transform().ToProp(out _computed);
public class ChainBuilder<TRoot, TValue> : FinalizationChainBuilder<TRoot, TValue>
{
    public ChainBuilder(TRoot root, RootNode<TRoot> rootNode, Func<TRoot, TValue> fullGetter) : base(root, rootNode,
        fullGetter)
    {
        // todo: create node in constructor?
    }

    // simple IValueProp access
    public ChainBuilder<TRoot, TNext> Then<TNext>(Func<TValue, IValueProp<TNext>> selector)
    {
        // var newNode = new ValueNode<TRoot, TNext>(selector, RootNode.Root);
        // RootNode.Add(newNode);
        // return new(newNode);
        throw new NotImplementedException();
    }

    // IValueProp access where TValue is a collection
    public CollectionChainBuilder<TRoot, IEnumerable<TNext>, TNext> Then<TNext>(Func<TValue, IValueProp<IEnumerable<TNext>>> selector)
    {
        // return new CollectionChainBuilder<IEnumerable<TNext>>()
        // var newNode = new ValueNode<TRoot, IEnumerable<TNext>>(selector, _last.Root);
        // _last.Add(newNode);
        // return new ChainBuilder<IEnumerable<TValue>>(newNode).Enter();
        throw new NotImplementedException();
    }

    // direct collection access
    public CollectionChainBuilder<TRoot, IEnumerable<TNext>, TNext> Then<TNext>(Func<TValue, IEnumerable<TNext>> selector)
    {
        throw new NotImplementedException();
        // var newNode = new CollectionNode<TRoot, IEnumerable<TValue>, TValue>(selector, _last.Root);
        // _last.Add(newNode);
        // return new(newNode);
    }

    public KeyedCollectionChainBuilder<TRoot> Then<TNext, TKey>(
        Func<TValue, IKeyedCollectionProp<TNext, TKey>> selector, TKey key)
        where TKey : notnull
    {
        // var newNode = new KeyedNode<TRoot, TValue, TKey>(selector, _last.Root, key);
        // _last.Add(newNode);
        // return new(newNode);
    }

    public ChainBuilder<TRoot, Tuple<TNext1, TNext2>> Branch<TNext1, TNext2>(
        Func<ChainBuilder<TRoot, TValue>, ChainBuilder<TRoot, TNext1>> b1,
        Func<ChainBuilder<TRoot, TValue>, ChainBuilder<TRoot, TNext2>> b2)
    {
        throw new NotImplementedException();
    }
}

public class CollectionChainBuilder<TRoot, TCollection, TValue> : FinalizationChainBuilder<TRoot, TCollection>
{

    public ChainBuilder<TRoot, TValue> Enter()
    {
        throw new NotImplementedException();
        // var newNode = new CollectionNode<IEnumerable<TValue>, IEnumerable<TValue>, TValue>(x => x, builder._last.Root);
        // builder._last.Add(newNode);
        // return new(newNode);
    }

    public CollectionChainBuilder(TRoot root, RootNode<TRoot> rootNode, Func<TRoot, TCollection> fullGetter) : base(root, rootNode, fullGetter)
    {
    }
}

public class KeyedCollectionChainBuilder<TRoot, TCollection, TValue, TKey> : CollectionChainBuilder<TRoot, TCollection, TValue> 
{
    public ChainBuilder<TRoot, TValue> Enter()
    {
        throw new NotImplementedException();
        // var newNode = new CollectionNode<IEnumerable<TValue>, IEnumerable<TValue>, TValue>(x => x, builder._last.Root);
        // builder._last.Add(newNode);
        // return new(newNode);
    }

    public KeyedCollectionChainBuilderr(TCollection root, RootNode<TCollection> rootNode, Func<TCollection, TValue> fullGetter) : base(root, rootNode, fullGetter)
    {
    }
}

public class FinalizationChainBuilder<TRoot, TValue>
{
    private TRoot _root;
    private RootNode<TRoot> _rootNode;

    private Func<TRoot, TValue> _fullGetter;

    public FinalizationChainBuilder(TRoot root, RootNode<TRoot> rootNode, Func<TRoot, TValue> fullGetter)
    {
        _root = root;
        _rootNode = rootNode;
        _fullGetter = fullGetter;
    }

    public IComputed<TValue> AsComputed()
    {
        IComputed<TValue> prop = new ComputedValueProp<TValue>(_fullGetter(_root));
        _rootNode.Initialize(() => prop.Set(_fullGetter(_root)));
        return prop;
    }
}

// public static class ChainBuilderExtensions
// {
//     // todo: validate using analyzer
//
//     public static ChainBuilder<TValue> Enter<TValue>(this ChainBuilder<IEnumerable<TValue>> builder)
//     {
//         var newNode = new CollectionNode<IEnumerable<TValue>, IEnumerable<TValue>, TValue>(x => x, builder._last.Root);
//         builder._last.Add(newNode);
//         return new(newNode);
//     }
//     
//     public static ChainBuilder<TValue> Enter<TValue>(this ChainBuilder<IListProp<TValue>> builder)
//     {
//         var newNode = new CollectionNode<IListProp<TValue>, IListProp<TValue>, TValue>(x => x, builder._last.Root);
//         builder._last.Add(newNode);
//         return new(newNode);
//     }
//
//     public static ChainBuilder<TValue> EnterAt<TCollection, TValue, TKey>(
//         this ChainBuilder<IKeyedCollectionProp<TValue, TKey>> builder, TKey key) where TKey : notnull
//     {
//         var newNode = new KeyedNode<IKeyedCollectionProp<TValue, TKey>, TValue, TKey>(x => x, builder._last.Root, key);
//         builder._last.Add(newNode);
//         return new(newNode);
//     }
//     
//     public static ChainBuilder<TValue> EnterAt<TValue, TKey>(
//         this ChainBuilder<IMap<TValue, TKey>> builder, TKey key) where TKey : notnull
//     {
//         var newNode = new KeyedNode<IMap<TValue, TKey>, TValue, TKey>(x => x, builder._last.Root, key);
//         builder._last.Add(newNode);
//         return new(newNode);
//     }
// }