using System.Collections;
using PropReact.Reactivity;

namespace PropReact.Collections;

public interface IReactiveArray<TValue> : IWatchableCollection<int, TValue>, IList<TValue>
{
}

internal class ReactiveArray<TValue> : ReactiveCollectionBase<int, TValue>, IReactiveArray<TValue>
{
    public TValue[] _data;
    public int Length { get; }

    internal ReactiveArray(int length, INavSetter<TValue>? navSetter) : base(navSetter)
    {
        Length = length;
        _data = new TValue[length];
    }

    public TValue this[int index]
    {
        get => _data[index];
        set
        {
            var oldValue = _data[index];
            _data[index] = value;
            Replaced(index, oldValue, value);
        }
    }

    public IEnumerator<TValue> GetEnumerator() => (IEnumerator<TValue>) _data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(TValue item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(TValue item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(TValue item)
    {
        throw new NotImplementedException();
    }

    public int IndexOf(TValue item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, TValue item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }
    
    public int Count => _data.Length;
    public bool IsReadOnly => true;
    protected override TValue InternalGetter(int key) => _data[key];
}