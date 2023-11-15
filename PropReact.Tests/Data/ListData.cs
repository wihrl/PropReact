using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Tests.Data;

public class ListData
{
    public readonly ReactiveList<Record> Records = new();
    public readonly Mutable<ReactiveList<Record>> MutableRecords = new(new());

    public ListData()
    {
    }
}