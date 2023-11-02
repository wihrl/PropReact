// using PropReact.Chain.Nodes;
// using PropReact.Props;
// using PropReact.Props.Collections;
// using PropReact.Props.Value;
//
// namespace PropReact.Chain;
//
// // NOTES: just class methods wont work because <T>(IEnumerable<T>) takes precedence over <T>(IEnumerable<IEnumerable<T>>) 
//
//
// // the idea would be to have something 
// // todo: public API: Prop.Watch().Chain(...).Branch(...).Transform().ToProp(out _computed);
// public class ChainBuilder<TRoot, TValue> : FinalizationChainBuilder<TRoot, TValue>
// {
//     public ChainBuilder(TRoot root, RootNode<TRoot> rootNode, Func<TRoot, TValue> fullGetter) : base(root, rootNode,
//         fullGetter)
//     {
//         // todo: create node in constructor?
//     }
//
//     // simple IValueProp access
//     public ChainBuilder<TRoot, TNext> Then<TNext>(Func<TValue, IValueProp<TNext>> selector)
//     {
//         // var newNode = new ValueNode<TRoot, TNext>(selector, RootNode.Root);
//         // RootNode.Add(newNode);
//         // return new(newNode);
//         throw new NotImplementedException();
//     }
//
//     // IValueProp access where TValue is a collection
//     public CollectionChainBuilder<TRoot, IEnumerable<TNext>, TNext> Then<TNext>(Func<TValue, IValueProp<IEnumerable<TNext>>> selector)
//     {
//         // return new CollectionChainBuilder<IEnumerable<TNext>>()
//         // var newNode = new ValueNode<TRoot, IEnumerable<TNext>>(selector, _last.Root);
//         // _last.Add(newNode);
//         // return new ChainBuilder<IEnumerable<TValue>>(newNode).Enter();
//         throw new NotImplementedException();
//     }
//
//     // direct collection access
//     public CollectionChainBuilder<TRoot, IEnumerable<TNext>, TNext> Then<TNext>(Func<TValue, IEnumerable<TNext>> selector)
//     {
//         throw new NotImplementedException();
//         // var newNode = new CollectionNode<TRoot, IEnumerable<TValue>, TValue>(selector, _last.Root);
//         // _last.Add(newNode);
//         // return new(newNode);
//     }
//
//     // public KeyedCollectionChainBuilder<TRoot> Then<TNext, TKey>(
//     //     Func<TValue, IKeyedCollectionProp<TNext, TKey>> selector, TKey key)
//     //     where TKey : notnull
//     // {
//     //     // var newNode = new KeyedNode<TRoot, TValue, TKey>(selector, _last.Root, key);
//     //     // _last.Add(newNode);
//     //     // return new(newNode)
//     //     throw new NotImplementedException();
//     // }
//
//     public ChainBuilder<TRoot, Tuple<TNext1, TNext2>> Branch<TNext1, TNext2>(
//         Func<ChainBuilder<TRoot, TValue>, ChainBuilder<TRoot, TNext1>> b1,
//         Func<ChainBuilder<TRoot, TValue>, ChainBuilder<TRoot, TNext2>> b2)
//     {
//         throw new NotImplementedException();
//     }
// }
//
// public class CollectionChainBuilder<TRoot, TCollection, TValue> : FinalizationChainBuilder<TRoot, TCollection>
// {
//
//     public ChainBuilder<TRoot, TValue> Enter()
//     {
//         throw new NotImplementedException();
//         // var newNode = new CollectionNode<IEnumerable<TValue>, IEnumerable<TValue>, TValue>(x => x, builder._last.Root);
//         // builder._last.Add(newNode);
//         // return new(newNode);
//     }
//
//     public CollectionChainBuilder(TRoot root, RootNode<TRoot> rootNode, Func<TRoot, TCollection> fullGetter) : base(root, rootNode, fullGetter)
//     {
//     }
// }
//
// // public class KeyedCollectionChainBuilder<TRoot, TCollection, TValue, TKey> : CollectionChainBuilder<TRoot, TCollection, TValue> 
// // {
// //     public ChainBuilder<TRoot, TValue> Enter()
// //     {
// //         throw new NotImplementedException();
// //         // var newNode = new CollectionNode<IEnumerable<TValue>, IEnumerable<TValue>, TValue>(x => x, builder._last.Root);
// //         // builder._last.Add(newNode);
// //         // return new(newNode);
// //     }
// //
// //     public KeyedCollectionChainBuilder(TRoot root, RootNode<TRoot> rootNode, Func<TRoot, TValue> fullGetter) : base(root, rootNode, fullGetter)
// //     {
// //     }
// // }
//
// public class FinalizationChainBuilder<TRoot, TValue>
// {
//     private TRoot _root;
//     private RootNode<TRoot> _rootNode;
//
//     private Func<TRoot, TValue> _fullGetter;
//
//     public FinalizationChainBuilder(TRoot root, RootNode<TRoot> rootNode, Func<TRoot, TValue> fullGetter)
//     {
//         _root = root;
//         _rootNode = rootNode;
//         _fullGetter = fullGetter;
//     }
//
//     public IComputed<TValue> AsComputed()
//     {
//         IComputed<TValue> prop = new ComputedValueProp<TValue>(_fullGetter(_root));
//         _rootNode.Initialize(() => prop.Set(_fullGetter(_root)));
//         return prop;
//     }
// }
//
// // public static class ChainBuilderExtensions
// // {
// //     // todo: validate using analyzer
// //
// //     public static ChainBuilder<TValue> Enter<TValue>(this ChainBuilder<IEnumerable<TValue>> builder)
// //     {
// //         var newNode = new CollectionNode<IEnumerable<TValue>, IEnumerable<TValue>, TValue>(x => x, builder._last.Root);
// //         builder._last.Add(newNode);
// //         return new(newNode);
// //     }
// //     
// //     public static ChainBuilder<TValue> Enter<TValue>(this ChainBuilder<IListProp<TValue>> builder)
// //     {
// //         var newNode = new CollectionNode<IListProp<TValue>, IListProp<TValue>, TValue>(x => x, builder._last.Root);
// //         builder._last.Add(newNode);
// //         return new(newNode);
// //     }
// //
// //     public static ChainBuilder<TValue> EnterAt<TCollection, TValue, TKey>(
// //         this ChainBuilder<IKeyedCollectionProp<TValue, TKey>> builder, TKey key) where TKey : notnull
// //     {
// //         var newNode = new KeyedNode<IKeyedCollectionProp<TValue, TKey>, TValue, TKey>(x => x, builder._last.Root, key);
// //         builder._last.Add(newNode);
// //         return new(newNode);
// //     }
// //     
// //     public static ChainBuilder<TValue> EnterAt<TValue, TKey>(
// //         this ChainBuilder<IMap<TValue, TKey>> builder, TKey key) where TKey : notnull
// //     {
// //         var newNode = new KeyedNode<IMap<TValue, TKey>, TValue, TKey>(x => x, builder._last.Root, key);
// //         builder._last.Add(newNode);
// //         return new(newNode);
// //     }
// // }

// need TMainRoot to store getter and determine whether or not is the current node finalizable (prevent finalize in branches) 

using PropReact.Chain.Nodes;
using PropReact.Props.Collections;
using PropReact.Props.Value;

// public class ValueChainBuilder<TMainRoot, TBranchType, TValue> : ChainBuilder<TMainRoot, TBranchType, TValue>
// {
// }

public interface IBranchType
{
}

public struct RootBranch : IBranchType
{
}

public struct InnerBranch : IBranchType
{
}

public struct ChainBuilder<TRoot, TBranchType, TValue, TResult> where TBranchType : IBranchType
{
    public required RootNode<TRoot> RootNode { get; init; }
    public required ChainNode<TValue> Node { get; init; }

    public required Func<TRoot, TResult> ChainGetter { get; init; }
    //public Func<TRoot, TValue> GetterTransformer { get; init; }
}

// public class ChainGetterBuilder<TRoot, TValue>
// {
//     public required Func<TRoot, TValue> ChainGetter { get; init; }
//
//     public ChainGetterBuilder<TRoot, TValue> AppendValue<TNext>(Func<TValue, TNext> func)
//     {
//         return x => func(ChainGetter(x));
//     }
//     
//     public ChainGetterBuilder<TRoot, IEnumerable<TNext>> AppendCollection<TNext>(Func<TValue, IEnumerable<TNext>> func)
//     {
//         return x => func(ChainGetter(x));
//     }
// }

public static class ChainBuilderActions
{
    public static Func<TRoot, TNext> Append<TRoot, TValue, TNext>(this Func<TRoot, TValue> start,
        Func<TValue, TNext> selector) =>
        root => selector(start(root));

    public static Func<TRoot, TNext> AppendValue<TRoot, TRes, TNext>(this Func<TRoot, TRes> start,
        Func<TRes, IValueProp<TNext>> selector) =>
        root => selector(start(root)).Value;

    public static Func<TRoot, IEnumerable<TNext>> Append<TRoot, TValue, TNext>(
        this Func<TRoot, IEnumerable<TValue>> start,
        Func<TValue, IValueProp<TNext>> selector) =>
        root => start(root).Select(x => selector(x).Value);

    #region Values

    public static ChainBuilder<TMainRoot, TBranchType, TNext, TNext> Then<TMainRoot, TBranchType, TNext, TRes>(
        this ChainBuilder<TMainRoot, TBranchType, TRes, TRes> builder, Func<TRes, IValueProp<TNext>> selector)
        where TBranchType : IBranchType
    {
        return new()
        {
            RootNode = builder.RootNode,
            Node = new ValueNode<TRes, TNext>(selector, builder.RootNode),
            ChainGetter = builder.ChainGetter.AppendValue(selector) //x => selector(builder.ChainGetter(x)).Value
        };
    }
    
    public static ChainBuilder<TMainRoot, TBranchType, TNext, IEnumerable<TNext>> Then2<TMainRoot, TBranchType, TValue, TNext>(
        this ChainBuilder<TMainRoot, TBranchType, TValue, IEnumerable<TValue>> builder, Func<TValue, IValueProp<TNext>> selector)
        where TBranchType : IBranchType
    {
        return new()
        {
            RootNode = builder.RootNode,
            Node = new ValueNode<TValue, TNext>(selector, builder.RootNode),
            ChainGetter = builder.ChainGetter.Append(selector)
        };
    }

    public static ChainBuilder<TMainRoot, TBranchType, TNext, TNext> ThenConstant<TMainRoot, TBranchType, TValue, TNext, TRes>(
        this ChainBuilder<TMainRoot, TBranchType, TValue, TRes> builder, Func<TValue, TNext> selector)
        where TBranchType : IBranchType
    {
        return new()
        {
            RootNode = builder.RootNode,
            Node = new ConstantNode<TValue, TNext>(selector, builder.RootNode),
            ChainGetter = builder.ChainGetter.Append(selector)
        };
    }

    #endregion

    #region Collections

    private static ChainBuilder<TMainRoot, TBranchType, TValue, IEnumerable<TValue>>
        EnterExplicit<TMainRoot, TBranchType, TSet, TValue, TResult>(
            this ChainBuilder<TMainRoot, TBranchType, TSet, TResult> builder)
        where TSet : class, IEnumerable<TValue> where TBranchType : IBranchType
    {
        return new()
        {
            RootNode = builder.RootNode,
            Node = new CollectionNode<TSet, TValue>(builder.RootNode),
            ChainGetter = builder.ChainGetter
        };
    }

    // type inference proxies
    public static ChainBuilder<TMainRoot, TBranchType, TValue, IEnumerable<TValue>> Enter<TMainRoot, TBranchType,
        TValue, TResult>(
        this ChainBuilder<TMainRoot, TBranchType, IEnumerable<TValue>, TResult> builder)
        where TBranchType : IBranchType =>
        builder.EnterExplicit<TMainRoot, TBranchType, IEnumerable<TValue>, TValue, TResult>();

    public static ChainBuilder<TMainRoot, TBranchType, TValue, IEnumerable<TValue>> Enter<TMainRoot, TBranchType, TValue, TResult>(
        this ChainBuilder<TMainRoot, TBranchType, IListProp<TValue>, TResult> builder) where TBranchType : IBranchType =>
        builder.EnterExplicit<TMainRoot, TBranchType, IListProp<TValue>, TValue, TResult>();

    public static ChainBuilder<TMainRoot, TBranchType, TValue, IEnumerable<TValue>> Enter<TMainRoot, TBranchType, TValue, TKey, TResult>(
        this ChainBuilder<TMainRoot, TBranchType, IMap<TValue, TKey>, TResult> builder)
        where TKey : notnull where TBranchType : IBranchType =>
        builder.EnterExplicit<TMainRoot, TBranchType, IMap<TValue, TKey>, TValue, TResult>();

    #endregion

    #region Maps

    public static ChainBuilder<TMainRoot, TBranchType, TValue, TValue> EnterAt<TMainRoot, TBranchType, TValue, TKey,
        TResult>(
        this ChainBuilder<TMainRoot, TBranchType, IMap<TValue, TKey>, TResult> builder, TKey key)
        where TKey : notnull where TBranchType : IBranchType =>
        throw new NotImplementedException();
        //builder.Enter<TMainRoot, TBranchType, IMap<TValue, TKey>, TValue, TResult>();

    #endregion

    #region Branching

    // todo: should return finalizable
    public static ChainBuilder<TMainRoot, RootBranch, IEnumerable<(TNext1, TNext2)>, IEnumerable<(TRes1, TRes2)>>
        Branch<TMainRoot, TBranchType, TValue, TRes, TNext1, TRes1, TNext2, TRes2>(
        this ChainBuilder<TMainRoot, TBranchType, TValue, TRes> builder,
        Func<ChainBuilder<TMainRoot, InnerBranch, TValue, TRes>, ChainBuilder<TMainRoot, InnerBranch, TNext1, TRes1>> selector1,
        Func<ChainBuilder<TMainRoot, InnerBranch, TValue, TRes>, ChainBuilder<TMainRoot, InnerBranch, TNext2, TRes2>> selector2)
        where TBranchType : IBranchType =>
        throw new NotImplementedException();

    #endregion

    #region Finalization

    public static IComputed<TRes> AsComputed<TMainRoot, TValue, TRes>(
        this ChainBuilder<TMainRoot, RootBranch, TValue, TRes> builder) =>
        throw new NotImplementedException();

    #endregion
}