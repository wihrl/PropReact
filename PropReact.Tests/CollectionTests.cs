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
        int expected;

        Prop.Watch(this)
            .ChainConstant(x => x.Data.Records)
            .Enter()
            .Immediate()
            .React(() => changes++)
            .Start(this);

        Assert.Equal(0, changes);

        Data.Records.Add(new());
        Data.Records.Add(new());
        Data.Records.Add(new());
        Data.Records.Add(new());

        Assert.Equal(expected = 4, changes);

        Data.Records.Insert(0, new());
        Assert.Equal(++expected, changes);

        Data.Records.RemoveAt(1);
        Assert.Equal(++expected, changes);

        Data.Records.Remove(Data.Records[0]);
        Assert.Equal(++expected, changes);

        Data.Records[1] = new();
        Assert.Equal(++expected, changes);

        expected += Data.Records.Count;
        Data.Records.Clear();
        Assert.Equal(expected, changes);
        
        Dispose();
        
        Data.Records.Add(new());
        Assert.Equal(expected, changes);
    }

    [Fact]
    public void ListAsValue()
    {
        var changes = 0;
        int expected;

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
        var changes = 0;
        int expected;

        Prop.Watch(this)
            .ChainConstant(x => x.Data.Records)
            .Enter()
            .Branch(
                y => y.ChainValue(x => x.Rating),
                y => y.ChainValue(x => x.Text))
            .Immediate()
            .React(() => changes++)
            .Start(this);

        Assert.Equal(0, changes);

        Data.Records.Add(new());
        Data.Records.Add(new());
        Data.Records.Add(new());
        Data.Records.Add(new());

        Assert.Equal(expected = 4, changes);

        Data.Records[1].Text.v = "asd";
        Assert.Equal(++expected, changes);

        Data.Records[2].Rating.v = 12;
        Assert.Equal(++expected, changes);
        
        Dispose();
        
        Data.Records[2].Rating.v = 21;
        Assert.Equal(expected, changes);
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