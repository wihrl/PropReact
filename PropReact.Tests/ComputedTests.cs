using PropReact.Chain;
using PropReact.Props.Value;

namespace PropReact.Tests;

public class ComputedTests : CompositeDisposable
{
    private readonly Mutable<int> _a = 0;

    [Fact]
    void Sync()
    {
        IComputed<int> computed;

        Watch.From(this)
            .ChainValue(x => x._a)
            .Immediate()
            .Compute(() => _a.v + 1, out computed)
            .Start(this);

        Assert.Equal(1, computed.Value);
        _a.Value = 5;
        Assert.Equal(6, computed.Value);

        Dispose();

        _a.Value = 10;
        Assert.Equal(6, computed.Value);
    }

    [Fact]
    void Async()
    {
        IComputedAsync<string> computed;

        Watch.From(this)
            .ChainValue(x => x._a)
            .Throttled(100)
            .ComputeAsync(async (x) =>
            {
                await Task.Delay(100, x);

                return "Formatted " + _a.v;
            }, out computed, "default")
            .Start(this);

        Assert.Equal("default", computed.Value);
        SpinWait.SpinUntil(() => computed.Value != "default", 1000);
        Assert.Equal("Formatted 0", computed.Value);

        _a.Value = 5;
        SpinWait.SpinUntil(() => computed.Value != "Formatted 0", 1000);
        Assert.Equal("Formatted 5", computed.Value);
        
        Dispose();
        
        _a.Value = 10;
        Assert.Equal("Formatted 5", computed.Value);
    }
}