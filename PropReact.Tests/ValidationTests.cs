﻿using PropReact.Chain;
using PropReact.Props;
using PropReact.Tests.Data;

namespace PropReact.Tests;

public class ValidationTests
{
    private ValueData Data { get; } = new();

    [Fact]
    private void NonlocalRoot()
    {
        Assert.Throws<ArgumentException>(() => Prop.Watch(Data));
        Assert.Throws<ArgumentException>(() => Prop.Watch("asdf"));
    }

    [Fact]
    private void ChainedExpression()
    {
        var builder = Prop.Watch(this);
        Assert.Throws<ArgumentException>(() => builder.ChainValue(x => x.Data.Int));
    }
}