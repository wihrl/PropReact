using System.Runtime.CompilerServices;
using PropReact.Chain;
using PropReact.Chain.Nodes;
using PropReact.Props;
using PropReact.Props.Collections;
using PropReact.Props.Value;
using PropReact.Selectors;

namespace PropReact.Tests;

public static class BuilderExtensions
{
    public static ChainNodeBase<TNext> Then<TSource, TNext>(this ChainNodeBase<TSource> root,
        Func<TSource, IValueProp<TNext>> getter)
        => new ValueNode<TSource, TNext>(getter, () => { });

    public static ChainNodeBase<TNext> Then<TSource, TNext>(this ChainNodeBase<TSource> root,
        Func<TSource, TNext> getter)
        => new ValueNode<TSource, TNext>(null!, () => { }); // todo: constant node

    public static void ThenEnter<TSource, TNext>(this IChainNode<TSource> root, Func<TSource, IValueProp<TNext>> getter)
        => new ValueNode<TSource, TNext>(getter, () => { });
}

public partial class ManualChainBuild
{
    string LambdaToString<TA, TB>(Func<TA, TB> func, [CallerArgumentExpression(nameof(func))] string? s = null) => s;


    [Fact]
    public void Simple()
    {
        // basic
        // public API: this.Watch(x => x.B.C.D1);

        // new ReactiveChain<A>(root =>
        //     root.Chain(x => x.B)
        //         .Chain(x => x.C)
        //         .Chain(x => x.D1));

        Reaction r = () => { };
        A root = null!;


        var r1 = new RootNode<A>(root, r)
        {
            new ValueNode<A, B>(x => x._b, r)
            {
                new ValueNode<B, C>(x => x._c, r)
            }
        };

        r1.Then(a => a.B).Then(a => a.C);
//A._props._b()
        // split
        // todo: public API: Prop.Watch(this, x => x.B.C).Split(x => x.Chain(...), x => ...).ToProp(out _prop, transform...)/.;
        // new ReactiveChain<A>(root =>
        //     root.Chain(x => x.B)
        //         .Chain(x => x.C)
        //         .Branch(
        //             y => y.Chain(x => x.D1)
        //                 .Chain(x => x.D2), // not necessary, will not trigger updates: .Chain(x => x.Value),
        //             y => y.Chain(x => x.D2)));

        // collection
        // todo: public API: this.Watch(x => x.B.C.EL).Enter().Chain(x => x.E1.Value);


        new RootNode<A>(root, r)
        {
            new ValueNode<A, B>(x => x.B, r)
            {
                new ValueNode<B, C>(x => x.C, r)
                {
                    new ValueNode<C, D>(x => x._d1, r),
                    new ValueNode<C, D>(x => x._d2, r),
                }
            }
        };

        Prop.Watch(root, x => x.B).Then(x => x.C)
            .Branch(
                c => c.Then(x => x.D1),
                c => c.Then(x => x.D2));

        // Prop.Watch(root, r => r.B.C.Branch(x => x.D1, x => x.D2));

        // new ReactiveChain<A>(root => root
        //     .Chain(x => x.B)
        //     .Chain(x => x.C)
        //     .ChainMany(x => x.EL)
        //     .Enter()
        //     .Chain(x => x.E1)
        //     .ChainMany(x => x.Value));

        new RootNode<A>(root, r)
        {
            new ValueNode<A, B>(x => x._b, r)
            {
                new ValueNode<B, C>(x => x._c, r)
                {
                    new CollectionNode<C, IListProp<E>, E>(x => x.El, r)
                    {
                        Inner =
                        {
                            new ValueNode<E, E>(x => x._e1, r)
                        },
                    }
                }
            }
        };

        //Prop.Watch(root, r => r.B.C.El.Select(x => x.E1));

        Prop.Watch(root, x => x.B)
            .Then(x => x.C)
            .Branch(b =>
                    b.Then(x => x.El)
                        .Then(x => x.E1),
                b => b.Then(x => x.D1)
            );

        // nested list
        // new ReactiveChain<A>(root => root
        // .Chain(x => x.B)
        // .Chain(x => x.C)
        // .ChainMany(x => x.ELE)
        // .Enter()
        // .ChainMany(x => x)
        // .Enter()
        // .Chain(x => x.E1));

        Prop.Watch(root, x => x.B)
            .Then(x => x.C)
            .Then(x => x.Ele);

        // value prop if enumerable or list
        // new ReactiveChain<A>(root => root
        //     .Chain(x => x.B)
        //     .Chain(x => x.ListOfC)
        //     .ChainMany(x => x)
        //     .Enter()
        //     .Chain(x => x.D1));

        Prop.Watch(root, x => x.B)
            .Then(x => x.ListOfC)
            .Enter()
            .Then(x => x.D1);

        Prop.Watch(root, x => x.B)
            .Then(x => x.C)
            .Then(x => x.Emap, "asdf")
            .Then(x => x.E1);

        // new RootNode<A>(root, r)
        // {
        //     new ValueNode<A, B>(x => x._b, r)
        //     {
        //         new ValueNode<B, IEnumerable<C>>(x => x._listOfC, r)
        //         {
        //             new CollectionNode<IEnumerable<C>, IEnumerable<C>, C>(x => x, r)
        //             {
        //                 Inner =
        //                 {
        //                     new ValueNode<C, D>(x => x._d1, r)
        //                 },
        //             }
        //         }
        //     }
        // };

        // new ReactiveChain<A>(root => root
        //     .Chain(x => x.B).Chain(x => x.C).ChainMany(x => x.EE).Enter()
        //     .Chain(x => x.E1));

        // creating props: Prop.Make(out _prop);
    }
}

partial class A
{
    private readonly IMutable<B> _b;

    private readonly IMutable<B> _pB;
}

partial class B
{
    private readonly IMutable<C> _c;

    private readonly IMutable<IEnumerable<C>> _listOfC;
}

partial class C
{
    public D Dd1 { get; }

    private readonly IMutable<D> _d1;
    private readonly IMutable<D> _d2;
    private readonly IEnumerable<D> _dl;
    
    public IListProp<E> El {get;}
    public IMap<E, string> Emap {get;}
    public IListProp<IEnumerable<E>> Ele {get;}
    public IEnumerable<E> Ee {get;}
}

partial class D
{
    private int Value { get; set; }
    private readonly IMutable<D> _d2;
}

partial class E
{
    private readonly string _value;
    private readonly IMutable<E> _e1;
}

// file partial class GetterTransformer
// {
//     static GetterTransformer()
//     {
//         Map = new Dictionary<object, object>();
//     }
// }
//
// file partial class GetterTransformer
// {
//     public static readonly Dictionary<object, object> Map;
//
//     public static IChainableNode<TSource>? Transform<TSource, TValue>(Func<TSource, TValue> getter)
//     {
//         if (!Map.TryGetValue(getter, out var transformed))
//             return null;
//
//         return (IChainableNode<TSource>) transformed;
//     }
// }