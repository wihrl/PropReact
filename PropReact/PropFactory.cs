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
    public static IProp<TValue> MakeNavigable<TOwner, TValue>(TOwner owner, TValue initialValue,
        Func<TValue, INavProp<TOwner?>> navGetter) where TOwner : notnull
        => new MutableProp<TValue>(initialValue, new NavSetter<TValue, TOwner>(owner, navGetter));

    public static IProp<TValue>
        Make<TValue>(TValue initialValue) // todo: keep only for props and navs, everthing else should use out
        => new MutableProp<TValue>(initialValue);

    public static INavProp<TValue?> MakeNav<TValue>() => new ReadOnlyProp<TValue?>();

    public static IListProp<TValue> MakeList<TValue>() => new ListProp<TValue>(null);
    public static IReactiveArray<TValue> MakeArray<TValue>(int length) => new ReactiveArray<TValue>(length, null);

    public static IMapProp<TKey, TValue> MakeMap<TKey, TValue>(Func<TValue, TKey> keySelector)
        where TKey : notnull => new MapProp<TKey, TValue>(keySelector, null);

    public static IList<TValue> MakeNavigableList<TValue, TOwner>(TOwner owner,
        Func<TValue, INavProp<TOwner>> navGetter) =>
        new ListProp<TValue>(new NavSetter<TValue, TOwner>(owner, navGetter));

    // todo: cache chains, use expression as key? - would be problematic because of disposing (need for ChainInstance)
    public static IDisposable Watch<TOwner, TValue>(TOwner owner, Expression<Func<TOwner, TValue>> selector,
        Action<TValue> action, Func<TOwner, TValue>? getter = null) where TOwner : notnull
    {
        getter ??= selector.Compile();
        return PropChain.Parse(owner, selector, () => action(getter(owner)));
    }

    public static ICompProp<TResult> Computed<TOwner, TValue, TResult>(TOwner owner,
        Expression<Func<TOwner, TValue>> selector,
        Func<TValue, TResult> compute) where TOwner : ICompositeDisposable
    {
        // todo: make lazy (only update if someone is observing the value) (cannot do because of transactions)
        ICompProp<TResult> comp = new ReadOnlyProp<TResult>();
        owner.AddDisposable(Watch(owner, selector, x => comp.Set(compute(x))));
        return comp;
    }

    public static ICompProp<TValue> Computed<TOwner, TValue>(TOwner owner,
        Expression<Func<TOwner, TValue>> selector) where TOwner : ICompositeDisposable
    {
        ICompProp<TValue> comp = new ReadOnlyProp<TValue>();
        owner.AddDisposable(Watch(owner, selector, x => comp.Set(x)));
        return comp;
    }

    // todo: add variants with multiple values on the same line
    public static void Make<TValue>(out IListProp<TValue> value) => value = new ListProp<TValue>(null);

    public static void Make<TOwner, TValue>(out IProp<TValue> prop, TOwner owner, TValue initialValue,
        Func<TValue, INavProp<TOwner?>> navGetter) where TOwner : notnull
        => prop = new MutableProp<TValue>(initialValue, new NavSetter<TValue, TOwner>(owner, navGetter));

    public static void Make<TOwner, TValue>(out IListProp<TValue> list, TOwner owner,
        Func<TValue, INavProp<TOwner?>> navGetter) where TOwner : notnull
        => list = new ListProp<TValue>(new NavSetter<TValue, TOwner>(owner, navGetter));

    public static void Make<TKey, TValue>(out IMapProp<TKey, TValue> value, Func<TValue, TKey> keySelector)
        where TKey : notnull => value = new MapProp<TKey, TValue>(keySelector, null);

    public static void Make<TValue>(out INavProp<TValue> value) => value = new ReadOnlyProp<TValue>();


    public static void Make<TValue>(out IProp<TValue?> value) => value = new MutableProp<TValue?>(default);

    public static void Make<TValue1, TValue2>(out IProp<TValue1?> value1, out IProp<TValue2?> value2)
    {
        Make(out value1);
        Make(out value2);
    } // todo: ...

    public static void Make<TValue>(out IProp<TValue> value, TValue initialValue) =>
        value = new MutableProp<TValue>(initialValue);

    public static void MakeComputed<TOwner, TValue, TResult>(out ICompProp<TResult> prop, TOwner owner,
        Expression<Func<TOwner, TValue>> selector,
        Func<TValue, TResult> compute) where TOwner : ICompositeDisposable
    {
        // todo: make lazy (only update if someone is observing the value)
        ICompProp<TResult> comp = new ReadOnlyProp<TResult>();
        owner.AddDisposable(Watch(owner, selector, x => comp.Set(compute(x))));
        prop = comp;
    }
}