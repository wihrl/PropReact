using PropReact.Chain;
using PropReact.Props;
using PropReact.Props.Collections;
using PropReact.Props.Value;
using PropReact.Utils;
using Record = PropReact.Tests.Shared.Record;

namespace PropReact.Tests.Collections;

public class MapTests : CompositeDisposable
{
    class MapData
    {
        public readonly ReactiveMap<Record, Guid> Records = new(x => x.Id);
        public readonly Mutable<ReactiveMap<Record, Guid>> MutableRecords = new(new(x => x.Id));
    }

    private MapData Data { get; } = new();

    [Fact]
    public void General()
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

        Data.Records.Add(new());
        Assert.Equal(++expected, changes);

        Data.Records.RemoveAt(Data.Records.First().Id);
        Assert.Equal(++expected, changes);

        Data.Records.Remove(Data.Records.First());
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
        var secondary = new ReactiveMap<Record, Guid>(x => x.Id);

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

        var record1 = new Record();
        var record2 = new Record();

        Data.Records.Add(record1);
        Data.Records.Add(record2);
        Data.Records.Add(new());
        Data.Records.Add(new());

        Assert.Equal(expected = 4, changes);

        Data.Records[record1.Id]!.Text.v = "asd";
        Assert.Equal(++expected, changes);

        Data.Records[record2.Id]!.Rating.v = 12;
        Assert.Equal(++expected, changes);

        Dispose();

        Data.Records[record2.Id]!.Rating.v = 21;
        record2.Rating.v = 22;
        Assert.Equal(expected, changes);
    }

    [Fact]
    public void ListRepeatedValues()
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
        Assert.Equal(expected = 1, changes);

        record.Text.v = "asdf";
        Assert.Equal(++expected, changes);

        Data.Records.RemoveAt(record.Id);
        Assert.Equal(++expected, changes); 
        
        record.Text.v = "asdf2";
        Assert.Equal(expected, changes);
    }

    [Fact]
    public void ListWatchAt()
    {
        var changes = 0;
        int expected = 0;

        var guids = Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToArray();

        Chain.Chain.From(this)
            .ChainConstant(x => x.Data.Records)
            .EnterAt(guids[1])
            .ChainValue(x => x.Text)
            .Immediate()
            .React(() => changes++)
            .Start(this);

        Assert.Equal(expected, changes);

        Data.Records.Add(new(guids[0]));
        Assert.Equal(expected, changes);

        Data.Records.Add(new(guids[1]));
        Assert.Equal(++expected, changes);

        Data.Records[guids[0]]!.Text.v = "asdf1";
        Assert.Equal(expected, changes);

        Data.Records[guids[1]]!.Text.v = "asdf2";
        Assert.Equal(++expected, changes);

        Data.Records.Add(new(guids[0]));
        Assert.Equal(expected, changes);

        var record = new Record(guids[1]);
        Data.Records.AddOrReplace(record);
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