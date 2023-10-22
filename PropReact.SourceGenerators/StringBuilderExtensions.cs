using System;
using System.Text;

namespace PropReact.SourceGenerators;

public static class StringBuilderExtensions
{
    private static int _currentOffset = 0;

    public static IDisposable Block(this StringBuilder sb, string? text = null, bool condition = true)
    {
        if (!condition)
            return new EmptyDisposable();

        if (text is not null)
            sb.IndentedLine(text);

        sb.IndentedLine("{");
        _currentOffset++;

        return new BlockCloser(sb);
    }

    public static void IndentedLine(this StringBuilder sb, string? text = null)
    {
        for (var i = 0; i < _currentOffset; i++)
            sb.Append("\t");

        sb.AppendLine(text ?? "");
    }

    class BlockCloser : IDisposable
    {
        private readonly StringBuilder _sb;

        public BlockCloser(StringBuilder sb) => _sb = sb;

        public void Dispose()
        {
            _currentOffset--;
            _sb.IndentedLine("}");
        }
    }

    class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}