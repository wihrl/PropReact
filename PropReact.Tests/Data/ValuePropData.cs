using PropReact.Collections;
using PropReact.Properties;

namespace PropReact.Tests;

public partial class ValuePropData
{
    private readonly IMutable<int> _int;
    private readonly IMutable<string?> _nullableString;
    private readonly IMutable<Record> _record;
    private readonly IMutable<Record?> _nullableRecord;
    public IMutable<int> Test { get; set; }

    public ValuePropData()
    {
        Prop.Mutable(out _int, out _nullableRecord, out _nullableString, out _record, new());

        
        NullableRecord = new();
    }
}

public partial class Record
{
    public Guid Id { get; } = Guid.NewGuid();

    private readonly IMutable<string> _text;
    private readonly IMutable<int> _rating;
    private readonly IMutable<Record?> _related;

    public void __hiden()
    {
        
    }

    public Record()
    {
        Prop.Mutable(out _related, out _rating, out _text, "");
    }
}