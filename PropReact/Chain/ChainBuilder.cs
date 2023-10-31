namespace PropReact.Chain;

// the idea would be to have something 

public class ChainBuilder
{
    // todo: public API: Prop.Watch().Chain(...).Branch(...).Transform().ToProp(out _computed);
    
    // todo:
    public void Chain<TSource, T1, T2, T3>(TSource source, Func<TSource, T1> s1, Func<T1, T2> s2, Func<T2, T3> s3)
    {
        
    }

    public void Chain<TSource, T1>() where T1 : IPropSource
    {
        
    }
}