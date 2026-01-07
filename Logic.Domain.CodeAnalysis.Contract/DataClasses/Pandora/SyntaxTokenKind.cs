namespace Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;

public enum SyntaxTokenKind
{
    Dot,
    Comma,
    Colon,
    Semicolon,
    Asterisk,
    Slash,
    Percent,
    Plus,
    Minus,
    Ampersand,
    Pipe,
    Caret,
    EqualsEquals,
    NotEquals,
    SmallerThan,
    SmallerEquals,
    GreaterThan,
    GreaterEquals,
    ShiftLeft,
    ShiftRight,

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
    AndKeyword,
    OrKeyword,
    VarsKeyword,

    EndOfFile
}