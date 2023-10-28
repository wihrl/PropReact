using PropReact.Properties;

namespace PropReact.Tests;

public class TestHierarchy
{
    public TestHierarchy()
    {
        Prop.Make(out _id, 1, out _name, "", out _nested1Field, new());
    }


    private readonly IMutable<int> _id;
    private readonly IMutable<string> _name;
    private readonly IMutable<Nested1?> _nested1;
    private readonly Nested1 _nested1NoPropMutable;
    private readonly Nested1 _nested1NoPropImmutable;
    private readonly IMutable<Nested1> _nested1Field;

    public IMutable<Nested1> NestedNavigable { get; }
}

class b : Nested1
{
}

public class Nested1
{
    public IMutable<int> Value1 { get; } = Prop.Make(0);
    public IMutable<string> Value2 { get; } = Prop.Make("");
    public IMutable<Nested2> Nested2 { get; } = Prop.Make(new Nested2());

    public INavProp<TestHierarchy> NavProp { get; } = Prop.MakeNav<TestHierarchy>();
}

public class Nested2
{
    public IMutable<int> Value1 { get; } = Prop.Make(0);
    public IMutable<string> Value2 { get; } = Prop.Make("");
}