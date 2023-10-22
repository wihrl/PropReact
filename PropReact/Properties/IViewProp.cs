namespace PropReact.Properties;

public interface IViewProp<TKey, TValue> : ICompProp<TValue?>
{
    TKey Key { get; }
}