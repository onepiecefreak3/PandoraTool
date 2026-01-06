using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;

namespace Logic.Domain.CodeAnalysis.DataClasses.Pandora;

public struct PandoraSyntaxToken
{
    public SyntaxTokenKind Kind { get; }
    public string Text { get; }

    public int Position { get; }
    public int Line { get; }
    public int Column { get; }

    public PandoraSyntaxToken(SyntaxTokenKind kind, int position, int line, int column, string? text = null)
    {
        Text = text ?? string.Empty;
        Kind = kind;
        Position = position;
        Line = line;
        Column = column;
    }
}