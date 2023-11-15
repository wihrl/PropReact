using PropReact.Props;
using PropReact.Props.Value;

namespace PropReact.Tests.Data;

public class Record
{
    public Guid Id { get; } = Guid.NewGuid();

    public readonly IMutable<string> Text;
    public readonly IMutable<int> Rating;
    public readonly IMutable<Record?> Related;

    public Record() => Prop.Mutable(out Related, out Rating, out Text, "");
}