using PropReact.Chain;
using PropReact.Props;
using PropReact.Tests.Value;

namespace PropReact.Tests;

public class ValueTests : CompositeDisposable
{
    private ValueData Data { get; } = new();
    
    [Fact]
    void SingleValue()
    {
        var changes = 0;

        Prop.Watch(this)
            .ThenConstant(x => x.Data)
            .Then(x => x.Int)
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