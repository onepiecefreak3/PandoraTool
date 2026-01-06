using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.CodeAnalysis.Contract.Pandora;

namespace Logic.Domain.CodeAnalysis.Pandora;

internal class PandoraSyntaxFactory : IPandoraSyntaxFactory
{
    public SyntaxToken Create(string text, int rawKind, SyntaxTokenTrivia? leadingTrivia = null, SyntaxTokenTrivia? trailingTrivia = null)
    {
        return new(text, rawKind, leadingTrivia, trailingTrivia);
    }

    public SyntaxToken Token(SyntaxTokenKind kind)
    {
        switch (kind)
        {
            case SyntaxTokenKind.Dot: return new(".", (int)kind);
            case SyntaxTokenKind.Comma: return new(",", (int)kind);
            case SyntaxTokenKind.Colon: return new(":", (int)kind);
            case SyntaxTokenKind.Semicolon: return new(";", (int)kind);

            case SyntaxTokenKind.ParenOpen: return new("(", (int)kind);
            case SyntaxTokenKind.ParenClose: return new(")", (int)kind);
            case SyntaxTokenKind.BracketOpen: return new("[", (int)kind);
            case SyntaxTokenKind.BracketClose: return new("]", (int)kind);
            case SyntaxTokenKind.CurlyOpen: return new("{", (int)kind);
            case SyntaxTokenKind.CurlyClose: return new("}", (int)kind);

            case SyntaxTokenKind.VarsKeyword: return new("vars", (int)kind);

            default: throw new InvalidOperationException($"Cannot create simple token from kind {kind}. Use other methods instead.");
        }
    }

    public SyntaxToken StringLiteral(string text)
    {
        return new($"\"{text.Replace("\"", "\\\"")}\"", (int)SyntaxTokenKind.StringLiteral);
    }

    public SyntaxToken DataLiteral(string text)
    {
        return new($"'{text.Replace("'", "\\'")}'", (int)SyntaxTokenKind.StringLiteral);
    }

    public SyntaxToken NumberLiteral(int value)
    {
        return new($"{value}", (int)SyntaxTokenKind.NumericLiteral);
    }

    public SyntaxToken JumpLiteral(string label)
    {
        return new($"\"{label.Replace("\"", "\\\"")}\"h", (int)SyntaxTokenKind.JumpLiteral);
    }

    public SyntaxToken Identifier(string text)
    {
        return new(text, (int)SyntaxTokenKind.Identifier);
    }
}