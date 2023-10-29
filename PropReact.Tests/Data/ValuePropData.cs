using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Tests;

public partial class ValuePropData
{
    private readonly IMutable<int> _int;
    private readonly IMutable<string?> _nullableString;
    private readonly IMutable<Record> _record;
    private readonly IMutable<Record?> _nullableRecord;

    public ValuePropData()
    {
        Prop.Mutable(out _int, out _nullableRecord, out _nullableString, out _record, new());

        _props._record(this);
        
        NullableRecord = new();
    }
}

public partial class Record
{
    public Guid Id { get; } = Guid.NewGuid();

    private readonly IMutable<string> _text;
    private readonly IMutable<int> _rating;
    private readonly IMutable<Record?> _related;

    public Record()
    {
        Prop.Mutable(out _related, out _rating, out _text, "");
    }
}