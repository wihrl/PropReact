using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PropReact.Props;

public abstract class PropBase<TValue> : IProp<TValue>
{
    protected Dictionary<IPropObserver<TValue>, int>? Observers;

    protected void NotifyObservers(TValue? oldValue, TValue? newValue)
    {
        if (Observers is null) return;

        foreach (var reactionsMapKey in Observers.Keys)
            reactionsMapKey.PropChanged(oldValue, newValue);
    }
    
    void IProp<TValue>.Watch(IPropObserver<TValue> observer) =>
        CollectionsMarshal.GetValueRefOrAddDefault(Observers ??= new(), observer, out _)++;

    void IProp<TValue>.StopWatching(IPropObserver<TValue> observer)
    {
        ref var subs = ref CollectionsMarshal.GetValueRefOrNullRef(Observers!, observer);
        if (Unsafe.IsNullRef(ref subs))
            return;
        
        if (--subs == 0)
            Observers!.Remove(observer);
    }
}