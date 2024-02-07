namespace PropReact.Props;

[Flags]
public enum ThrottleMode
{
    None = 0,
    Extendable = 0b1,
    Immediate = 0b10,
    ImmediateExtendable = Immediate | Extendable
}