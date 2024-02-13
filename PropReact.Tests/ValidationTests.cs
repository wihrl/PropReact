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
        Assert.Throws<ArgumentException>(() => Chain.Chain.From(Data));
        Assert.Throws<ArgumentException>(() => Chain.Chain.From("asdf"));
    }

    [Fact]
    private void ChainedExpression()
    {
        var builder = Chain.Chain.From(this);
        Assert.Throws<ArgumentException>(() => builder.ChainValue(x => x.Data.Int));
    }
}