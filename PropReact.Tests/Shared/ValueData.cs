using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Tests.Data;

public class ValueData
{
    public readonly Mutable<int> Int = 0;
    public readonly Mutable<string?> NullableString = new(null);
    public readonly Mutable<Record> Record = new Record();
    public readonly Mutable<Record?> NullableRecord = new(null);
    public readonly Mutable<Mutable<bool>> Nested = new(false);
}