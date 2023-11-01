// using PropReact.Chain;
// using PropReact.Chain.Nodes;
// using PropReact.Props;
// using PropReact.Props.Value;
// using PropReact.Selectors;
//
// namespace PropReact.Tests;
//

public static class Extensions
{
    public static void asdf<T>(this List<IEnumerable<T>> e) {}
}

public class T
{
    [Fact]
    public void NullExtension()
    {
        ICallable i = null!;
        i.Call();
    }

    [Fact]
    public void t1()
    {
        var list = new List<List<string>>();
        list.asdf();
        t2(arg => list);
    }

    public void t2<Tvalue>(Func<T, IEnumerable<Tvalue>> action)
    {
        
    }
    
    public void t2<Tvalue>(Func<T, IEnumerable<IEnumerable<Tvalue>>> action)
    {
        
    }
}

public interface ICallable
{
    
}

public static class CallableExtensions
{
    public static void Call(this ICallable callable) => Console.WriteLine("t");
}

// // todo: leave sourcegen tests for SourceGenerators.Tests
// public partial class BasicTests
// {
//     private readonly IMutable<ValueData> _data;
//
//     public BasicTests()
//     {
//         Prop.Mutable(out _data, new ValueData());
//     }
//
//     [Fact]
//     public void Simple()
//     {
//         int changes = 0;
//         Reaction reaction = () => changes++;
//
//         Prop.Watch(this, x => x.Data);
//
//         var manualRoot = new RootNode<BasicTests>(this, reaction)
//         {
//             new ValueNode<BasicTests, ValueData>(x => x._data, reaction)
//         }.Initialize();
//
//         _data.Value = new();
//         Assert.Equal(1, changes);
//
//         manualRoot.Dispose();
//         _data.Value = new();
//         Assert.Equal(1, changes);
//     }
//
//     [Fact]
//     public void Chained()
//     {
//         int changes = 0;
//         Reaction reaction = () => changes++;
//
//         Prop.Watch(this, x => x.Data.Record.Rating);
//         
//         var manualRoot = new RootNode<BasicTests>(this, reaction)
//         {
//             new ValueNode<BasicTests, ValueData>(_props._data, reaction)
//             {
//                 new ValueNode<ValueData, Record>(ValueData._props._record, reaction)
//                 {
//                     new ValueNode<Record, int>(Record._props._rating, reaction)
//                 }
//             }
//         }.Initialize();
//
//
//         var oldValue = _data.Value;
//         _data.Value = new();
//         Assert.Equal(1, changes);
//
//         oldValue.Record = new();
//         Assert.Equal(1, changes);
//
//         _data.Value.Record.Rating = 12;
//         Assert.Equal(2, changes);
//
//         _data.Value.Record = new();
//         Assert.Equal(3, changes);
//
//         manualRoot.Dispose();
//         _data.Value = new();
//         Assert.Equal(3, changes);
//     }
//
//     // [Fact]
//     // public void NestedProp()
//     // {
//     //     int changes = 0;
//     //
//     //     var disposable = Prop.Watch(Hierarchy, x => x.Nested1.Value.Nested2.Value.Value1, x => changes++);
//     //     Assert.Equal(0, changes);
//     //
//     //     Hierarchy.Id.Value = 2;
//     //     Assert.Equal(0, changes);
//     //
//     //     Hierarchy.Nested1.Value.Nested2.Value.Value1.Value = 2;
//     //     Assert.Equal(1, changes);
//     //
//     //     Hierarchy.Nested1.Value = new();
//     //     Assert.Equal(2, changes);
//     //
//     //     Hierarchy.Nested1.Value.Nested2.Value.Value1.Value = 3;
//     //     Assert.Equal(3, changes);
//     //
//     //     disposable.Dispose();
//     //
//     //     Hierarchy.Nested1.Value.Nested2.Value.Value1.Value = 4;
//     //     Assert.Equal(3, changes);
//     // }
//     //
//     // [Fact]
//     // public void BranchingProp()
//     // {
//     //     int changes = 0;
//     //
//     //     var disposable = Prop.Watch(Hierarchy, x => new { V1 = x.Name.Value, V2 = x.Name.Value }, x => changes++);
//     //
//     //     Hierarchy.Name.Value = "asdf";
//     //     Assert.Equal(1, changes);
//     //
//     //     disposable.Dispose();
//     //
//     //     Hierarchy.Name.Value = "asdf3";
//     //     Assert.Equal(1, changes);
//     // }
//     //
//     // [Fact]
//     // public void MutableNonReactivePropInSelector()
//     // {
//     //     Assert.Throws<ArgumentException>(() =>
//     //     {
//     //         Prop.Watch(Hierarchy, x => x.Nested1NoPropMutable.Value1.Value, x => { });
//     //     });
//     // }
//     //
//     // [Fact]
//     // public void FieldInSelector()
//     // {
//     //     Assert.Throws<ArgumentException>(() =>
//     //     {
//     //         Prop.Watch(Hierarchy, x => x.Nested1Field.Value, x => { });
//     //     });
//     // }
//     //
//     // [Fact]
//     // public void NavigableProp()
//     // {
//     //     Assert.Equal(Hierarchy, Hierarchy.NestedNavigable.Value.NavProp.V);
//     //     
//     //     var newNested = new Nested1();
//     //     Hierarchy.NestedNavigable.Value = newNested;
//     //     
//     //     Assert.Equal(Hierarchy, Hierarchy.NestedNavigable.Value.NavProp.V);
//     // }
// }
//
// // TEST IDEAS:
// // reactive collections multiple identical values sub/unsub
// // loops
// // unsupported expression tree constructs