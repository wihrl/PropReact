using PropReact.Props;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Chain;

// the idea would be to have something 
// todo: public API: Prop.Watch().Chain(...).Branch(...).Transform().ToProp(out _computed);
public class ChainBuilder<TRoot>
{
    internal ChainBuilder(){}
    
    //todo: extension methods on null?
    public ChainBuilder<TValue> Then<TValue>(Func<TRoot, IProp<TValue>> selector)
    {
        return new ChainBuilder<TValue>();
    }

    public ChainBuilder<TValue> Then<TValue, TKey>(Func<TRoot, IMap<TValue, TKey>> selector, TKey at)
        where TKey : notnull
    {
        return new ChainBuilder<TValue>();
    }


    public ChainBuilder<TValue> Constant<TSource, TValue>(Func<TSource, TValue> selector)
    {
        return new ChainBuilder<TValue>();
    }

    public ChainBuilder<TRoot> Branch(Action<ChainBuilder<TRoot>> b1, Action<ChainBuilder<TRoot>> b2)
    {
        b1(this);
        b2(this);
        return this;
    }
}

public class ListChainBuilder<TRoot> : ChainBuilder<TRoot>
{
}

public static class ChainBuilderExtensions
{
    // todo: validate using analyzer

    // has to be an extension method because it is only available for ChainBuilders with TRoot : IPropSource
    public static ChainBuilder<TValue> Then<TRoot, TValue>(this ChainBuilder<TRoot> builder,
        Func<TRoot, TValue> selector)
        where TRoot : IPropSource<TRoot>
    {
        return new ChainBuilder<TValue>();
    }

    public static ChainBuilder<TValue> Enter<TValue>(this ChainBuilder<IEnumerable<TValue>> builder)
    {
        return new ChainBuilder<TValue>();
    }

    public static ChainBuilder<TValue> Enter<TValue>(this ChainBuilder<IListProp<TValue>> builder)
    {
        return new ChainBuilder<TValue>();
    }

    public static ChainBuilder<TValue> EnterAt<TValue, TKey>(
        this ChainBuilder<IKeyedCollectionProp<TValue, TKey>> builder, TKey key)
    {
        return new ChainBuilder<TValue>();
    }
}