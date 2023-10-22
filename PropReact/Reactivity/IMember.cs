using System.Reflection;

namespace PropReact.Reactivity;

internal interface IMember
{
    public MemberInfo MemberInfo { get; }
    public Type MemberType { get; }
    public Type DeclaringType { get; }
    public bool IsReadOnly { get; }
    public object? GetValue(object? value);
}

public record PropertyMember(PropertyInfo PropertyInfo) : IMember
{
    public MemberInfo MemberInfo => PropertyInfo;
    public Type MemberType => PropertyInfo.PropertyType;
    public Type DeclaringType => PropertyInfo.DeclaringType!;
    public bool IsReadOnly => !PropertyInfo.CanWrite;
    public object? GetValue(object? value) => PropertyInfo.GetValue(value);
}

public record FieldMember(FieldInfo FieldInfo) : IMember
{
    public MemberInfo MemberInfo => FieldInfo;
    public Type MemberType => FieldInfo.FieldType;
    public Type DeclaringType => FieldInfo.DeclaringType!;
    public bool IsReadOnly => FieldInfo.IsInitOnly;
    public object? GetValue(object? value) => FieldInfo.GetValue(value);
}