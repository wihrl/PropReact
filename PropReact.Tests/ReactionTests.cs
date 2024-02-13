using System.Diagnostics;
using PropReact.Chain;
using PropReact.Props;
using PropReact.Props.Value;
using PropReact.Utils;

namespace PropReact.Tests;

public class ReactionTests : CompositeDisposable
{
    private readonly IMutable<int> _int = new Mutable<int>(0);

    [Fact]
    void Immediate()
    {
        var counter1 = 0;
        var counter2 = 0;

        Chain.Chain.From(this)
            .ChainValue(x => x._int)
            .Immediate()
            .React(() => counter1++, true)
            .React(() => counter2++)
            .Start(this);

        Assert.Equal(1, counter1);
        Assert.Equal(0, counter2);

        _int.Value = 1;

        Assert.Equal(2, counter1);
        Assert.Equal(1, counter2);

        Dispose();

        _int.Value = 2;

        Assert.Equal(2, counter1);
        Assert.Equal(1, counter2);
    }

    [Fact]
    void Throttled() => ThrottledInternal(false);

    [Fact]
    void ThrottledImmediate() => ThrottledInternal(true);

    void ThrottledInternal(bool immediate)
    {
        const int delay = 100;
        var counter = 0;
        var expected = 0;
        var ready = false;
        var sw = Stopwatch.StartNew();

        Chain.Chain.From(this)
            .ChainValue(x => x._int)
            .Throttled(delay, ThrottleMode.Extendable | ThrottleMode.ImmediateExtendable)
            .React(() =>
            {
                counter++;
                ready = true;
            })
            .Start(this);

        Assert.Equal(expected, counter);

        var time = sw.ElapsedMilliseconds;

        void TriggerInitial()
        {
            _int.Value++;
            Assert.Equal(++expected, counter);
            ready = false;
            time = sw.ElapsedMilliseconds;
        }

        // basic
        {
            TriggerInitial();

            _int.Value++;
            Assert.Equal(expected, counter);
            SpinWait.SpinUntil(() => ready, delay * 10);
            ready = false;

            Assert.Equal(++expected, counter);
            CheckDelta(sw, ref time, delay);
        }

        // delay reset
        {
            TriggerInitial();

            _int.Value++;
            Assert.Equal(expected, counter);

            Thread.Sleep(delay / 2);
            Assert.Equal(expected, counter);

            _int.Value++;
            Assert.Equal(expected, counter);
            time = sw.ElapsedMilliseconds;

            SpinWait.SpinUntil(() => ready, delay * 10);
            Assert.Equal(++expected, counter);
            CheckDelta(sw, ref time, delay);
        }

        Dispose();

        _int.Value++;
        Thread.Sleep(delay * 2);
        Assert.Equal(expected, counter);
    }

    void CheckDelta(Stopwatch sw, ref long last, int expected, int maxDeviation = 40)
    {
        var delta = sw.ElapsedMilliseconds - last;
        Assert.True(delta <= expected + maxDeviation, $"Expected at most {expected + maxDeviation}ms, got {delta}ms");
        Assert.True(delta >= expected, $"Expected at least {expected}ms, got {delta}ms");
        last = sw.ElapsedMilliseconds;
    }

    [Fact]
    void Async()
    {
        var counter = 0;
        var expected = 0;
        var sw = Stopwatch.StartNew();

        Chain.Chain.From(this)
            .ChainValue(x => x._int)
            .Immediate()
            .ReactAsync(async x =>
            {
                await Task.Delay(100, x);
                counter++;
            })
            .Start(this);

        Assert.Equal(expected, counter);

        _int.Value++;
        Assert.Equal(expected, counter);
        Thread.Sleep(50);
        Assert.Equal(expected, counter);
        _int.Value++;
        Assert.Equal(expected, counter);
        Thread.Sleep(200);
        Assert.Equal(++expected, counter);
    }

    [Fact]
    void AsyncException()
    {
        var caught = false;
        var sw = Stopwatch.StartNew();

        Chain.Chain.From(this)
            .ChainValue(x => x._int)
            .Immediate()
            .ReactAsync(async x =>
            {
                await Task.Yield();
                throw new Exception();
            })
            .CatchAsync(x => caught = true)
            .Start(this);

        _int.Value++;
        Thread.Sleep(100);

        Assert.True(caught);
    }
}