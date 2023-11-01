using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using PropReact.Chain;
using PropReact.Chain.Nodes;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Props;

/// <summary>
/// Class for handling the creation and initialization of reactive properties.
/// Using this pattern allows for general type inference - instead of having to type:
/// <![CDATA[ IMap<int, Foo> Foo { get; } = Prop.Mutable<int, Foo>(); ]]>
/// which can get very verbose for long type names, you can instead just do:
/// <![CDATA[
/// readonly IMap<int, Foo> Foo;
/// Prop.Mutable(out Foo);
/// ]]>
/// This also forces you to init all props in the constructor, which is necessary for certain prop types anyway. 
/// </summary>
public static class Prop
{
    #region Mutable props

    #region Simple

    public static void Mutable<T1>(out IMutable<T1?> prop) => prop = new MutableValueProp<T1?>(default);

    public static void Mutable<T1, T2>(out IMutable<T1?> prop1, out IMutable<T2?> prop2)
    {
        Mutable(out prop1);
        Mutable(out prop2);
    }

    public static void Mutable<T1, T2, T3>(out IMutable<T1?> prop1, out IMutable<T2?> prop2, out IMutable<T3?> prop3)
    {
        Mutable(out prop1);
        Mutable(out prop2);
        Mutable(out prop3);
    }

    public static void Mutable<T1, T2, T3, T4>(out IMutable<T1?> prop1, out IMutable<T2?> prop2,
        out IMutable<T3?> prop3, out IMutable<T4?> prop4)
    {
        Mutable(out prop1);
        Mutable(out prop2);
        Mutable(out prop3);
        Mutable(out prop4);
    }

    #endregion

    #region With defaults

    public static void Mutable<T1>(out IMutable<T1> prop1, T1 val1) => prop1 = new MutableValueProp<T1>(val1);

    public static void Mutable<T1, T2>(out IMutable<T1> prop1, T1 val1, out IMutable<T2> prop2, T2 val2)
    {
        Mutable(out prop1, val1);
        Mutable(out prop2, val2);
    }

    public static void Mutable<T1, T2, T3>(
        out IMutable<T1> prop1, T1 val1,
        out IMutable<T2> prop2, T2 val2,
        out IMutable<T3> prop3, T3 val3)
    {
        Mutable(out prop1, val1);
        Mutable(out prop2, val2);
        Mutable(out prop3, val3);
    }

    public static void Mutable<T1, T2, T3, T4>(
        out IMutable<T1> prop1, T1 val1,
        out IMutable<T2> prop2, T2 val2,
        out IMutable<T3> prop3, T3 val3,
        out IMutable<T4> prop4, T4 val4)
    {
        Mutable(out prop1, val1);
        Mutable(out prop2, val2);
        Mutable(out prop3, val3);
        Mutable(out prop4, val4);
    }

    #endregion

    #region Combinations

    // default-less props are only allowed at the beginning


    public static void Mutable<T1, T2>(out IMutable<T1?> prop1, out IMutable<T2> prop2, T2 val2)
    {
        Mutable(out prop1);
        Mutable(out prop2, val2);
    }

    public static void Mutable<T1, T2, T3>(
        out IMutable<T1?> prop1,
        out IMutable<T2> prop2, T2 val2,
        out IMutable<T3> prop3, T3 val3)
    {
        Mutable(out prop1);
        Mutable(out prop2, val2);
        Mutable(out prop3, val3);
    }

    public static void Mutable<T1, T2, T3, T4>(
        out IMutable<T1?> prop1,
        out IMutable<T2> prop2, T2 val2,
        out IMutable<T3> prop3, T3 val3,
        out IMutable<T4> prop4, T4 val4)
    {
        Mutable(out prop1);
        Mutable(out prop2, val2);
        Mutable(out prop3, val3);
        Mutable(out prop4, val4);
    }

    public static void Mutable<T1, T2, T3>(
        out IMutable<T1?> prop1,
        out IMutable<T2?> prop2,
        out IMutable<T3> prop3, T3 val3)
    {
        Mutable(out prop1);
        Mutable(out prop2);
        Mutable(out prop3, val3);
    }

    public static void Mutable<T1, T2, T3, T4>(
        out IMutable<T1?> prop1,
        out IMutable<T2?> prop2,
        out IMutable<T3> prop3, T3 val3,
        out IMutable<T4> prop4, T4 val4)
    {
        Mutable(out prop1);
        Mutable(out prop2);
        Mutable(out prop3, val3);
        Mutable(out prop4, val4);
    }

    public static void Mutable<T1, T2, T3, T4>(
        out IMutable<T1?> prop1,
        out IMutable<T2?> prop2,
        out IMutable<T3?> prop3,
        out IMutable<T4> prop4, T4 val4)
    {
        Mutable(out prop1);
        Mutable(out prop2);
        Mutable(out prop3);
        Mutable(out prop4, val4);
    }

    #endregion

    #endregion

    #region Lists

    public static void Map<T1>(out IList<T1> list) => list = new ListProp<T1>();
    public static void Map<T1>(out IList<T1> list, int capacity) => list = new ListProp<T1>(capacity);

    public static void Map<T1, T2>(out IList<T1> prop1, out IList<T2> prop2)
    {
        Map(out prop1);
        Map(out prop2);
    }

    public static void Map<T1, T2, T3>(out IList<T1> prop1, out IList<T2> prop2, out IList<T3> prop3)
    {
        Map(out prop1);
        Map(out prop2);
        Map(out prop3);
    }

    public static void Map<T1, T2, T3, T4>(out IList<T1> prop1, out IList<T2> prop2,
        out IList<T3> prop3, out IList<T4> prop4)
    {
        Map(out prop1);
        Map(out prop2);
        Map(out prop3);
        Map(out prop4);
    }

    #endregion

    #region Maps

    public static void Map<T1Value, T1Key>(out IMap<T1Value, T1Key> map, Func<T1Value, T1Key> selector1)
        where T1Key : notnull => map = new CollectionProp<T1Value, T1Key>(selector1);

    public static void Map<T1Value, T1Key, T2Value, T2Key>(
        out IMap<T1Value, T1Key> prop1, Func<T1Value, T1Key> selector1,
        out IMap<T2Value, T2Key> prop2, Func<T2Value, T2Key> selector2)
        where T1Key : notnull
        where T2Key : notnull
    {
        Map(out prop1, selector1);
        Map(out prop2, selector2);
    }

    public static void Map<T1Value, T1Key, T2Value, T2Key, T3Value, T3Key>(
        out IMap<T1Value, T1Key> prop1, Func<T1Value, T1Key> selector1,
        out IMap<T2Value, T2Key> prop2, Func<T2Value, T2Key> selector2,
        out IMap<T3Value, T3Key> prop3, Func<T3Value, T3Key> selector3)
        where T1Key : notnull
        where T2Key : notnull
        where T3Key : notnull
    {
        Map(out prop1, selector1);
        Map(out prop2, selector2);
        Map(out prop3, selector3);
    }

    #endregion

    #region Watch

    // internal static void ParseChain<TRoot, TResult>(TRoot root, Action action, Func<TRoot, TResult> selector,
    //     [CallerArgumentExpression(nameof(selector))] string? expression = null) where TRoot : IChainRoot<TRoot>
    // {
    //     if (expression is null)
    //         throw new ArgumentException("Expression string cannot be null.");
    //
    //     root.CreateChain()
    // }

    public static ChainBuilder<TRoot, RootBranch, TRoot> Watch<TRoot>([NotNull] TRoot root) where TRoot : notnull => new();

    // public static ChainBuilder<TValue> Watch<TRoot, TValue>([NotNull] TRoot root,
    //     Func<TRoot, IEnumerable<TValue>> selector)
    //     where TRoot : notnull =>
    //     new ChainBuilder<TRoot>(new RootNode<TRoot>(root))
    //         .Then(selector);
    //
    // public static ChainBuilder<TValue> Watch<TRoot, TValue, TKey>([NotNull] TRoot root,
    //     Func<TRoot, IKeyedCollectionProp<TValue, TKey>> selector)
    //     where TRoot : notnull =>
    //     new ChainBuilder<TRoot>(new RootNode<TRoot>(root))
    //         .Then(selector);

    #endregion
}