namespace PropReact.Properties;

internal class ViewProp<TKey, TValue> : ReadOnlyProp<TValue>, IViewProp<TKey, TValue>
{
    public ViewProp(TKey key)
    {
        Key = key;
    }

    public TKey Key { get; }
}