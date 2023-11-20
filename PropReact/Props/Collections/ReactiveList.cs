using System.Collections;

namespace PropReact.Props.Collections;

public interface IReactiveList<TValue> : IReactiveCollection<TValue, int>, IList<TValue>
{
}

public sealed class ReactiveList<TValue> : ReactiveCollection<TValue, int>, IReactiveList<TValue>
{
    private readonly List<TValue> _list;

    public ReactiveList() => _list = new();
    public ReactiveList(IEnumerable<TValue> existing) => _list = new(existing);
    public ReactiveList(int capacity) => _list = new(capacity);


    public override IEnumerator<TValue> GetEnumerator() => _list.GetEnumerator();

    public void Add(TValue item)
    {
        _list.Add(item);
        Added(_list.Count - 1, item);
    }

    public void Clear()
    {
        // until bulk updates are supported, items have to be deleted one at a time to ensure consistency
        while (_list.Count > 0) Remove(_list.Last());
    }

    public bool Contains(TValue item) => _list.Contains(item);
    public void CopyTo(TValue[] array, int arrayIndex) => throw new NotImplementedException();

    public bool Remove(TValue item)
    {
        var index = _list.IndexOf(item);
        if (index < 0) return false;

        RemoveAt(index);
        return true;
    }

    public override int Count => _list.Count;
    protected override TValue? InternalGetter(int key) => key < 0 || key >= _list.Count ? default : _list[key];

    public bool IsReadOnly => ((ICollection<TValue>)_list).IsReadOnly;
    public int IndexOf(TValue item) => _list.IndexOf(item);

    public void Insert(int index, TValue item)
    {
        _list.Insert(index, item);
        Added(index, item);
    }

    public void RemoveAt(int index)
    {
        var item = _list[index];
        _list.RemoveAt(index);
        Removed(index, item);
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
}