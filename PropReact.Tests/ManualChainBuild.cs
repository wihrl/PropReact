using System.Runtime.CompilerServices;

namespace PropReact.Tests;

file static class Extensions
{
    public static (T1, T2) Select<T, T1, T2>(this T source, Func<T, T1> selector1, Func<T, T2> selector2) =>
        (selector1(source), selector2(source));

    public static Func<A, B> f = x => x.B.Value;
}

public class ManualChainBuild
{
    string LambdaToString<A, B>(Func<A, B> func, [CallerArgumentExpression(nameof(func))] string? s = null) => s;
    
    [Fact]
    public void LambdaTest()
    {
        Func<A, B> f = x => x.B.Value;

        var s1 = LambdaToString(f);
        var s2 = LambdaToString(Extensions.f);
        
        // todo: use callerlinenumber instead
        
        Assert.True(s1 == s2);
    }
    
    
    [Fact]
    public void Simple()
    {
        //Watch(this, static build => build);
        var a = new A();

        IPropNode<A, A> builder = null!;

        // basic
        // public API: this.Watch(x => x.B.C.D1.Split());
        builder.ChainSingle(x => x.B)
            .ChainSingle(x => x.C)
            .ChainSingle(x => x.D1);

        var c1 = (a.B.Value.C.Value.D1.Value, a.B.Value.C.Value.EL.Select(x => x.Value));
        var c2 = a.B.Value.C.Value.Select(x => x.D1.Value, x => x.EL.Select(x => x.Value));

        // split
        // todo: public API: Prop.Watch(this, x => x.B.C).Split(x => x.Chain(...), x => ...).ToProp(out _prop, transform...)/.;
        builder.ChainSingle(x => x.B)
            .ChainSingle(x => x.C)
            .Branch(
                y => y.ChainSingle(x => x.D1)
                    .ChainSingle(x => x.D2), // not necessary, will not trigger updates: .Chain(x => x.Value),
                y => y.ChainSingle(x => x.D2));

        // collection
        // todo: public API: this.Watch(x => x.B.C.EL).Enter().Chain(x => x.E1.Value);
        builder.ChainSingle(x => x.B)
            .ChainSingle(x => x.C)
            .ChainSingle(x => x.EL)
            .Enter()
            .ChainSingle(x => x.E1)
            .ChainSingle(x => x.Value);

        // creating props: Prop.Make(out _prop);
    }

    IDisposable Watch<TSource, TRes>(TSource source, Func<TSource, TRes> getter)
    {
        return null!;
    }
}

// generator result Value<Depth>.<Branch>

// var n31 = new ChainNode(x=>x.Value3-1);
// var n21 = new ChainNode(x=>x.Value2-1);
// var n22 = new ChainNode(x=>x.Value2-2);
// var n1_0 = new ChainNode<T1, T2>(x=>x.Value1_0);

// Chain.Build(this).Then(x=>x.V1).Then(x=>x.V2).Select(x=>x.Split.V3)

record Car(string Brand, string Model, int Mileage, int Power);

interface IChangeSource
{
}

file interface IProp<TValue>
{
    public TValue Value { get; }
}

file interface IChainableNode<TSource>
{
    
    // todo: IMapProp<> could be used for both Lists and Maps - the only thing that differs is initialization, access api is the same
    // todo: ChainSingle - only single access allowed, must result into an IProp<> or IMapProp<>
    // todo: Chain - multiple properties can be used, internally replaced with calls to ChainSingle
    IPropNode<TSource, TNext> ChainSingle<TNext>(Func<TSource, IProp<TNext>> selector);
    ISetNode<TSource, IEnumerable<TValue>, TValue> ChainSingle<TValue>(Func<TSource, IEnumerable<TValue>> selector);
    ISetNode<TSource, List<TValue>, TValue> ChainSingle<TValue>(Func<TSource, List<TValue>> selector);

    // implementation: create a split node, set it as next and pass args
    void Branch(Action<IChainableNode<TSource>> selector1, Action<IChainableNode<TSource>> selector2);
}

file interface IChainSource<T>
{
    static abstract Func<T, IProp<TValue>> TransformGetter<TValue>(Func<T, TValue> getter);
}

file class NodeImpl<TSource> : IChainableNode<TSource>
{
    public IPropNode<TSource, TNext> ChainSingle<TNext>(Func<TSource, IProp<TNext>> selector)
    {
        throw new NotImplementedException();
    }

    public ISetNode<TSource, IEnumerable<TValue>, TValue> ChainSingle<TValue>(Func<TSource, IEnumerable<TValue>> selector)
    {
        throw new NotImplementedException();
    }

    public ISetNode<TSource, List<TValue>, TValue> ChainSingle<TValue>(Func<TSource, List<TValue>> selector)
    {
        throw new NotImplementedException();
    }

    public void Branch(Action<IChainableNode<TSource>> selector1, Action<IChainableNode<TSource>> selector2)
    {
        throw new NotImplementedException();
    }
}

file interface IPropNode<TFrom, TTo> : IChainableNode<TTo>
{
    Func<TFrom, TTo> _getter { get; }

    void Watch(IProp<TFrom> source);
}

file interface ISetNode<TFrom, TTo, TValue> : IPropNode<TFrom, TTo> where TTo : IEnumerable<TValue>
{
    IPropNode<TFrom, TValue> Enter();
}

file interface IMapNode<TFrom, TTo, TValue, TKey> : ISetNode<TFrom, TTo, TValue> where TTo : IEnumerable<TValue>
{
    IPropNode<TFrom, TValue> Enter(TKey key);
}

file class A
{
    public readonly IProp<B> B;

    private readonly IProp<B> _pB;
    public B PB => _pB.Value;
}

file class B
{
    public readonly IProp<C> C;
}

file class C
{
    public readonly IProp<D> D1;
    public readonly IProp<D> D2;
    public readonly IEnumerable<D> DL;
    public readonly IEnumerable<E> EL;
}

file class D
{
    public int Value { get; set; }
    public readonly IProp<D> D2;
}

file class E
{
    public readonly string Value;
    public readonly IProp<E> E1;
}

file partial class GetterTransformer
{
    static GetterTransformer()
    {
        Map = new Dictionary<object, object>();
    }
}

file partial class GetterTransformer
{
    public static readonly Dictionary<object, object> Map;

    public static IChainableNode<TSource>? Transform<TSource, TValue>(Func<TSource, TValue> getter)
    {
        if (!Map.TryGetValue(getter, out var transformed))
            return null;

        return (IChainableNode<TSource>) transformed;
    } 
}