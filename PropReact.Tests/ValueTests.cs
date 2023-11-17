using PropReact.Chain;
using PropReact.Props;
using PropReact.Tests.Data;
using Record = PropReact.Tests.Data.Record;

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
    void ChainedConstants()
    {
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

        var a = new Record();
        var b = new Record();

        Data.NullableRecord.Value = a;
        Assert.Equal(++expected, changes);


        Data.NullableRecord.Value.Related.Value = b;
        Assert.Equal(++expected, changes);

        Data.NullableRecord.Value.Related.Value!.Text.Value = "12";
        Assert.Equal(++expected, changes);

        Data.NullableRecord.Value.Related.Value!.Text.Value = "12";
        Assert.Equal(expected, changes);

        b.Text.Value = "34";
        Assert.Equal(++expected, changes);

        Data.NullableRecord.Value.Related.v = null;
        Assert.Equal(++expected, changes);

        Data.NullableRecord.v = null;
        Assert.Equal(++expected, changes);

        a.Related.v = b;
        Assert.Equal(expected, changes);

        b.Text.v = "foo";
        Assert.Equal(expected, changes);

        Dispose();

        Data.NullableRecord.v = a;
        Assert.Equal(expected, changes);
    }

    [Fact]
    void ChainLoop()
    {
        var changes = 0;
        var expected = 1;

        Prop.Watch(this)
            .ChainConstant(x => x.Data)
            .ChainValue(x => x.NullableRecord)
            .ChainValue(x => x.Related)
            .ChainValue(x => x.Related)
            .ChainValue(x => x.Text)
            .Immediate()
            .React(() => changes++, true)
            .Start(this);

        Assert.Equal(expected, changes);

        var a = new Record();
        var b = new Record();

        Data.NullableRecord.v = a;
        Assert.Equal(++expected, changes);

        Data.NullableRecord.v.Related.v = b;
        Assert.Equal(++expected, changes);

        Data.NullableRecord.v.Related.v!.Text.v = "12";
        Assert.Equal(expected, changes);

        Data.NullableRecord.v.Related.v!.Related.v = a;
        Assert.Equal(++expected, changes);

        b.Text.v = "34";
        Assert.Equal(expected, changes);

        a.Text.v = "34";
        Assert.Equal(++expected, changes); // React() should only be called once

        Dispose();

        a.Text.v = "56";
        Assert.Equal(expected, changes);
    }

    [Fact]
    void Branching()
    {
        var changes = 0;
        var expected = 0;

        Prop.Watch(this)
            .ChainConstant(x => x.Data)
            .Branch(
                y => y.ChainValue(x => x.Int),
                y => y.Branch(
                    z => z.ChainValue(x => x.NullableString),
                    z => z.ChainValue(x => x.NullableRecord).ChainValue(x => x.Text)
                )
            )
            .Immediate()
            .React(() => changes++)
            .Start(this);

        Assert.Equal(expected, changes);

        Data.Int.v = 123;
        Assert.Equal(++expected, changes);

        Data.Int.v = 123;
        Assert.Equal(expected, changes);

        Data.NullableString.v = "asdf";
        Assert.Equal(++expected, changes);

        Data.NullableRecord.v = new();
        Assert.Equal(++expected, changes);

        Data.NullableRecord.v.Text.v = "zxcv";
        Assert.Equal(++expected, changes);

        Dispose();

        Data.NullableRecord.v.Text.v = "zxcv2";
        Assert.Equal(expected, changes);

        Data.NullableString.v = "zxcv2";
        Assert.Equal(expected, changes);
    }

    [Fact]
    void NestedMutable()
    {
        var changes = 0;

        Prop.Watch(this)
            .ChainConstant(x => x.Data)
            .ChainValue(x => x.Nested)
            .ChainValue(x => x)
            .Immediate()
            .React(() => changes++)
            .Start(this);

        Assert.Equal(0, changes);

        var val = Data.Nested.v;
        
        Data.Nested.v.v = true;
        Assert.Equal(1, changes);

        Data.Nested.v = new(false);
        Assert.Equal(2, changes);

        Dispose();

        val.v = true;
        val.v = false;
        Assert.Equal(2, changes);
    }
}