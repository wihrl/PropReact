using PropReact.Properties;

namespace PropReact.Tests;

public class TestHierarchy
{
    public TestHierarchy()
    {
        NestedNavigable = Prop.MakeNavigable(this, new Nested1(), x => x.NavProp);
    }

    public IProp<int> Id { get; } = Prop.Make(0);
    public IProp<string> Name { get; } = Prop.Make("");
    public IProp<Nested1?> Nested1 { get; } = Prop.Make(new Nested1());

    public Nested1 Nested1NoPropMutable { get; set; } = new();
    public Nested1 Nested1NoPropImmutable { get; } = new();

    public readonly IProp<Nested1> Nested1Field = Prop.Make(new Nested1());


    public IProp<Nested1> NestedNavigable { get; }
}

public class Nested1
{
    public IProp<int> Value1 { get; } = Prop.Make(0);
    public IProp<string> Value2 { get; } = Prop.Make("");
    public IProp<Nested2> Nested2 { get; } = Prop.Make(new Nested2());

    public INavProp<TestHierarchy> NavProp { get; } = Prop.MakeNav<TestHierarchy>();
}

public class Nested2
{
    public IProp<int> Value1 { get; } = Prop.Make(0);
    public IProp<string> Value2 { get; } = Prop.Make("");
}