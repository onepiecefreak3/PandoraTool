namespace Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;

public enum SyntaxTokenKind
{
    Dot,
    Comma,
    Colon,
    Semicolon,

    ParenOpen,
    ParenClose,
    BracketOpen,
    BracketClose,
    CurlyOpen,
    CurlyClose,

    Trivia,

    StringLiteral,
    DataLiteral,
    NumericLiteral,
    JumpLiteral,

    Identifier,
    VarsKeyword,

    EndOfFile
}