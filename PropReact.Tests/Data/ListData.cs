using PropReact.Props;
using PropReact.Props.Collections;
using PropReact.Props.Value;

namespace PropReact.Tests.Value;

public class ListData
{
    public IListProp<Record> Records;
    public IMutable<IListProp<Record>> MutableRecords;

    public ListData()
    {
    }
}