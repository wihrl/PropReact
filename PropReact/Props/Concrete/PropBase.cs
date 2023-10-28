using PropReact.Chain;

namespace PropReact.Properties;

public abstract class PropBase<TValue> : IProp<TValue>
{
    private readonly Dictionary<IPropObserver<TValue>, HashSet<Action>> _reactionsMap = new();

    
    void IProp<TValue>.Sub(IPropObserver<TValue> propObserver, IReadOnlyCollection<Action> reactions)
    {
        if (!_reactionsMap.TryGetValue(propObserver, out var storedReactions))
            _reactionsMap[propObserver] = storedReactions = new();
        
        foreach (var reaction in reactions) storedReactions.Add(reaction);
    }

    void IProp<TValue>.Unsub(IPropObserver<TValue> propObserver, IReadOnlyCollection<Action> reactions)
    {
        if (!_reactionsMap.TryGetValue(propObserver, out var storedReactions))
            throw new("Cannot unsub an observer which isn't subscribed!");
        
        foreach (var reaction in reactions) storedReactions.Remove(reaction);

        if (storedReactions.Count == 0)
            _reactionsMap.Remove(propObserver);
    }

    protected void TriggerReactions(TValue? oldValue, TValue? newValue)
    {
        foreach (var reactionsMapKey in _reactionsMap.Keys)
        {
            reactionsMapKey.PropChanged(oldValue, newValue, _reactionsMap.Values);
        }
    }
}