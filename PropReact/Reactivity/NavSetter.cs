using PropReact.Properties;

namespace PropReact.Reactivity;

internal record NavSetter<TValue, TOwner>(TOwner Owner, Func<TValue, INavProp<TOwner?>> NavGetter) : INavSetter<TValue>
{
    void INavSetter<TValue>.Update(TValue? oldValue, TValue? newValue)
    {
        if (oldValue is not null) NavGetter(oldValue).Set(default!);
        if (newValue is not null) NavGetter(newValue).Set(Owner);
    }
}

internal interface INavSetter<TValue>
{
    internal void Update(TValue? oldValue, TValue? newValue);
}