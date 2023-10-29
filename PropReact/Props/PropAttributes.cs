namespace PropReact.Props;

[AttributeUsage(AttributeTargets.Field)]
public class DontExpose : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class GetOnly : Attribute
{
}