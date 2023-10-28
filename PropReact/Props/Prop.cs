using System.Linq.Expressions;
using PropReact.Properties;

namespace PropReact;

/// <summary>
/// Class for handling the creation and initialization of reactive properties.
/// Using this pattern allows for general type inference - instead of having to type:
/// <![CDATA[ IMapProp<int, Foo> Foo { get; } = Prop.Make<int, Foo>(); ]]>
/// which can get very verbose for long type names, you can instead just do:
/// <![CDATA[
/// readonly IMapProp<int, Foo> Foo;
/// Prop.Make(out Foo);
/// ]]>
/// This also forces you to init all props in the constructor, which is necessary for certain prop types anyway. 
/// </summary>
public static class Prop
{
    #region Mutable props

    #region Simple

    public static void Make<T1>(out IMutable<T1?> prop) => prop = new MutableValueProp<T1?>(default);

    public static void Make<T1, T2>(out IMutable<T1?> prop1, out IMutable<T2?> prop2)
    {
        Make(out prop1);
        Make(out prop2);
    }

    public static void Make<T1, T2, T3>(out IMutable<T1?> prop1, out IMutable<T2?> prop2, out IMutable<T3?> prop3)
    {
        Make(out prop1);
        Make(out prop2);
        Make(out prop3);
    }

    public static void Make<T1, T2, T3, T4>(out IMutable<T1?> prop1, out IMutable<T2?> prop2,
        out IMutable<T3?> prop3, out IMutable<T4?> prop4)
    {
        Make(out prop1);
        Make(out prop2);
        Make(out prop3);
        Make(out prop4);
    }

    #endregion

    #region With defaults

    public static void Make<T1>(out IMutable<T1> prop1, T1 val1) => prop1 = new MutableValueProp<T1>(val1);

    public static void Make<T1, T2>(out IMutable<T1> prop1, T1 val1, out IMutable<T2> prop2, T2 val2)
    {
        Make(out prop1, val1);
        Make(out prop2, val2);
    }

    public static void Make<T1, T2, T3>(
        out IMutable<T1> prop1, T1 val1,
        out IMutable<T2> prop2, T2 val2,
        out IMutable<T3> prop3, T3 val3)
    {
        Make(out prop1, val1);
        Make(out prop2, val2);
        Make(out prop3, val3);
    }

    public static void Make<T1, T2, T3, T4>(
        out IMutable<T1> prop1, T1 val1,
        out IMutable<T2> prop2, T2 val2,
        out IMutable<T3> prop3, T3 val3,
        out IMutable<T4> prop4, T4 val4)
    {
        Make(out prop1, val1);
        Make(out prop2, val2);
        Make(out prop3, val3);
        Make(out prop4, val4);
    }

    #endregion

    #region Combinations

    // default-less props are only allowed at the beginning


    public static void Make<T1, T2>(out IMutable<T1?> prop1, out IMutable<T2> prop2, T2 val2)
    {
        Make(out prop1);
        Make(out prop2, val2);
    }

    public static void Make<T1, T2, T3>(
        out IMutable<T1?> prop1,
        out IMutable<T2> prop2, T2 val2,
        out IMutable<T3> prop3, T3 val3)
    {
        Make(out prop1);
        Make(out prop2, val2);
        Make(out prop3, val3);
    }

    public static void Make<T1, T2, T3, T4>(
        out IMutable<T1?> prop1,
        out IMutable<T2> prop2, T2 val2,
        out IMutable<T3> prop3, T3 val3,
        out IMutable<T4> prop4, T4 val4)
    {
        Make(out prop1);
        Make(out prop2, val2);
        Make(out prop3, val3);
        Make(out prop4, val4);
    }

    public static void Make<T1, T2, T3>(
        out IMutable<T1?> prop1,
        out IMutable<T2?> prop2,
        out IMutable<T3> prop3, T3 val3)
    {
        Make(out prop1);
        Make(out prop2);
        Make(out prop3, val3);
    }

    public static void Make<T1, T2, T3, T4>(
        out IMutable<T1?> prop1,
        out IMutable<T2?> prop2,
        out IMutable<T3> prop3, T3 val3,
        out IMutable<T4> prop4, T4 val4)
    {
        Make(out prop1);
        Make(out prop2);
        Make(out prop3, val3);
        Make(out prop4, val4);
    }

    public static void Make<T1, T2, T3, T4>(
        out IMutable<T1?> prop1,
        out IMutable<T2?> prop2,
        out IMutable<T3?> prop3,
        out IMutable<T4> prop4, T4 val4)
    {
        Make(out prop1);
        Make(out prop2);
        Make(out prop3);
        Make(out prop4, val4);
    }

    #endregion

    #endregion


    // public static void MakeComputed<TOwner, T, TResult>(out IComputed<TResult> prop, TOwner owner,
    //     Expression<Func<TOwner, T>> selector,
    //     Func<T, TResult> compute) where TOwner : ICompositeDisposable
    // {
    //     // todo: make lazy (only update if someone is observing the prop)
    //     IComputed<TResult> comp = new ComputedpropProp<TResult>(default!);
    //     owner.AddDisposable(Watch(owner, selector, x => comp.Set(compute(x))));
    //     prop = comp;
    // }
}

public static class Set
{
    public static void Make()
    {
    }
}