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

        Hierarchy.Id.V = 2;
        Assert.Equal(0, changes);

        Hierarchy.Name.V = "asdf";
        Assert.Equal(1, changes);

        Hierarchy.Name.V = "asdf2";
        Assert.Equal(2, changes);

        disposable.Dispose();

        Hierarchy.Name.V = "asdf3";
        Assert.Equal(2, changes);
    }

    [Fact]
    public void NestedProp()
    {
        int changes = 0;

        var disposable = Prop.Watch(Hierarchy, x => x.Nested1.V.Nested2.V.Value1, x => changes++);
        Assert.Equal(0, changes);

        Hierarchy.Id.V = 2;
        Assert.Equal(0, changes);

        Hierarchy.Nested1.V.Nested2.V.Value1.V = 2;
        Assert.Equal(1, changes);

        Hierarchy.Nested1.V = new();
        Assert.Equal(2, changes);

        Hierarchy.Nested1.V.Nested2.V.Value1.V = 3;
        Assert.Equal(3, changes);

        disposable.Dispose();

        Hierarchy.Nested1.V.Nested2.V.Value1.V = 4;
        Assert.Equal(3, changes);
    }

    [Fact]
    public void BranchingProp()
    {
        int changes = 0;

        var disposable = Prop.Watch(Hierarchy, x => new { V1 = x.Name.V, V2 = x.Name.V }, x => changes++);

        Hierarchy.Name.V = "asdf";
        Assert.Equal(1, changes);

        disposable.Dispose();

        Hierarchy.Name.V = "asdf3";
        Assert.Equal(1, changes);
    }

    [Fact]
    public void MutableNonReactivePropInSelector()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Prop.Watch(Hierarchy, x => x.Nested1NoPropMutable.Value1.V, x => { });
        });
    }

    [Fact]
    public void FieldInSelector()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Prop.Watch(Hierarchy, x => x.Nested1Field.V, x => { });
        });
    }

    [Fact]
    public void NavigableProp()
    {
        Assert.Equal(Hierarchy, Hierarchy.NestedNavigable.V.NavProp.V);
        
        var newNested = new Nested1();
        Hierarchy.NestedNavigable.V = newNested;
        
        Assert.Equal(Hierarchy, Hierarchy.NestedNavigable.V.NavProp.V);
    }
}

// TEST IDEAS:
// reactive collections multiple identical values sub/unsub
// loops
// unsupported expression tree constructs