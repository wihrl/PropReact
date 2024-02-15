# PropReact

An **experimental** reactive programming framework for C# loosely inspired by Vue 3's composition API.\
Can be used standalone or trough a [Blazor integration](#propreactblazor).

The current release is a proof of concept and is not recommended for production use.

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
    - No information about which property along the chain changed

#### Creating reactive properties:

```csharp
class Foo
{
    // props should be readonly fields - if a prop is reassigned, it will break existing observers
    public readonly Mutable<string> Name = "John"; // implicit conversion from T

    // BEWARE of null
    // this will NOT work: public readonly Mutable<string?> Nickname = null;
    public readonly Mutable<string?> Nickname = new(null);

    // initialized in constructor
    public readonly IComputed<string> Greeting;

    // initialized with default value, deffered setup
    // useful for when you cannot initialize in constructor such as in Blazor components
    public readonly Computed<string> GreetingWithDefaultValue = "";

    // reactive collections must also be readonly so they can be chained with .ChainConstant(...)
    public readonly ReactiveList<Message> Messages = new();

    readonly CompositeDisposable _disposables = new(); // you can also inherit from it or implement ICompositeDisposable

    public Foo()
    {
        // accessing values must be done with .Value or a shorthand .v, but implicit conversion is also available 
        var getGreeting = () => $"Hello, {Nickname.Value ?? Name.v}! You have {Messages.Count(x => !x.Read.v)} unread messages.";

        // for the time being, chains must be created manually
        ChainBuilder.From(this)
            .Branch(
                x => x.ChainConstant(y => y.Messages).Enter().ChainValue(y => y.Read),
                x => x.Branch(
                    y => y.ChainValue(z => z.Name),
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

class Message
{
    public required string Text { get; init; }
    public readonly Mutable<bool> Read = false;
}
```

Creating reactive chains is generally done in 3 steps:

1. **Create a chain** using `ChainBuilder.From(this)`\
   ChainBuilder.From() must always be called with `this` as the first argument.
   This is to enforce local initialization and to prevent leaks.
2. **Define dependencies** using a combination of the following methods:
    - `.ChainValue(x => x.Prop)`: Subscribe to `IValue`. This will generally be `IMutable<T>` or `IComputed<T>`.\
      *Chaining is not allowed.* `.ChainValue(x => x.Prop1.Prop2)` should be replaced
      with `.ChainValue(x => x.Prop1).ChainValue(x => x.Prop2).`

    - `.ChainConstant(x => x.SomeValue1.SomeValue2)`: Chain a constant value, also used for collections.\
      If the value ever changes, the chain will break and observers will be leaked. Chaining is allowed here.
    - `Enter()`: Enter any IEnumerable\<T\>. If the type implements `IProp`, updates will also trigger on changes.
    - `EnterAt(TKey)`: Enter at a specific key. Collection must implement `IReactiveCollection`. Both `ReactiveList`
      and `ReactiveMap` support this.
    - `.Branch(x => ..., x => ...)`: Split the chain in two. Can be used recursively to subscribe to multiple
      properties.

3. **Select reaction type**\
   `.Immediate()` or `.Throttled(...)` to specify how the reaction should be handled.
4. **Define reactions**
    - `.React(...)` - execute on change
    - `.ReactAsync(...)` - execute on change, async with a CancellationToken\
      *Keep in mind PropReact is not thread safe*
    - `.Compute(...)` - create a computed value that updates when dependencies change
    - `.ComputeAsync(...)` - same as above, but async
5. **Start the chain**\
   Call `.Start()` to start observing. The chain will be stopped when disposed.\
   **It is important to dispose chains when they are no longer needed to prevent dangling references.**

| Type                | Description                                         |
|---------------------|-----------------------------------------------------|
| `Mutable<T>`        | Mutable reactive property                           |
| `Computed<T>`       | Read-only reactive property                         |
| `ReactiveList<T>`   | Reactive version of `List<T>`                       |
| `ReactiveMap<T, K>` | Reactive `Dictionary<T, K>` but with a key selector |

For more complex examples, including async and collection support, see tests or
the [Blazor example](https://github.com/wihrl/PropReact/tree/main/samples/BlazorSample).

### PropReact.Blazor

#### ReactiveComponent

All components using PropReact should inherit from `ReactiveComponent`.\
This class provides `Watch()` and `Bind()` methods which can be used within the render tree to observe or bind props.\
It also manages a `CompositeDisposable` instance for creating observation chains.

```csharp
@if(string.IsNullOrEmpty(Watch(_text)))
{
    <p>Please enter some text</p>   
}
<input @bind="Bind(_text).v"/>
```

##### ExclusiveReactiveComponent

Same as `ReactiveComponent`, but re-renders *only* if dependencies change.\
This can prevent unnecessary re-renders due to re-renders of the parent component.

#### Parameters

`IValue` derived props cannot be used directly as blazor parameters since PropReact has no way of knowing
when they are changed and thus cannot unsubscribe from them. Instead, use a setter only parameter property in combination with `Value/MutableParam`.

```csharp
    [Parameter]
    public IMutable<int> Count { set => _count.Update(value); }
    readonly MutableParam<int> _count = new();
    
    // ... .ChainValue(x => x._count)
```