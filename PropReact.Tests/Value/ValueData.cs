using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Tests.Value;

public class ValueData
{
    public readonly IMutable<int> Int;
    public readonly IMutable<string?> NullableString;
    public readonly IMutable<Record> Record;
    public readonly IMutable<Record?> NullableRecord;

    public ValueData() => Prop.Mutable(out Int, out NullableRecord, out NullableString, out Record, new());
}

public partial class Record
{
    public Guid Id { get; } = Guid.NewGuid();

    public readonly IMutable<string> Text;
    public readonly IMutable<int> Rating;
    public readonly IMutable<Record?> Related;

    public Record() => Prop.Mutable(out Related, out Rating, out Text, "");
}