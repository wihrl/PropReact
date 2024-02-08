using PropReact.Chain;
using PropReact.Props;
using PropReact.Props.Collections;
using PropReact.Props.Value;
using Record = PropReact.Tests.Data.Record;

namespace PropReact.Tests.Collections;

public class ListTests : CompositeDisposable
{
    class ListData
    {
        public readonly ReactiveList<Record> Records = new();
        public readonly Mutable<ReactiveList<Record>> MutableRecords = new(new());
    }

    private ListData Data { get; } = new();

    [Fact]
    public void Basic()
    {
        var changes = 0;
        int expected;

        Chain.Chain.From(this)
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

        Chain.Chain.From(this)
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
    public void EnteredChain()
    {
        var changes = 0;
        int expected;

        Chain.Chain.From(this)
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
    public void RepeatedValues()
    {
        var changes = 0;
        int expected;

        Chain.Chain.From(this)
            .ChainConstant(x => x.Data.Records)
            .Enter()
            .ChainValue(x => x.Text)
            .Immediate()
            .React(() => changes++)
            .Start(this);

        var record = new Record();

        Data.Records.Add(record);
        Data.Records.Add(record);
        Assert.Equal(expected = 2, changes);

        // should only trigger reactions once
        record.Text.v = "asdf";
        Assert.Equal(++expected, changes);

        Data.Records.RemoveAt(0);
        Assert.Equal(++expected, changes); // reactions are triggered on any change along the chain 

        // one instance is still in the collection => should still trigger reactions
        record.Text.v = "asdf2";
        Assert.Equal(++expected, changes);
    }

    [Fact]
    public void WatchAt()
    {
        var changes = 0;
        int expected = 0;

        Chain.Chain.From(this)
            .ChainConstant(x => x.Data.Records)
            .EnterAt(1)
            .ChainValue(x => x.Text)
            .Immediate()
            .React(() => changes++)
            .Start(this);

        Assert.Equal(expected, changes);

        Data.Records.Add(new());
        Assert.Equal(expected, changes);

        Data.Records.Add(new());
        Assert.Equal(++expected, changes);

        Data.Records[0].Text.v = "asdf1";
        Assert.Equal(expected, changes);

        Data.Records[1].Text.v = "asdf2";
        Assert.Equal(++expected, changes);

        Data.Records[0] = new();
        Assert.Equal(expected, changes);

        var record = new Record();
        Data.Records[1] = record;
        Assert.Equal(++expected, changes);

        Data.Records.Clear();
        Assert.Equal(++expected, changes);

        record.Text.v = "zxcv";
        Assert.Equal(expected, changes);

        Dispose();

        Data.Records.Add(new());
        Data.Records.Add(new());
        Assert.Equal(expected, changes);
    }
}