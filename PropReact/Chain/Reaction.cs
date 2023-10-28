namespace PropReact.Chain;

public delegate void Reaction();

public record ReactionContext(Reaction BeforeChange, Reaction AfterChange);