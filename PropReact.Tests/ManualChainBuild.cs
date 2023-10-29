// using System.Runtime.CompilerServices;
// using PropReact.Chain;
// using PropReact.Properties;
// using PropReact.Props;
//
// namespace PropReact.Tests;
//
// file static class Extensions
// {
//     public static (T1, T2) Select<T, T1, T2>(this T source, Func<T, T1> selector1, Func<T, T2> selector2) =>
//         (selector1(source), selector2(source));
//
//     public static Func<A, B> f = x => x.B.Value;
// }
//
// public class ManualChainBuild
// {
//     string LambdaToString<A, B>(Func<A, B> func, [CallerArgumentExpression(nameof(func))] string? s = null) => s;
//
//     [Fact]
//     public void LambdaTest()
//     {
//         Func<A, B> f = x => x.B.Value;
//
//         var s1 = LambdaToString(f);
//         var s2 = LambdaToString(Extensions.f);
//
//         // todo: use callerlinenumber instead
//
//         Assert.True(s1 == s2);
//     }
//
//
//     [Fact]
//     public void Simple()
//     {
//         // basic
//         // public API: this.Watch(x => x.B.C.D1);
//
//         new ReactiveChain<A>(root =>
//             root.ChainSingle(x => x.B)
//                 .ChainSingle(x => x.C)
//                 .ChainSingle(x => x.D1));
//
//         // split
//         // todo: public API: Prop.Watch(this, x => x.B.C).Split(x => x.Chain(...), x => ...).ToProp(out _prop, transform...)/.;
//         new ReactiveChain<A>(root =>
//             root.ChainSingle(x => x.B)
//                 .ChainSingle(x => x.C)
//                 .Branch(
//                     y => y.ChainSingle(x => x.D1)
//                         .ChainSingle(x => x.D2), // not necessary, will not trigger updates: .Chain(x => x.Value),
//                     y => y.ChainSingle(x => x.D2)));
//
//         // collection
//         // todo: public API: this.Watch(x => x.B.C.EL).Enter().Chain(x => x.E1.Value);
//
//         new ReactiveChain<A>(root => root
//             .ChainSingle(x => x.B)
//             .ChainSingle(x => x.C)
//             .ChainSingle(x => x.ES)
//             .Branch(y =>
//                     y.ChainSingle(x => x.Count),
//                 y => y.Enter()
//                     .ChainSingle(x => x.E1)
//                     .ChainSingle(x => x.Value)));
//
//         new ReactiveChain<A>(root => root
//             .ChainSingle(x => x.B).ChainSingle(x => x.C).ChainSingle(x => x.EL).Enter()
//             .ChainSingle(x => x.E1));
//
//         // creating props: Prop.Make(out _prop);
//     }
//
//     IDisposable Watch<TSource, TRes>(TSource source, Func<TSource, TRes> getter)
//     {
//         return null!;
//     }
// }
//
// file class A
// {
//     public readonly IMutable<B> B;
//
//     private readonly IMutable<B> _pB;
//     public B PB => _pB.Value;
// }
//
// file class B
// {
//     public readonly IMutable<C> C;
// }
//
// file class C
// {
//     public readonly IMutable<D> D1;
//     public readonly IMutable<D> D2;
//     public readonly IEnumerable<D> DL;
//     public readonly ISetProp<E> ES;
//     public readonly IEnumerable<E> EL;
// }
//
// file class D
// {
//     public int Value { get; set; }
//     public readonly IMutable<D> D2;
// }
//
// file class E
// {
//     public readonly string Value;
//     public readonly IMutable<E> E1;
// }
//
// // file partial class GetterTransformer
// // {
// //     static GetterTransformer()
// //     {
// //         Map = new Dictionary<object, object>();
// //     }
// // }
// //
// // file partial class GetterTransformer
// // {
// //     public static readonly Dictionary<object, object> Map;
// //
// //     public static IChainableNode<TSource>? Transform<TSource, TValue>(Func<TSource, TValue> getter)
// //     {
// //         if (!Map.TryGetValue(getter, out var transformed))
// //             return null;
// //
// //         return (IChainableNode<TSource>) transformed;
// //     }
// // }