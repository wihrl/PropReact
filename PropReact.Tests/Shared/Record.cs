using PropReact.Props.Value;

namespace PropReact.Tests.Shared;

public class Record
{
    public Guid Id { get; } = Guid.NewGuid();

    public Record()
    {
    }

    public Record(Guid guid) => Id = guid;

    public readonly Mutable<string> Text = "";
    public readonly Mutable<int> Rating = 0;
    public readonly Mutable<Record?> Related = new(null);
}