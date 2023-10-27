### features:

- watching simple property chains: `x => x.Prop1.Prop2.Prop3`
    - branching: `x => x.Prop1.Prop2.Branch(y => y.Prop3a.Prop4, y => y.Prop3a)`
- watching sets: `x => x.Prop1.SomeSet.Select(y => y.Prop3)`
    - nesting:
    - branching: `x => x.Branch(y => y.Prop1a.SomeSet.Select(y => y.Prop3), y => y.Prop1b)`
- watching maps
    - same as sets
    - watch specific keys: `x => x.Prop1.SomeMap["foo"].Prop2`
- very little boilerplate
- reflection free - works great with AOT / trimming

performance optimizations:

- use source generators for expression parsing
- make ChainNode/IChangeObserver generic to avoid boxing
- lazy computed property evaluation

todo:

- use expression captured variables to allow for static class access and avoid having to pass this
- autodispose
- Prop.Make().LinkWith(allowOverride);

- remove navigation properties, instead allow watching collection keys in computed properties (
  x.Parent.SomeCollection[Key])