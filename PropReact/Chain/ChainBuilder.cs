using System.Runtime.CompilerServices;
using PropReact.Chain.Nodes;
using PropReact.Chain.Reactions;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Chain;

public interface IBranchType
{
}

public struct RootBranch : IBranchType
{
}

public struct InnerBranch : IBranchType
{
}

public class ChainBuilder<TRoot, TBranchType, TValue> where TBranchType : IBranchType
{
    internal ChainBuilder(RootNodeSource<TRoot> rootNodeSource, ChainNode<TValue> node)
    {
        RootNodeSource = rootNodeSource;
        Node = node;
    }

    internal RootNodeSource<TRoot> RootNodeSource { get; }
    internal ChainNode<TValue> Node { get; }
}

public static class ChainBuilder
{
    #region Values

    public static ChainBuilder<TMainRoot, TBranchType, TNext> ChainValue<TMainRoot, TBranchType, TValue, TNext>(
        this ChainBuilder<TMainRoot, TBranchType, TValue> builder, Func<TValue, IValueProp<TNext?>> selector,
        [CallerArgumentExpression(nameof(selector))]
        string? expression = null)
        where TBranchType : IBranchType
    {
#if DEBUG
        if (expression?.Count(x => x == '.') != 1)
            throw new ArgumentException(
                """
                Appending a non-constant node to a reactive chain must be done one property at a time.
                Expressions such as x => x.Prop1.Value.Prop2.Value are not allowed and should be replaced with 2 separate .Then() calls.
                """,
                nameof(selector));
#endif

        var nextNode = new ValueNodeSource<TValue, TNext>(selector, builder.RootNodeSource);
        builder.Node.Chain(nextNode);

        return new(
            builder.RootNodeSource,
            nextNode
        );
    }

    public static ChainBuilder<TMainRoot, TBranchType, TNext> ChainConstant<TMainRoot, TBranchType, TValue, TNext>(
        this ChainBuilder<TMainRoot, TBranchType, TValue> builder, Func<TValue, TNext> selector)
        where TBranchType : IBranchType
    {
        var nextNode = new ConstantNodeSource<TValue, TNext>(selector, builder.RootNodeSource);
        builder.Node.Chain(nextNode);

        return new(
            builder.RootNodeSource,
            nextNode
        );
    }

    #endregion

    #region Collections

    public static ChainBuilder<TMainRoot, TBranchType, TValue>
        EnterExplicit<TMainRoot, TBranchType, TSet, TValue>(ChainBuilder<TMainRoot, TBranchType, TSet> builder)
        where TSet : class, IEnumerable<TValue> where TBranchType : IBranchType
    {
        var nextNode = new CollectionNodeSource<TSet, TValue>(builder.RootNodeSource);
        builder.Node.Chain(nextNode);

        return new(
            builder.RootNodeSource,
            nextNode
        );
    }


    #region 'Type inference' aliases

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType,
        TValue>(
        this ChainBuilder<TMainRoot, TBranchType, IEnumerable<TValue>> builder)
        where TBranchType : IBranchType =>
        EnterExplicit<TMainRoot, TBranchType, IEnumerable<TValue>, TValue>(builder);

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType, TValue>(
        this ChainBuilder<TMainRoot, TBranchType, IReactiveList<TValue>> builder) where TBranchType : IBranchType =>
        EnterExplicit<TMainRoot, TBranchType, IReactiveList<TValue>, TValue>(builder);

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType, TValue>(
        this ChainBuilder<TMainRoot, TBranchType, ReactiveList<TValue>> builder) where TBranchType : IBranchType =>
        EnterExplicit<TMainRoot, TBranchType, ReactiveList<TValue>, TValue>(builder);

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType, TValue, TKey>(
        this ChainBuilder<TMainRoot, TBranchType, IReactiveMap<TValue, TKey>> builder)
        where TKey : notnull where TBranchType : IBranchType =>
        EnterExplicit<TMainRoot, TBranchType, IReactiveMap<TValue, TKey>, TValue>(builder);

    #endregion

    #endregion

    #region Maps

    public static ChainBuilder<TMainRoot, TBranchType, TValue> EnterAt<TMainRoot, TBranchType, TValue, TKey>(
        this ChainBuilder<TMainRoot, TBranchType, IReactiveMap<TValue, TKey>> builder, TKey key)
        where TKey : notnull where TBranchType : IBranchType =>
        throw new NotImplementedException();
    //builder.Enter<TMainRoot, TBranchType, IMap<TValue, TKey>, TValue>();

    #endregion

    #region Branching

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Branch<TMainRoot, TBranchType, TValue, TNext1, TNext2>(
        this ChainBuilder<TMainRoot, TBranchType, TValue> builder,
        Func<ChainBuilder<TMainRoot, InnerBranch, TValue>, ChainBuilder<TMainRoot, InnerBranch, TNext1>> selector1,
        Func<ChainBuilder<TMainRoot, InnerBranch, TValue>, ChainBuilder<TMainRoot, InnerBranch, TNext2>> selector2)
        where TBranchType : IBranchType
    {
        var innerBuilder = new ChainBuilder<TMainRoot, InnerBranch, TValue>(builder.RootNodeSource, builder.Node);

        selector1(innerBuilder);
        selector2(innerBuilder);

        return new(
            builder.RootNodeSource,
            builder.Node
        );
    }

    #endregion

    #region Reactions

    public static IReactionBuilder<TMainRoot> Immediate<TMainRoot, TValue>(this ChainBuilder<TMainRoot, RootBranch, TValue> builder) =>
        new ImmediateReaction<TMainRoot>(builder.RootNodeSource);

    public static IReactionBuilder<TMainRoot> Throttled<TMainRoot, TValue>(
        this ChainBuilder<TMainRoot, RootBranch, TValue> builder,
        int delay, bool runFirst = false) =>
        new ThrottledReaction<TMainRoot>(builder.RootNodeSource) { Delay = delay, RunFirst = runFirst };

    #endregion
}