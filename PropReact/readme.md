# PropReact

An **experimental** reactive programming framework for C# loosely inspired by Vue 3's composition API.

The current release is a proof of concept and is not recommended for production use.

### PropReact.Blazor

A Blazor component library for PropReact.

### Features:

- Change tracking
    - Property chaining: `x.Prop1.Prop2.Prop3`
        - Branching: `x.Prop1.Prop2.Branch(y => y.Prop3, y => y.Prop4)`
    - Collection support
        - Reactive lists: `x.Prop1.List1.Enter()`
            - Nesting: `x.Prop1.List1.Select(y => y.Prop2)`
            - Branching
            - Watching specific index: `x => x.Prop1.SomeList[0].Prop2`
        - Watching maps (dictionaries)
            - All of the above
            - Watch specific keys: `x => x.Prop1.SomeMap["foo"].Prop2`
- Change throttling / debounce
- Computed values
    - Async support
- Reflection free - works fine with AOT / trimming

##### Advantages over INotifyPropertyChanged:

- Generally more powerful
- Better collection support
- Less boxing
- Eventually less boilerplate (see limitations)

##### Current limitations:

- Watch chains must be created manually. The plan is to eventually automate this using source generated interceptors.
- Thread safety is not guaranteed (excluding ComputeAsync())
- No change aggregation
- No bulk updates for collections (all changes are processed per-item)
- No data automatically passed into React(...) and Computed(...), properties must be accessed directly

#### Creating reactive properties:

```csharp
class Foo
{
    // props should be readonly fields - if a prop is reassigned, it will break existing observers
    public readonly Mutable<string> Name = "John";     // implicit conversion from T
    public readonly Mutable<int> Unread = 30;

    // BEWARE of null
    // this will NOT work: public readonly Mutable<string?> Nickname = null;
    public readonly Mutable<string?> Nickname = new(null);

    // initialized in constructor
    public readonly IComputed<string> Greeting;

    // initialized with default value, deffered setup
    // useful for when you cannot initialize in constructor such as in Blazor components
    public readonly Computed<string> GreetingWithDefaultValue = "";
    
    readonly CompositeDisposable _disposables = new(); // you can also inherit from it or implement ICompositeDisposable

    public Foo()
    {
        // accessing values must be done with .Value or a shorthand .v, but implicit conversion is also available 
        var getGreeting = () => $"Hello, {Nickname.Value ?? Name.v}! You have {Unread.v} unread messages.";

        // for the time being, chains must be created manually
        ChainBuilder.From(this)
            .Branch(
                x => x.ChainValue(y => y.Name),
                x => x.Branch(
                    y => y.ChainValue(z => z.Unread),
                    y => y.ChainValue(z => z.Nickname)
                )
            )
            .Immediate() // respond to changes immediately (alternatively, use .Throttled(...))
            .Compute(getGreeting, out Greeting)
            .Compute(getGreeting, GreetingWithDefaultValue)
            .React(() => Console.WriteLine("Something changed!"))
            .Start(_disposables);


        // one day, this will be replaced with:
        // Greeting = Computed(getGreeting);
        // Computed(getGreeting, GreetingWithDefaultValue);
    }
}
```

Creating reactive chains is generally done in 3 steps:

1. **Create a chain using `ChainBuilder.From(this)`**

   ChainBuilder.From() must always be called with `this` as the first argument.
   This is to enforce local initialization and to prevent leaks.
2. **Define dependencies**

   Use `.ChainValue()`, `.ChainConstant()` in combination with `.Branch()` to declare dependencies.
    1. `.ChainValue(x => x.Prop)`: Subscribe to a reactive property. The prop must implement `IValue`.

       Chaining is not allowed. ChainValue(x => x.Prop1.Prop2) should be replaced
       with `.ChainValue(x => x.Prop1).ChainValue(x => x.Prop2).`
    2. `.ChainConstant(x => x.SomeValue1.SomeValue2)`: Chain a constant value. If the value ever changes, the chain will
       break and
       observers will be leaked. Chaining is allowed here.
    3. `.Branch(x => ..., x => ...)`: Split the chain in two. Can be used recursively to subscribe to multiple
       properties.
3. **Select reaction type**

   `.Immediate()` or `.Throttled(...)` to specify how the reaction should be handled.
4. **Define reactions**

    1. `.React(...)`
    2. `.ReactAsync(...)`
    2. `.Compute(...)`
    3. `.ComputeAsync(...)`
5. **Start the chain**

   `.Start()` to start observing. The chain will be stopped when disposed. **It is important to dispose chains when they
   are no longer needed to prevent dangling references.**