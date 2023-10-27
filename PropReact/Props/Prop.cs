using System.Linq.Expressions;
using PropReact.Collections;
using PropReact.Properties;
using PropReact.Reactivity;

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
/// This also forces you to init all props in the constructor, which is necessary for certain types anyway. 
/// </summary>
public static class Prop
{
    // todo: cache chains, use expression as key? - would be problematic because of disposing (need for ChainInstance)
    public static IDisposable Watch<TOwner, TValue>(TOwner owner, Expression<Func<TOwner, TValue>> selector,
        Action<TValue> action, Func<TOwner, TValue>? getter = null) where TOwner : notnull
    {
        getter ??= selector.Compile();
        return PropChain.Parse(owner, selector, () => action(getter(owner)));
    }

    public static IComputed<TResult> Computed<TOwner, TValue, TResult>(TOwner owner,
        Expression<Func<TOwner, TValue>> selector,
        Func<TValue, TResult> compute) where TOwner : ICompositeDisposable
    {
        // todo: make lazy (only update if someone is observing the value) (cannot do because of transactions)
        IComputed<TResult> comp = new ReadOnlyProp<TResult>(default!);
        owner.AddDisposable(Watch(owner, selector, x => comp.Set(compute(x))));
        return comp;
    }

    public static IComputed<TValue> Computed<TOwner, TValue>(TOwner owner,
        Expression<Func<TOwner, TValue>> selector) where TOwner : ICompositeDisposable
    {
        IComputed<TValue> comp = new ReadOnlyProp<TValue>(default!);
        owner.AddDisposable(Watch(owner, selector, x => comp.Set(x)));
        return comp;
    }

    // todo: add variants with multiple values on the same line
    // public static void Make<TValue>(out IListProp<TValue> value) => value = new ListProp<TValue>(null);
    //
    // public static void Make<TOwner, TValue>(out IProp<TValue> prop, TValue initialValue,
    //     Func<TValue, INavProp<TOwner?>> navGetter) where TOwner : notnull
    //     => prop = new MutableProp<TValue>(initialValue, new NavSetter<TValue, TOwner>(owner, navGetter));
    //
    // public static void Make<TOwner, TValue>(out IListProp<TValue> list, TOwner owner) where TOwner : notnull
    //     => list = new ListProp<TValue>(new NavSetter<TValue, TOwner>(owner, navGetter));
    //
    // public static void Make<TKey, TValue>(out IMapProp<TKey, TValue> value, Func<TValue, TKey> keySelector)
    //     where TKey : notnull => value = new MapProp<TKey, TValue>(keySelector, null);

    #region Mutable props

    public static void Make<TValue>(out IMutable<TValue?> value) => value = new MutableProp<TValue?>(default);
    public static void Make<TValue>(out IMutable<TValue> value, TValue initialValue) =>
        value = new MutableProp<TValue>(initialValue);
    
    public static void MakeMany<TValue1, TValue2>(out IMutable<TValue1?> value1, out IMutable<TValue2?> value2)
    {
        Make(out value1);
        Make(out value2);
    } // todo: ...

    #endregion



    public static void MakeComputed<TOwner, TValue, TResult>(out IComputed<TResult> prop, TOwner owner,
        Expression<Func<TOwner, TValue>> selector,
        Func<TValue, TResult> compute) where TOwner : ICompositeDisposable
    {
        // todo: make lazy (only update if someone is observing the value)
        IComputed<TResult> comp = new ReadOnlyProp<TResult>(default!);
        owner.AddDisposable(Watch(owner, selector, x => comp.Set(compute(x))));
        prop = comp;
    }

    // todo:
    public static void Make<TFactory, TValue>(out TFactory value) where TFactory : IPropFactory<TValue?> =>
        value = (TFactory) TFactory.Make();
}

public interface IPropFactory<T>
{
    public static abstract IPropFactory<T> Make();
}