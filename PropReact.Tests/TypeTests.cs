using PropReact.Chain;
using PropReact.Props.Collections;
using PropReact.Props.Value;
// ReSharper disable All
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace PropReact.Tests;

/// <summary>
/// Checks that "type inference" overloads compile, does not check runtime logic.
/// This "test" can be useful when messing around with ChainBuilder generics.
/// </summary>
public class TypeTests
{
    class ValueTypeData
    {
        public readonly Mutable<string> Mutable;
        public readonly IMutable<string> IMutable;
        public readonly IComputed<string> IComputed;
        public readonly IValue<string> IValue;
    }

    void Values()
    {
        var builder = ChainBuilder.From(new ValueTypeData());
        builder.ChainValue(x => x.Mutable);
        builder.ChainValue(x => x.IMutable);
        builder.ChainValue(x => x.IComputed);
        builder.ChainValue(x => x.IValue);
    }

    class CollectionTypeData
    {
        public readonly ReactiveList<string> ReactiveList;
        public readonly IReactiveList<string> IReactiveList;

        public readonly ReactiveMap<string, int> ReactiveMap;
        public readonly IReactiveMap<string, int> IReactiveMap;

        public readonly IReactiveCollection<string, int> IKeyedList;

        public readonly string[] Array;
        public readonly IEnumerable<string> IEnumerable;
    }

    void Collections()
    {
        var builder = ChainBuilder.From(new CollectionTypeData());
        builder.ChainConstant(x => x.ReactiveList).Enter();
        builder.ChainConstant(x => x.ReactiveList).EnterAt(1);

        builder.ChainConstant(x => x.IReactiveList).Enter();
        builder.ChainConstant(x => x.IReactiveList).EnterAt(1);

        builder.ChainConstant(x => x.ReactiveList).Enter();
        builder.ChainConstant(x => x.ReactiveList).EnterAt(1);

        builder.ChainConstant(x => x.IReactiveList).Enter();
        builder.ChainConstant(x => x.IReactiveList).EnterAt(1);

        builder.ChainConstant(x => x.IKeyedList).Enter();
        builder.ChainConstant(x => x.IKeyedList).EnterAt(1);

        builder.ChainConstant(x => x.Array).Enter();
        builder.ChainConstant(x => x.IEnumerable).Enter();
    }

    public void Mixed()
    {
        A root = null!;

        ChainBuilder.From(root)
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

        ChainBuilder.From(root)
            .ChainValue(x => x.B)
            .ChainValue(x => x.C)
            .ChainConstant(x => x.Ele)
            .Enter()
            .Enter()
            .ChainValue(x => x.E1)
            .Immediate()
            .Compute(() => root.B.Value.C.Value.Ele.Select(x => x.Select(y => y.E1.v)), out _);

        ChainBuilder.From(root)
            .ChainValue(x => x.B)
            .ChainValue(x => x.ListOfC)
            .Enter()
            .ChainValue(x => x.D1)
            .Immediate()
            .React(() => { });

        ChainBuilder.From(root)
            .ChainValue(x => x.B)
            .ChainValue(x => x.IListOfC)
            .Enter()
            .Enter()
            .Enter()
            .ChainValue(x => x.D1)
            .Immediate();

        ChainBuilder.From(root)
            .ChainValue(x => x.B)
            .ChainValue(x => x.C)
            .ChainConstant(x => x.Emap)
            .EnterAt("asdf")
            .ChainValue(x => x.E1)
            .Immediate();

        ChainBuilder.From(root)
            .ChainValue(x => x.Blist)
            .Enter()
            .ChainValue(x => x.C)
            .Immediate()
            .Compute(() => "", out _)
            .StartAsDisposable();
    }

    class A
    {
        public readonly IMutable<B> B;
        public readonly IMutable<IEnumerable<B>> Blist;

        public readonly IMutable<B> PB;
    }

    class B
    {
        public readonly IMutable<C> C;

        public readonly IMutable<IEnumerable<C>> ListOfC;
        public readonly IMutable<IReactiveList<IEnumerable<IReactiveMap<C, string>>>> IListOfC;
    }

    class C
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

    class D
    {
        public int Value { get; set; }
        public readonly IMutable<D> D2;
    }

    class E
    {
        public readonly string Value;
        public readonly IMutable<E> E1;
    }
}