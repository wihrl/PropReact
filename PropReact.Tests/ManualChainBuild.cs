using System.Runtime.CompilerServices;
using PropReact.Chain;
using PropReact.Chain.Nodes;
using PropReact.Props;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Tests;

public partial class ManualChainBuild
{
    [Fact]
    public void Simple()
    {
        // basic
        // public API: this.Watch(x => x.B.C.D1);

        // new ReactiveChain<A>(root =>
        //     root.Chain(x => x.B)
        //         .Chain(x => x.C)
        //         .Chain(x => x.D1));

        A root = null!;


        // var r1 = new RootNode<A>(root, r)
        // {
        //     new ValueNode<A, B>(x => x.B, r)
        //     {
        //         new ValueNode<B, C>(x => x.C, r)
        //     }
        // };

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


        // new RootNode<A>(root, r)
        // {
        //     new ValueNode<A, B>(x => x.B, r)
        //     {
        //         new ValueNode<B, C>(x => x.C, r)
        //         {
        //             new ValueNode<C, D>(x => x.D1, r),
        //             new ValueNode<C, D>(x => x.D2, r),
        //         }
        //     }
        // };
        //
        // Prop.Watch(root, x => x.B).Then(x => x.C)
        //     .Branch(
        //         c => c.Then(x => x.D1),
        //         c => c.Then(x => x.D2));

        // Prop.Watch(root, r => r.B.C.Branch(x => x.D1, x => x.D2));

        // new ReactiveChain<A>(root => root
        //     .Chain(x => x.B)
        //     .Chain(x => x.C)
        //     .ChainMany(x => x.EL)
        //     .Enter()
        //     .Chain(x => x.E1)
        //     .ChainMany(x => x.Value));

        // new RootNode<A>(root, r)
        // {
        //     new ValueNode<A, B>(x => x.B, r)
        //     {
        //         new ValueNode<B, C>(x => x.C, r)
        //         {
        //             new CollectionNode<C, IListProp<E>, E>(x => x.El, r)
        //             {
        //                 new ValueNode<E, E>(x => x.E1, r)
        //             }
        //         }
        //     }
        // };

        //Prop.Watch(root, r => r.B.C.El.Select(x => x.E1));

        Prop.Watch(root)
            .ChainValue(x => x.B)
            .ChainValue(x => x.C)
            .Branch(
                b => b.ChainConstant(x => x.El).Enter().ChainValue(x => x.E1),
                b =>
                    b.Branch(
                        b2 => b2.ChainValue(x => x.D1),
                        b2 => b2.ChainValue(x => x.D2))
            )
            .Immediate()
            .React(() => { });

        // nested list
        // new ReactiveChain<A>(root => root
        // .Chain(x => x.B)
        // .Chain(x => x.C)
        // .ChainMany(x => x.ELE)
        // .Enter()
        // .ChainMany(x => x)
        // .Enter()
        // .Chain(x => x.E1));

        var p2 = Prop.Watch(root)
            .ChainValue(x => x.B)
            .ChainValue(x => x.C)
            .ChainConstant(x => x.Ele)
            .Enter()
            .Enter()
            .ChainValue(x => x.E1)
            .Immediate()
            .Compute(() => root.B.Value.C.Value.Ele.Select(x => x.Select(y => y.E1.v)), out var d0);

        // value prop if enumerable or list
        // new ReactiveChain<A>(root => root
        //     .Chain(x => x.B)
        //     .Chain(x => x.ListOfC)
        //     .ChainMany(x => x)
        //     .Enter()
        //     .Chain(x => x.D1));

        Prop.Watch(root)
            .ChainValue(x => x.B)
            .ChainValue(x => x.ListOfC)
            .Enter()
            .ChainValue(x => x.D1)
            .Immediate()
            .React(() => {});

        //IEnumerable<IMap<string, C>> a = root.B.Value.IListOfC.Value[0];

        var p4 = Prop.Watch(root)
            .ChainValue(x => x.B)
            .ChainValue(x => x.IListOfC)
            .Enter()
            .Enter()
            .Enter()
            .ChainValue(x => x.D1)
            .Immediate();


        var p5 = Prop.Watch(root)
            .ChainValue(x => x.B)
            .ChainValue(x => x.C)
            .ChainConstant(x => x.Emap)
            .EnterAt("asdf")
            .ChainValue(x => x.E1)
            .Immediate();

        var p6 = Prop.Watch(root)
            .ChainValue(x => x.Blist)
            .Enter()
            .ChainValue(x => x.C)
            .Immediate()
            .Compute(() => "", out var d1)
            .Start();

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
    public readonly IMutable<B> B;
    public readonly IMutable<IEnumerable<B>> Blist;

    public readonly IMutable<B> PB;
}

partial class B
{
    public readonly IMutable<C> C;

    public readonly IMutable<IEnumerable<C>> ListOfC;
    public readonly IMutable<IReactiveList<IEnumerable<IReactiveMap<C, string>>>> IListOfC;
}

partial class C
{
    public D Dd1 { get; }

    public readonly IMutable<D> D1;
    public readonly IMutable<D> D2;
    public readonly IEnumerable<D> Dl;

    public IReactiveList<E> El { get; }
    public IReactiveMap<E, string> Emap { get; }
    public IReactiveList<IEnumerable<E>> Ele { get; }
    public IEnumerable<E> Ee { get; }
}

partial class D
{
    public int Value { get; set; }
    public readonly IMutable<D> D2;
}

partial class E
{
    public readonly string Value;
    public readonly IMutable<E> E1;
}