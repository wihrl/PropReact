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

advantages over INPC:
- completely implemented with generics - no `object` boxing
- better collection support
- more source generator-friendly, hence no reflection

maybe one day:
- arbitrary expression watching (no need for selectors)
- incremental source generators
- more granular transaction locking
- bulk collection changes

performance optimizations:

- use source generators for expression parsing
- make ChainNode/IChangeObserver generic to avoid boxing
- lazy computed property evaluation

todo:

- use expression captured variables to allow for static class access and avoid having to pass this
- autodispose
- Prop.Make().LinkWith(allowOverride);
- blazor component that disallows re-renders unless it was by an observation

- remove navigation properties, instead allow watching collection keys in computed properties (
  x.Parent.SomeCollection[Key])

current limitations:
 - manual reactive chain creation
 - no bulk updates for collections (all changes are processed per-item, albeit without allocations)
 - no type inference for selected expressions
   - updates triggered for or changes along the chain, not just the result