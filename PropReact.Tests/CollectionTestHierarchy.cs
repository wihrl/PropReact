using PropReact.Collections;
using PropReact.Properties;

namespace PropReact.Tests;

public class CollectionTestHierarchy
{
    public IListProp<Item1> Items1 { get; } = Prop.MakeList<Item1>();
    public IMapProp<Guid, Item1> Items1MapProp { get; } = Prop.MakeMap<Guid, Item1>(x => x.Id);

    public class Item1
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public IProp<string> Value1 { get; } = Prop.Make("");
        public IProp<int> Value2 { get; } = Prop.Make(0);
        public IListProp<Item2> Items2 { get; } = Prop.MakeList<Item2>();

        public class Item2
        {
            public IProp<string> Value1 { get; } = Prop.Make("");
            public IProp<int> Value2 { get; } = Prop.Make(0);
        }
    }
}