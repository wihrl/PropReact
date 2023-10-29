// namespace PropReact.Tests;
//
// public class CollectionTests
// {
//     private ValuePropData Data { get; } = new();
//
//     [Fact]
//     public void BasicList()
//     {
//         var changes = 0;
//         Prop.Watch(Data, x => x.Items1.Select(y => y.Value1), x => changes++);
//
//         Assert.Equal(0, changes);
//
//         Data.Items1.Add(new());
//         Assert.Equal(1, changes);
//
//         Data.Items1[0].Value1.Value = "asdf";
//         Assert.Equal(2, changes);
//
//         Data.Items1.Add(new());
//         Data.Items1.Clear();
//
//
//         Assert.Equal(5, changes);
//     }
//
//     [Fact]
//     public void NestedList()
//     {
//         var changes = 0;
//         Prop.Watch(Data, x => x.Items1.Select(y => y.Items2.Select(x => x.Value2)), x => changes++);
//
//         Assert.Equal(0, changes);
//
//         Data.Items1.Add(new());
//         Assert.Equal(1, changes);
//
//         Data.Items1[0].Items2.Add(new());
//         Assert.Equal(2, changes);
//         
//         Data.Items1[0].Items2[0].Value2.Value = 2;
//         Assert.Equal(3, changes);
//         
//         // todo: remove, replace, map, same values, ...
//     }
//
//     [Fact]
//     public void MapWatcher()
//     {
//         var guid = Guid.NewGuid();
//         var prop = Data.Items1MapProp.WatchAt(guid);
//         
//         Assert.Null(prop.V);
//         
//         Data.Items1MapProp.Add(new());
//         Assert.Null(prop.V);
//
//         var val = new ValuePropData.Item1() { Id = guid };
//         Data.Items1MapProp.Add(val);
//         Assert.Equal(val, prop.V);
//         
//         Data.Items1MapProp.Remove(val);
//         Assert.Null(prop.V);
//     }
//     
//     // todo: Collection.Length (properties on the collection instead of nested ones)
// }