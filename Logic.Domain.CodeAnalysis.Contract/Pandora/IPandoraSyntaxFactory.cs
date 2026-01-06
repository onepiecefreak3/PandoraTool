using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;

namespace Logic.Domain.CodeAnalysis.Contract.Pandora;

public interface IPandoraSyntaxFactory
{
    SyntaxToken Create(string text, int rawKind, SyntaxTokenTrivia? leadingTrivia = null, SyntaxTokenTrivia? trailingTrivia = null);

    SyntaxToken Token(SyntaxTokenKind kind);

    SyntaxToken StringLiteral(string text);
    SyntaxToken DataLiteral(string text);
    SyntaxToken NumberLiteral(int value);
    SyntaxToken JumpLiteral(string label);
    SyntaxToken Identifier(string text);
}