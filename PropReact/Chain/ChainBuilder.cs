using PropReact.Chain.Nodes;
using PropReact.Props.Collections;
using PropReact.Props.Value;

public interface IBranchType
{
}

public struct RootBranch : IBranchType
{
}

public struct InnerBranch : IBranchType
{
}

public struct ChainBuilder<TRoot, TBranchType, TValue> where TBranchType : IBranchType
{
    public required RootNode<TRoot> RootNode { get; init; }
    public required ChainNode<TValue> Node { get; init; }
}

public interface IReactionBuilder : IReaction
{
    void React(Action action);
    IComputed<TValue> ToComputed<TValue>(Func<TValue> getter);

}

public interface IReaction
{
    internal void Trigger();
}

abstract class Reaction : IReactionBuilder
{
    public void Trigger()
    {
        throw new NotImplementedException();
    }

    public void React(Action action)
    {
        throw new NotImplementedException();
    }

    public IComputed<TValue> ToComputed<TValue>(Func<TValue> getter)
    {
        throw new NotImplementedException();
    }
}

public class ImmediateReaction : Reaction
{
    public void React(Action action)
    {
        throw new NotImplementedException();
    }

    public IComputed<TValue> ToComputed<TValue>(Func<TValue> getter)
    {
        throw new NotImplementedException();
    }
}

public class ThrottledReaction : Reaction
{
    public void React(Action action)
    {
        throw new NotImplementedException();
    }

    public IComputed<TValue> ToComputed<TValue>(Func<TValue> getter)
    {
        throw new NotImplementedException();
    }
}

public static class ChainBuilderActions
{
    #region Values

    public static ChainBuilder<TMainRoot, TBranchType, TNext> Then<TMainRoot, TBranchType, TValue, TNext>(
        this ChainBuilder<TMainRoot, TBranchType, TValue> builder, Func<TValue, IValueProp<TNext>> selector)
        where TBranchType : IBranchType
    {
        return new()
        {
            RootNode = builder.RootNode,
            Node = new ValueNode<TValue, TNext>(selector, builder.RootNode),
        };
    }

    public static ChainBuilder<TMainRoot, TBranchType, TNext> ThenConstant<TMainRoot, TBranchType, TValue, TNext>(
        this ChainBuilder<TMainRoot, TBranchType, TValue> builder, Func<TValue, TNext> selector)
        where TBranchType : IBranchType
    {
        return new()
        {
            RootNode = builder.RootNode,
            Node = new ConstantNode<TValue, TNext>(selector, builder.RootNode),
        };
    }

    #endregion

    #region Collections

    private static ChainBuilder<TMainRoot, TBranchType, TValue>
        EnterExplicit<TMainRoot, TBranchType, TSet, TValue>(
            this ChainBuilder<TMainRoot, TBranchType, TSet> builder)
        where TSet : class, IEnumerable<TValue> where TBranchType : IBranchType
    {
        return new()
        {
            RootNode = builder.RootNode,
            Node = new CollectionNode<TSet, TValue>(builder.RootNode),
        };
    }

    // type inference proxies
    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType,
        TValue>(
        this ChainBuilder<TMainRoot, TBranchType, IEnumerable<TValue>> builder)
        where TBranchType : IBranchType =>
        builder.EnterExplicit<TMainRoot, TBranchType, IEnumerable<TValue>, TValue>();

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType, TValue>(
        this ChainBuilder<TMainRoot, TBranchType, IListProp<TValue>> builder) where TBranchType : IBranchType =>
        builder.EnterExplicit<TMainRoot, TBranchType, IListProp<TValue>, TValue>();

    public static ChainBuilder<TMainRoot, TBranchType, TValue> Enter<TMainRoot, TBranchType, TValue, TKey>(
        this ChainBuilder<TMainRoot, TBranchType, IMap<TValue, TKey>> builder)
        where TKey : notnull where TBranchType : IBranchType =>
        builder.EnterExplicit<TMainRoot, TBranchType, IMap<TValue, TKey>, TValue>();

    #endregion

    #region Maps

    public static ChainBuilder<TMainRoot, TBranchType, TValue> EnterAt<TMainRoot, TBranchType, TValue, TKey>(
        this ChainBuilder<TMainRoot, TBranchType, IMap<TValue, TKey>> builder, TKey key)
        where TKey : notnull where TBranchType : IBranchType =>
        throw new NotImplementedException();
    //builder.Enter<TMainRoot, TBranchType, IMap<TValue, TKey>, TValue>();

    #endregion

    #region Branching

    public static ChainBuilder<TMainRoot, TBranchType, (TNext1, TNext2)>
        Branch<TMainRoot, TBranchType, TValue, TNext1, TNext2>(
            this ChainBuilder<TMainRoot, TBranchType, TValue> builder,
            Func<ChainBuilder<TMainRoot, InnerBranch, TValue>, ChainBuilder<TMainRoot, InnerBranch, TNext1>> selector1,
            Func<ChainBuilder<TMainRoot, InnerBranch, TValue>, ChainBuilder<TMainRoot, InnerBranch, TNext2>> selector2)
        where TBranchType : IBranchType =>
        throw new NotImplementedException();

    #endregion

    #region Finalization

    public static IReactionBuilder Immediate<TMainRoot, TValue>(
        this ChainBuilder<TMainRoot, RootBranch, TValue> builder)
        => new ImmediateReaction();

    public static void Throttled<TMainRoot, TValue>(this ChainBuilder<TMainRoot, RootBranch, TValue> builder)
        => new ThrottledReaction();

    #endregion
}