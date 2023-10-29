using System.Collections;

namespace PropReact.Props.Collections;

public interface IListProp<TValue> : ICollectionProp<TValue, int>, IList<TValue>
{
    new int Count { get; }
}

internal sealed class ListProp<TValue> : CollectionPropBase<TValue, int>, IListProp<TValue>
{
    private readonly List<TValue> _list;

    internal ListProp() => _list = new();
    internal ListProp(IEnumerable<TValue> existing) => _list = new(existing);
    internal ListProp(int capacity) => _list = new(capacity);
    

    public override IEnumerator<TValue> GetEnumerator() => _list.GetEnumerator();

    public void Add(TValue item)
    {
        _list.Add(item);
        Added(_list.Count - 1, item);
    }

    public void Clear()
    {
        while (_list.Count > 0) Remove(_list.Last());
    }

    public bool Contains(TValue item) => _list.Contains(item);
    public void CopyTo(TValue[] array, int arrayIndex) => throw new NotImplementedException();

    public bool Remove(TValue item)
    {
        var index = _list.IndexOf(item);

        if (index >= 0)
        {
            _list.RemoveAt(index);
            Removed(index, item);
            return true;
        }

        return false;
    }

    public override int Count => _list.Count;

    public bool IsReadOnly => ((ICollection<TValue>) _list).IsReadOnly;
    public int IndexOf(TValue item) => _list.IndexOf(item);

    public void Insert(int index, TValue item)
    {
        _list.Insert(index, item);
        Added(index, item);
    }

    public void RemoveAt(int index)
    {
        var toRemove = _list[index];
        _list.RemoveAt(index);
        Removed(index, toRemove);
    }

    public TValue this[int index]
    {
        get => _list[index];
        set
        {
            var toReplace = _list[index];
            _list[index] = value;
            Replaced(index, toReplace, value);
        }
    }

    protected override TValue InternalGetter(int key) => _list[key];
}