using PropReact.Properties;

namespace PropReact.Tests;

public class TestHierarchy
{
    public TestHierarchy()
    {
        NestedNavigable = Prop.MakeNavigable(this, new Nested1(), x => x.NavProp);
    }

    public IMutable<int> Id { get; } = Prop.Make(0);
    public IMutable<string> Name { get; } = Prop.Make("");
    public IMutable<Nested1?> Nested1 { get; } = Prop.Make(new Nested1());

    public Nested1 Nested1NoPropMutable { get; set; } = new();
    public Nested1 Nested1NoPropImmutable { get; } = new();

    public readonly IMutable<Nested1> Nested1Field = Prop.Make(new Nested1());


    public IMutable<Nested1> NestedNavigable { get; }
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