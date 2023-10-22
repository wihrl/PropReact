namespace PropReact.Reactivity;

public class HashsetQueue<T>
{
    private Queue<T> _queue = new();
    private HashSet<T> _hashSet = new();

    public void Enqueue(T value)
    {
        if(_hashSet.Add(value))
            _queue.Enqueue(value);
    }

    public void Dequeue(T value)
    {
        
    }
}