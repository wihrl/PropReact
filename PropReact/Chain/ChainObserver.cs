namespace PropReact.Chain;

public class ChainObserver<TRoot> : IDisposable
{
    private RootNode<TRoot> _root;
    public ChainObserver(RootNode<TRoot> root)
    {
        _root = root;
    }

    class ChainObserverNode<TValue> : IPropObserver<TValue>
    {
        private IProp<TValue>? _currentlyObserving;
        
        public void PropChanged(TValue? oldValue, TValue? newValue)
        {
            throw new NotImplementedException();
        }

        public void MoveTo(IProp<TValue>? newProp)
        {
            _currentlyObserving?.Unsub(this);
            newProp?.Sub(this);
            _currentlyObserving = newProp;
        }
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}