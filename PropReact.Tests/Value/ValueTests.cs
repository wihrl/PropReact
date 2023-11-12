using PropReact.Chain;
using PropReact.Props;
using PropReact.Tests.Value;
using Record = PropReact.Tests.Value.Record;

namespace PropReact.Tests;

public class ValueTests : CompositeDisposable
{
    private ValueData Data { get; } = new();

    [Fact]
    void SingleValue()
    {
        var changes = 0;

        Prop.Watch(this)
            .ChainConstant(x => x.Data)
            .ChainValue(x => x.Int)
            .Immediate()
            .React(() => changes++)
            .Start(this);

        Assert.Equal(0, changes);

        Data.Int.Value = 2;
        Assert.Equal(1, changes);

        Data.Int.Value = 0;
        Assert.Equal(2, changes);

        Dispose();

        Data.Int.Value = 1;
        Assert.Equal(2, changes);
    }

    [Fact]
    void ChainedValue()
    {
        var changes = 0;
        var expected = 1;

        Prop.Watch(this)
            .ChainConstant(x => x.Data)
            .ChainValue(x => x.NullableRecord)
            .ChainValue(x => x.Related)
            .ChainValue(x => x.Text)
            .Immediate()
            .React(() => changes++, true)
            .Start(this);

        Assert.Equal(expected, changes);

        var record = new Record();
        var related = new Record();

        Data.NullableRecord.Value = record;
        Assert.Equal(++expected, changes);


        Data.NullableRecord.Value.Related.Value = related;
        Assert.Equal(++expected, changes);

        Data.NullableRecord.Value.Related.Value!.Text.Value = "12";
        Assert.Equal(++expected, changes);

        Data.NullableRecord.Value.Related.Value!.Text.Value = "12";
        Assert.Equal(expected, changes);

        related.Text.Value = "34";
        Assert.Equal(++expected, changes);

        Data.NullableRecord.Value.Related.v = null;
        Assert.Equal(++expected, changes);

        Data.NullableRecord.v = null;
        Assert.Equal(++expected, changes);

        record.Related.v = related;
        Assert.Equal(expected, changes);

        related.Text.v = "foo";
        Assert.Equal(expected, changes);

        Dispose();

        Data.NullableRecord.v = record;
        Assert.Equal(expected, changes);
    }

    [Fact]
    void ChainLoop()
    {
        
    }

    [Fact]
    void BranchingValues()
    {
    }

    [Fact]
    void ComputedImmediate()
    {
    }
}