using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Tests.Data;

public class Record
{
    public Guid Id { get; } = Guid.NewGuid();

    public readonly Mutable<string> Text = "";
    public readonly Mutable<int> Rating = 0;
    public readonly Mutable<Record?> Related = new(null);
}