namespace PropReact.Tests;

public class CollectionTests
{
    private CollectionTestHierarchy Hierarchy { get; } = new();

    [Fact]
    public void BasicList()
    {
        var changes = 0;
        Prop.Watch(Hierarchy, x => x.Items1.Select(y => y.Value1), x => changes++);

        Assert.Equal(0, changes);

        Hierarchy.Items1.Add(new());
        Assert.Equal(1, changes);

        Hierarchy.Items1[0].Value1.Value = "asdf";
        Assert.Equal(2, changes);

        Hierarchy.Items1.Add(new());
        Hierarchy.Items1.Clear();


        Assert.Equal(5, changes);
    }

    [Fact]
    public void NestedList()
    {
        var changes = 0;
        Prop.Watch(Hierarchy, x => x.Items1.Select(y => y.Items2.Select(x => x.Value2)), x => changes++);

        Assert.Equal(0, changes);

        Hierarchy.Items1.Add(new());
        Assert.Equal(1, changes);

        Hierarchy.Items1[0].Items2.Add(new());
        Assert.Equal(2, changes);
        
        Hierarchy.Items1[0].Items2[0].Value2.Value = 2;
        Assert.Equal(3, changes);
        
        // todo: remove, replace, map, same values, ...
    }

    [Fact]
    public void MapWatcher()
    {
        var guid = Guid.NewGuid();
        var prop = Hierarchy.Items1MapProp.WatchAt(guid);
        
        Assert.Null(prop.V);
        
        Hierarchy.Items1MapProp.Add(new());
        Assert.Null(prop.V);

        var val = new CollectionTestHierarchy.Item1() { Id = guid };
        Hierarchy.Items1MapProp.Add(val);
        Assert.Equal(val, prop.V);
        
        Hierarchy.Items1MapProp.Remove(val);
        Assert.Null(prop.V);
    }
    
    // todo: Collection.Length (properties on the collection instead of nested ones)
}