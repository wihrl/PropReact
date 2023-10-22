namespace PropReact
{
    internal class ReactiveEvent
    {
        List<Action> _subs = new();

        internal IDisposable Subscribe(Action action)
        {
            _subs.Add(action);
            return new UnsubHandler(action, this);
        }

        internal void Invoke()
        {
            foreach (var item in _subs)
            {
                item();
            }
        }

        class UnsubHandler : IDisposable
        {
            private readonly Action _action;
            private readonly ReactiveEvent _event;

            public UnsubHandler(Action action, ReactiveEvent @event)
            {
                _action = action;
                _event = @event;
            }


            public void Dispose()
            {
                _event._subs.Remove(_action);
            }
        }
    }

    public class ReactiveEvent<T>
    {
        List<Action<T>> _subs = new();

        internal IDisposable Subscribe(Action<T> action)
        {
            _subs.Add(action);
            return new UnsubHandler<T>(action, this);
        }

        internal void Invoke(T value)
        {
            foreach (var item in _subs)
            {
                item(value);
            }
        }

        class UnsubHandler<T> : IDisposable
        {
            private readonly Action<T> _action;
            private readonly ReactiveEvent<T> _event;

            public UnsubHandler(Action<T> action, ReactiveEvent<T> @event)
            {
                _action = action;
                _event = @event;
            }


            public void Dispose()
            {
                _event._subs.Remove(_action);
            }
        }
    }
}
