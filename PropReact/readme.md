performance optimizations:
- use source generators for expression parsing
- make ChainNode/IChangeObserver generic to avoid boxing
- lazy computed property evaluation

todo:
- use expression captured variables to allow for static class access and avoid having to pass this
- autodispose
- Prop.Make().LinkWith(allowOverride);

- remove navigation properties, instead allow watching collection keys in computed properties (x.Parent.SomeCollection[Key])