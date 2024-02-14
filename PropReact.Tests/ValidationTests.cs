using PropReact.Chain;
using PropReact.Props;
using PropReact.Tests.Shared;

namespace PropReact.Tests;

public class ValidationTests
{
    private ValueData Data { get; } = new();

    [Fact]
    private void NonlocalRoot()
    {
        Assert.Throws<ArgumentException>(() => ChainBuilder.From(Data));
        Assert.Throws<ArgumentException>(() => ChainBuilder.From("asdf"));
    }

    [Fact]
    private void ChainedExpression()
    {
        var builder = ChainBuilder.From(this);
        Assert.Throws<ArgumentException>(() => builder.ChainValue(x => x.Data.Int));
    }
}