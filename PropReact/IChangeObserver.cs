namespace PropReact;

internal interface IChangeObserver
{
    void OwnedValueChanged(object? oldValue, object? newValue);
}

//internal record ChangeSet(object[]? Added, object[]? Removed);