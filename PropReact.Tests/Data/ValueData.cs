using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Tests.Data;

public class ValueData
{
    public readonly IMutable<int> Int;
    public readonly IMutable<string?> NullableString;
    public readonly IMutable<Record> Record;
    public readonly IMutable<Record?> NullableRecord;

    public ValueData() => Prop.Mutable(out Int, out NullableRecord, out NullableString, out Record, new());
}