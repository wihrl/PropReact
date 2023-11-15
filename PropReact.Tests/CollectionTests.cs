using PropReact.Chain;
using PropReact.Props;
using PropReact.Props.Collections;
using PropReact.Tests.Data;
using PropReact.Tests.Value;
using Record = PropReact.Tests.Data.Record;

namespace PropReact.Tests;

public class CollectionTests : CompositeDisposable
{
    private ListData Data { get; } = new();

    [Fact]
    public void ListSimple()
    {
        var changes = 0;
        Prop.Watch(this)
            .ChainConstant(x => x.Data.Records)
            .Enter()
            .Immediate()
            .React(() => changes++)
            .Start();

        Assert.Equal(0, changes);

        // todo: try all ops
        Data.Records.Add(new());
        Data.Records.Clear();
        Assert.Equal(2, changes);
    }

    [Fact]
    public void ListAsValue()
    {
        var changes = 0;
        var expected = 0;
        Prop.Watch(this)
            .ChainConstant(x => x.Data)
            .ChainValue(x => x.MutableRecords)
            .Enter()
            .Immediate()
            .React(() => changes++)
            .Start(this);

        var primary = Data.MutableRecords.v;
        var secondary = new ReactiveList<Record>();

        Data.MutableRecords.v.Add(new());
        Data.MutableRecords.v.Clear();
        Assert.Equal(expected = 2, changes);

        Data.MutableRecords.v = secondary;
        Assert.Equal(++expected, changes);

        primary.Add(new());
        Assert.Equal(expected, changes);
        
        secondary.Add(new());
        Assert.Equal(++expected, changes);
        
        Dispose();
        
        secondary.Add(new());
        Assert.Equal(expected, changes);
    }

    [Fact]
    public void ListEnteredChain()
    {
    }

    [Fact]
    public void ListRepeatedValues()
    {
    }

    [Fact]
    public void ListWatchAt()
    {
    }
}

// namespace PropReact.Tests;
//
// public class CollectionTests
// {
//     private ValuePropData Data { get; } = new();
//
//     [Fact]
//     public void BasicList()
//     {
//         var changes = 0;
//         Prop.Watch(Data, x => x.Items1.Select(y => y.Value1), x => changes++);
//
//         Assert.Equal(0, changes);
//
//         Data.Items1.Add(new());
//         Assert.Equal(1, changes);
//
//         Data.Items1[0].Value1.Value = "asdf";
//         Assert.Equal(2, changes);
//
//         Data.Items1.Add(new());
//         Data.Items1.Clear();
//
//
//         Assert.Equal(5, changes);
//     }
//
//     [Fact]
//     public void NestedList()
//     {
//         var changes = 0;
//         Prop.Watch(Data, x => x.Items1.Select(y => y.Items2.Select(x => x.Value2)), x => changes++);
//
//         Assert.Equal(0, changes);
//
//         Data.Items1.Add(new());
//         Assert.Equal(1, changes);
//
//         Data.Items1[0].Items2.Add(new());
//         Assert.Equal(2, changes);
//         
//         Data.Items1[0].Items2[0].Value2.Value = 2;
//         Assert.Equal(3, changes);
//         
//         // todo: remove, replace, map, same values, ...
//     }
//
//     [Fact]
//     public void MapWatcher()
//     {
//         var guid = Guid.NewGuid();
//         var prop = Data.Items1MapProp.WatchAt(guid);
//         
//         Assert.Null(prop.V);
//         
//         Data.Items1MapProp.Add(new());
//         Assert.Null(prop.V);
//
//         var val = new ValuePropData.Item1() { Id = guid };
//         Data.Items1MapProp.Add(val);
//         Assert.Equal(val, prop.V);
//         
//         Data.Items1MapProp.Remove(val);
//         Assert.Null(prop.V);
//     }
//     
//     // todo: Collection.Length (properties on the collection instead of nested ones)
// }