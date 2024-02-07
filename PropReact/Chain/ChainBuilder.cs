using System.Runtime.CompilerServices;
using PropReact.Chain.Nodes;
using PropReact.Chain.Reactions;
using PropReact.Props;
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
    internal ChainBuilder(RootNode<TRoot> rootNode, ChainNode<TValue> node)
    {
        RootNode = rootNode;
        Node = node;
    }

    internal RootNode<TRoot> RootNode { get; }
    internal ChainNode<TValue> Node { get; }
}

public static class ChainBuilder
{
    #region Values

    public static ChainBuilder<TMainRoot, TBranchType, TNext> ChainValue<TMainRoot, TBranchType, TValue, TNext>(
        this ChainBuilder<TMainRoot, TBranchType, TValue> builder, Func<TValue, IValue<TNext?>> selector,
        [CallerArgumentExpression(nameof(selector))]
        string? expression = null)
        where TBranchType : IBranchType
    {
#if DEBUG
        // disallow chaining multiple properties at once except for IsRunning of async computed props
        if (expression?.Count(x => x == '.') > 1 && !expression.Contains(nameof(IComputedAsync<object>.IsRunning)))
            throw new ArgumentException(
                $"""
                 Appending a non-constant node to a reactive chain must be done one property at a time.
                 Expressions such as x => x.Prop1.Value.Prop2.Value are not allowed and should be replaced with 2 separate .ChainValue calls.
                 """,
                nameof(selector));
#endif

        var nextNode = new ValueNode<TValue, TNext>(selector, builder.RootNode);
        builder.Node.Chain(nextNode);

        return new(
            builder.RootNode,
            nextNode
        );
    }

    public static ChainBuilder<TMainRoot, TBranchType, TNext> ChainConstant<TMainRoot, TBranchType, TValue, TNext>(
        this ChainBuilder<TMainRoot, TBranchType, TValue> builder, Func<TValue, TNext> selector)
        where TBranchType : IBranchType
    {
        var nextNode = new ConstantNode<TValue, TNext>(selector, builder.RootNode);
        builder.Node.Chain(nextNode);

        return new(
            builder.RootNode,
            nextNode
        );
    }

    #endregion

    #region Collections

    public static ChainBuilder<TMainRoot, TBranchType, TValue> EnterExplicit<TMainRoot, TBranchType, TSet, TValue>(
        ChainBuilder<TMainRoot, TBranchType, TSet> builder) where TSet : class, IEnumerable<TValue> where TBranchType : IBranchType
    {
        var nextNode = new CollectionNode<TSet, TValue>(builder.RootNode);
        builder.Node.Chain(nextNode);

        return new(
            builder.RootNode,
            nextNode
        );
    }


    #region 'Type inference' aliases

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType, TValue>(
        this ChainBuilder<TMainRoot, TBranchType, IEnumerable<TValue>> builder)
        where TBranchType : IBranchType =>
        EnterExplicit<TMainRoot, TBranchType, IEnumerable<TValue>, TValue>(builder);

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType, TValue>(
        this ChainBuilder<TMainRoot, TBranchType, TValue[]> builder)
        where TBranchType : IBranchType =>
        EnterExplicit<TMainRoot, TBranchType, TValue[], TValue>(builder);

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

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType, TValue, TKey>(
        this ChainBuilder<TMainRoot, TBranchType, ReactiveMap<TValue, TKey>> builder)
        where TKey : notnull where TBranchType : IBranchType =>
        EnterExplicit<TMainRoot, TBranchType, ReactiveMap<TValue, TKey>, TValue>(builder);

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType, TValue, TKey>(
        this ChainBuilder<TMainRoot, TBranchType, IReactiveCollection<TValue, TKey>> builder)
        where TKey : notnull where TBranchType : IBranchType =>
        EnterExplicit<TMainRoot, TBranchType, IReactiveCollection<TValue, TKey>, TValue>(builder);

    #endregion

    #endregion

    #region Maps

    public static ChainBuilder<TMainRoot, TBranchType, TValue> EnterAtExplicit<TMainRoot, TBranchType, TSet, TValue, TKey>(
        ChainBuilder<TMainRoot, TBranchType, TSet> builder, TKey key)
        where TSet : class, IReactiveCollection<TValue, TKey> where TBranchType : IBranchType where TKey : notnull
    {
        var nextNode = new KeyNode<TSet, TValue, TKey>(builder.RootNode, key);
        builder.Node.Chain(nextNode);

        return new(
            builder.RootNode,
            nextNode
        );
    }

    #region 'Type inference' aliases

    public static ChainBuilder<TMainRoot, TBranchType, TValue> EnterAt<TMainRoot, TBranchType,
        TValue, TKey>(
        this ChainBuilder<TMainRoot, TBranchType, IReactiveCollection<TValue, TKey>> builder, TKey key)
        where TBranchType : IBranchType where TKey : notnull =>
        EnterAtExplicit<TMainRoot, TBranchType, IReactiveCollection<TValue, TKey>, TValue, TKey>(builder, key);

    public static ChainBuilder<TMainRoot, TBranchType, TValue> EnterAt<TMainRoot, TBranchType,
        TValue>(
        this ChainBuilder<TMainRoot, TBranchType, IReactiveList<TValue>> builder, int key)
        where TBranchType : IBranchType =>
        EnterAtExplicit<TMainRoot, TBranchType, IReactiveList<TValue>, TValue, int>(builder, key);

    public static ChainBuilder<TMainRoot, TBranchType, TValue> EnterAt<TMainRoot, TBranchType,
        TValue>(
        this ChainBuilder<TMainRoot, TBranchType, ReactiveList<TValue>> builder, int key)
        where TBranchType : IBranchType =>
        EnterAtExplicit<TMainRoot, TBranchType, ReactiveList<TValue>, TValue, int>(builder, key);


    public static ChainBuilder<TMainRoot, TBranchType, TValue> EnterAt<TMainRoot, TBranchType, TValue, TKey>(
        this ChainBuilder<TMainRoot, TBranchType, IReactiveMap<TValue, TKey>> builder, TKey key)
        where TKey : notnull where TBranchType : IBranchType =>
        EnterAtExplicit<TMainRoot, TBranchType, IReactiveMap<TValue, TKey>, TValue, TKey>(builder, key);

    public static ChainBuilder<TMainRoot, TBranchType, TValue> EnterAt<TMainRoot, TBranchType, TValue, TKey>(
        this ChainBuilder<TMainRoot, TBranchType, ReactiveMap<TValue, TKey>> builder, TKey key)
        where TKey : notnull where TBranchType : IBranchType =>
        EnterAtExplicit<TMainRoot, TBranchType, ReactiveMap<TValue, TKey>, TValue, TKey>(builder, key);

    #endregion

    #endregion

    #region Branching

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Branch<TMainRoot, TBranchType, TValue, TNext1, TNext2>(
        this ChainBuilder<TMainRoot, TBranchType, TValue> builder,
        Func<ChainBuilder<TMainRoot, InnerBranch, TValue>, ChainBuilder<TMainRoot, InnerBranch, TNext1>> selector1,
        Func<ChainBuilder<TMainRoot, InnerBranch, TValue>, ChainBuilder<TMainRoot, InnerBranch, TNext2>> selector2)
        where TBranchType : IBranchType
    {
        var innerBuilder = new ChainBuilder<TMainRoot, InnerBranch, TValue>(builder.RootNode, builder.Node);

        selector1(innerBuilder);
        selector2(innerBuilder);

        return new(
            builder.RootNode,
            builder.Node
        );
    }

    #endregion

    #region Reactions

    public static IReactionBuilder<TMainRoot> Immediate<TMainRoot, TValue>(this ChainBuilder<TMainRoot, RootBranch, TValue> builder) =>
        new ImmediateReaction<TMainRoot>(builder.RootNode);

    public static IReactionBuilder<TMainRoot> Throttled<TMainRoot, TValue>(
        this ChainBuilder<TMainRoot, RootBranch, TValue> builder, int delay, ThrottleMode mode = ThrottleMode.Extendable) =>
        new ThrottledReaction<TMainRoot>(builder.RootNode)
        {
            Timeout = delay, Immediate = mode.HasFlag(ThrottleMode.Immediate),
            ResetTimeoutOnTrigger = mode.HasFlag(ThrottleMode.Extendable)
        };

    #endregion
}