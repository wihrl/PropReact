namespace PropReact.Selectors;

public static class SelectorExtensions
{
    public static (T1, T2) Branch<TSource, T1, T2>(this TSource source, Func<TSource, T1> s1, Func<TSource, T2> s2) =>
        (s1(source), s2(source));

    public static (T1, T2, T3) Branch<TSource, T1, T2, T3>(this TSource source, Func<TSource, T1> s1,
        Func<TSource, T2> s2, Func<TSource, T3> s3) =>
        (s1(source), s2(source), s3(source));

    public static (T1, T2, T3, T4) Branch<TSource, T1, T2, T3, T4>(this TSource source, Func<TSource, T1> s1,
        Func<TSource, T2> s2, Func<TSource, T3> s3, Func<TSource, T4> s4) =>
        (s1(source), s2(source), s3(source), s4(source));
}