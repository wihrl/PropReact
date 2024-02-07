using PropReact.Chain;
using PropReact.Props;
using PropReact.Tests.Data;

namespace PropReact.Tests;

public class ValidationTests
{
    private ValueData Data { get; } = new();

    [Fact]
    private void NonlocalRoot()
    {
        Assert.Throws<ArgumentException>(() => Watch.From(Data));
        Assert.Throws<ArgumentException>(() => Watch.From("asdf"));
    }

    [Fact]
    private void ChainedExpression()
    {
        var builder = Watch.From(this);
        Assert.Throws<ArgumentException>(() => builder.ChainValue(x => x.Data.Int));
    }
}