namespace PropReact.Tests;

public class BasicTests
{
    private TestHierarchy Hierarchy { get; } = new();

    [Fact]
    public void SimpleProp()
    {
        int changes = 0;

        var disposable = Prop.Watch(Hierarchy, x => x.Name, x => changes++);
        Assert.Equal(0, changes);

        Hierarchy.Id.Value = 2;
        Assert.Equal(0, changes);

        Hierarchy.Name.Value = "asdf";
        Assert.Equal(1, changes);

        Hierarchy.Name.Value = "asdf2";
        Assert.Equal(2, changes);

        disposable.Dispose();

        Hierarchy.Name.Value = "asdf3";
        Assert.Equal(2, changes);
    }

    [Fact]
    public void NestedProp()
    {
        int changes = 0;

        var disposable = Prop.Watch(Hierarchy, x => x.Nested1.Value.Nested2.Value.Value1, x => changes++);
        Assert.Equal(0, changes);

        Hierarchy.Id.Value = 2;
        Assert.Equal(0, changes);

        Hierarchy.Nested1.Value.Nested2.Value.Value1.Value = 2;
        Assert.Equal(1, changes);

        Hierarchy.Nested1.Value = new();
        Assert.Equal(2, changes);

        Hierarchy.Nested1.Value.Nested2.Value.Value1.Value = 3;
        Assert.Equal(3, changes);

        disposable.Dispose();

        Hierarchy.Nested1.Value.Nested2.Value.Value1.Value = 4;
        Assert.Equal(3, changes);
    }

    [Fact]
    public void BranchingProp()
    {
        int changes = 0;

        var disposable = Prop.Watch(Hierarchy, x => new { V1 = x.Name.Value, V2 = x.Name.Value }, x => changes++);

        Hierarchy.Name.Value = "asdf";
        Assert.Equal(1, changes);

        disposable.Dispose();

        Hierarchy.Name.Value = "asdf3";
        Assert.Equal(1, changes);
    }

    [Fact]
    public void MutableNonReactivePropInSelector()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Prop.Watch(Hierarchy, x => x.Nested1NoPropMutable.Value1.Value, x => { });
        });
    }

    [Fact]
    public void FieldInSelector()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Prop.Watch(Hierarchy, x => x.Nested1Field.Value, x => { });
        });
    }

    [Fact]
    public void NavigableProp()
    {
        Assert.Equal(Hierarchy, Hierarchy.NestedNavigable.Value.NavProp.V);
        
        var newNested = new Nested1();
        Hierarchy.NestedNavigable.Value = newNested;
        
        Assert.Equal(Hierarchy, Hierarchy.NestedNavigable.Value.NavProp.V);
    }
}

// TEST IDEAS:
// reactive collections multiple identical values sub/unsub
// loops
// unsupported expression tree constructs