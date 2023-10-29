namespace PropReact.Shared;

[AttributeUsage(AttributeTargets.Field)]
public class DontExpose : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class NoSetter : Attribute
{
}