namespace PropReact.Tests;

public class ManualChainBuild
{
    [Fact]
    public void Simple()
    {
        //Watch(this, static build => build);
        var a = new A();

        IPropNode<A, A> builder = null!;
        
        // basic
        builder.Chain(x => x.B)
            .Chain(x => x.C)
            .Chain(x => x.D1);

        // split
        builder.Chain(x => x.B)
            .Chain(x => x.C)
            .Split(
                y => y.Chain(x => x.D1).Chain(x => x.D2), // not necessary, will not trigger updates: .Chain(x => x.Value),
                y => y.Chain(x => x.D2));
        
        // collection
        builder.Chain(x => x.B)
            .Chain(x => x.C)
            .Chain(x => x.EL)
            .Enter()
            .Chain(x => x.E1)
            .Chain(x => x.Value);
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
}

file interface IChainableNode<TSource>
{
    IPropNode<TSource, TNext> Chain<TNext>(Func<TSource, IProp<TNext>> selector);
    ISetNode<TSource, IEnumerable<TValue>, TValue> Chain<TValue>(Func<TSource, IEnumerable<TValue>> selector);
    ISetNode<TSource, List<TValue>, TValue> Chain<TValue>(Func<TSource, List<TValue>> selector);

    // implementation: create a split node, set it as next and pass args
    void Split(Action<IChainableNode<TSource>> selector1, Action<IChainableNode<TSource>> selector2);
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