using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PropReact.SourceGenerators;

/// <summary>
/// A sample source generator that creates C# classes based on the text file (in this case, Domain Driven Design ubiquitous language registry).
/// When using a simple text file as a baseline, we can create a non-incremental source generator.
/// </summary>
[Generator]
public class WrapperGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
            return;

        foreach (var receiverError in receiver.Errors) context.ReportDiagnostic(receiverError);

        StringBuilder sb = new();
        sb.AppendLine("using PropReact;");
        sb.AppendLine("using PropReact.Props;");
        sb.AppendLine("using PropReact.Props.Collections;");
        sb.AppendLine("using PropReact.Props.Value;");
        sb.AppendLine();

        foreach (var reactiveClass in receiver.Classes.Values)
        {
            using (sb.Block("namespace " + reactiveClass.Namespace, reactiveClass.Namespace is not null))
            {
                using (sb.Block($"partial class {reactiveClass.Name}"))
                {
                    foreach (var field in reactiveClass.Fields)
                    {
                        if (reactiveClass.IsBlazorComponent)
                            sb.IndentedLine("[Microsoft.AspNetCore.Components.Parameter]");

                        using (sb.Block($"public {field.ValueType} {field.WrapperName}"))
                        {
                            sb.IndentedLine($"get => {field.FieldName}.Value;");

                            if (!field.GetterOnly)
                                sb.IndentedLine($"set => {field.FieldName}.Value = value;");
                        }
                        
                        sb.IndentedLine();
                    }

                    using (sb.Block($"public static class _props"))
                    {
                        foreach (var field in reactiveClass.Fields)
                            sb.IndentedLine(
                                $"public static {field.FieldType}<{field.ValueType}> {field.FieldName}({reactiveClass.Name} x) => x.{field.FieldName};");
                    }
                }
            }
        }

        context.AddSource("PropReact.generated.cs", sb.ToString());
    }

    class SyntaxReceiver : ISyntaxContextReceiver
    {
        readonly DiagnosticDescriptor _descriptorInvalidName = new("PRE01", "Invalid name",
            "Invalid name of a backing field", "PropReact", DiagnosticSeverity.Error, true);

        readonly DiagnosticDescriptor _descriptorWritableField = new("PRE02", "Writable backing field",
            "Backing field must be readonly", "PropReact", DiagnosticSeverity.Error, true);

        readonly DiagnosticDescriptor _descriptorPropAsProperty = new("PRE03", "IProp as a property",
            "IProp-derived types can only be used as a field", "PropReact", DiagnosticSeverity.Error, true);


        public Dictionary<string, ReactiveClass> Classes { get; } = new();
        public List<Diagnostic> Errors { get; } = new();


        // nameof ignores generics
        bool IsProp(string typeName, out bool isComputed) =>
            (isComputed = typeName.StartsWith(Magic.Computed)) || typeName.StartsWith(Magic.Mutable);

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is PropertyDeclarationSyntax propertyDeclaration)
            {
                if (IsProp(propertyDeclaration.Type.ToString(), out _))
                    Errors.Add(Diagnostic.Create(_descriptorPropAsProperty, propertyDeclaration.GetLocation()));

                return;
            }

            if (context.Node is not FieldDeclarationSyntax fieldDeclaration)
                return;

            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(variable);

                if (symbol is not IFieldSymbol field)
                    continue;

                // field must be a value prop
                if (!IsProp(field.Type.Name, out var isComputed))
                    continue;

                // field must be readonly
                if (!field.IsReadOnly)
                {
                    Errors.Add(Diagnostic.Create(_descriptorWritableField, fieldDeclaration.GetLocation()));
                    continue;
                }

                // field must be private (public is allowed, but do not generate wrapper)
                if (field.DeclaredAccessibility is not (Accessibility.Private or Accessibility.Protected))
                    continue;

                // field must start with an underscore or lowercase letter
                if (field.Name[0] != '_' || char.IsLower(field.Name[0]))
                    continue;

                var wrapperName = GetWrapperName(field.Name);
                if (wrapperName is null)
                {
                    Errors.Add(Diagnostic.Create(_descriptorInvalidName, fieldDeclaration.GetLocation()));
                    continue;
                }

                var attributes = field.GetAttributes();
                if (attributes.Any(x => x.AttributeClass?.Name == Magic.DontExpose))
                    continue;

                var valueTypeSymbol = (INamedTypeSymbol) field.Type;
                var valueType = valueTypeSymbol.TypeArguments[0]
                    .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                var getterOnly = isComputed || attributes.Any(x => x.AttributeClass?.Name == Magic.GetOnly);
                var className = field.ContainingType!.Name;

                if (!Classes.TryGetValue(className, out var reactiveClass))
                {
                    var classNamespace = field.ContainingType.ContainingNamespace.IsGlobalNamespace
                        ? null
                        : field.ContainingType.ContainingNamespace.ToDisplayString();
                    var classInpc = field.ContainingType.Interfaces.Any(x => x.Name == "INotifyPropertyChanged");
                    var classBlazor = field.ContainingType?.BaseType?.Name == "ComponentBase";

                    reactiveClass = Classes[className] = new(className, classNamespace, classInpc, classBlazor);
                }

                reactiveClass.Fields.Add(new(field.Name, wrapperName, valueType, field.Type.Name, getterOnly));
            }
        }

        string? GetWrapperName(string name)
        {
            var trimmed = name.TrimStart('_');
            if (trimmed.Length == 0)
                return null;

            return char.ToUpper(trimmed[0]) + trimmed.Substring(1);
        }
    }

    class ReactiveClass
    {
        public ReactiveClass(string name, string? ns, bool generatePropertyChanged, bool isBlazorComponent)
        {
            Name = name;
            GeneratePropertyChanged = generatePropertyChanged;
            IsBlazorComponent = isBlazorComponent;
            Namespace = ns;
        }

        public string Name { get; }
        public string? Namespace { get; }
        public bool GeneratePropertyChanged { get; }
        public bool IsBlazorComponent { get; }
        public List<ReactiveField> Fields { get; } = new();
    }

    class ReactiveField
    {
        public ReactiveField(string fieldName, string wrapperName, string valueType, string fieldType, bool getterOnly)
        {
            WrapperName = wrapperName;
            GetterOnly = getterOnly;
            FieldType = fieldType;
            FieldName = fieldName;
            ValueType = valueType;
        }

        public string FieldName { get; }
        public string WrapperName { get; }
        public string ValueType { get; }
        public string FieldType { get; }
        public bool GetterOnly { get; }
    }
}