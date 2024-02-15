// See https://aka.ms/new-console-template for more information

using PropReact.Chain;
using PropReact.Props.Collections;
using PropReact.Props.Value;
using PropReact.Utils;

var foo = new Foo();

foo.Name.Value = "Feature";
foo.Messages.Add(new() { Text = "1" });
foo.Messages.Add(new() { Text = "2" });
foo.Messages[0].Read.v = true;
foo.Nickname.v = "Bug";

Console.WriteLine(foo.Greeting.v);

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