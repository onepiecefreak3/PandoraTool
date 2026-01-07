using System.Text;
using Logic.Domain.CodeAnalysis.Contract;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.CodeAnalysis.Contract.Exceptions;
using Logic.Domain.CodeAnalysis.DataClasses.Pandora;

namespace Logic.Domain.CodeAnalysis.Pandora;

internal class PandoraScriptLexer : ILexer<PandoraSyntaxToken>
{
    private readonly StringBuilder _sb;
    private readonly IBuffer<int> _buffer;

    public bool IsEndOfInput => _buffer.IsEndOfInput;

    private int Line { get; set; } = 1;
    private int Column { get; set; } = 1;
    private int Position { get; set; }

    public PandoraScriptLexer(IBuffer<int> buffer)
    {
        _sb = new StringBuilder();
        _buffer = buffer;
    }

    public PandoraSyntaxToken Read()
    {
        if (!TryPeekChar(out char character))
            return new PandoraSyntaxToken(SyntaxTokenKind.EndOfFile, Position, Line, Column);

        switch (character)
        {
            case '.':
                return new PandoraSyntaxToken(SyntaxTokenKind.Dot, Position, Line, Column, $"{ReadChar()}");
            case ',':
                return new PandoraSyntaxToken(SyntaxTokenKind.Comma, Position, Line, Column, $"{ReadChar()}");
            case ':':
                return new PandoraSyntaxToken(SyntaxTokenKind.Colon, Position, Line, Column, $"{ReadChar()}");
            case ';':
                return new PandoraSyntaxToken(SyntaxTokenKind.Semicolon, Position, Line, Column, $"{ReadChar()}");
            case '*':
                return new PandoraSyntaxToken(SyntaxTokenKind.Asterisk, Position, Line, Column, $"{ReadChar()}");
            case '%':
                return new PandoraSyntaxToken(SyntaxTokenKind.Percent, Position, Line, Column, $"{ReadChar()}");
            case '+':
                return new PandoraSyntaxToken(SyntaxTokenKind.Plus, Position, Line, Column, $"{ReadChar()}");
            case '&':
                return new PandoraSyntaxToken(SyntaxTokenKind.Ampersand, Position, Line, Column, $"{ReadChar()}");
            case '|':
                return new PandoraSyntaxToken(SyntaxTokenKind.Pipe, Position, Line, Column, $"{ReadChar()}");
            case '^':
                return new PandoraSyntaxToken(SyntaxTokenKind.Caret, Position, Line, Column, $"{ReadChar()}");

            case '<':
                if (IsPeekedChar(1, '='))
                    return new PandoraSyntaxToken(SyntaxTokenKind.SmallerEquals, Position, Line, Column, $"{ReadChar()}{ReadChar()}");

                if (IsPeekedChar(1, '<'))
                    return new PandoraSyntaxToken(SyntaxTokenKind.ShiftLeft, Position, Line, Column, $"{ReadChar()}{ReadChar()}");

                return new PandoraSyntaxToken(SyntaxTokenKind.SmallerThan, Position, Line, Column, $"{ReadChar()}");

            case '>':
                if (IsPeekedChar(1, '='))
                    return new PandoraSyntaxToken(SyntaxTokenKind.GreaterEquals, Position, Line, Column, $"{ReadChar()}{ReadChar()}");

                if (IsPeekedChar(1, '>'))
                    return new PandoraSyntaxToken(SyntaxTokenKind.ShiftRight, Position, Line, Column, $"{ReadChar()}{ReadChar()}");

                return new PandoraSyntaxToken(SyntaxTokenKind.GreaterThan, Position, Line, Column, $"{ReadChar()}");

            case '!':
                if (!IsPeekedChar(1, '='))
                    break;

                return new PandoraSyntaxToken(SyntaxTokenKind.NotEquals, Position, Line, Column, $"{ReadChar()}{ReadChar()}");

            case '=':
                if (!IsPeekedChar(1, '='))
                    break;

                return new PandoraSyntaxToken(SyntaxTokenKind.EqualsEquals, Position, Line, Column, $"{ReadChar()}{ReadChar()}");

            case '-':
                if (TryPeekChar(1, out char newChar) && newChar is >= '0' and <= '9')
                    goto case '0';

                return new PandoraSyntaxToken(SyntaxTokenKind.Minus, Position, Line, Column, $"{ReadChar()}");

            case '/':
                if (IsPeekedChar(1, '/') || IsPeekedChar(1, '*'))
                    goto case ' ';

                return new PandoraSyntaxToken(SyntaxTokenKind.Slash, Position, Line, Column, $"{ReadChar()}");

            case '(':
                return new PandoraSyntaxToken(SyntaxTokenKind.ParenOpen, Position, Line, Column, $"{ReadChar()}");
            case ')':
                return new PandoraSyntaxToken(SyntaxTokenKind.ParenClose, Position, Line, Column, $"{ReadChar()}");
            case '[':
                return new PandoraSyntaxToken(SyntaxTokenKind.BracketOpen, Position, Line, Column, $"{ReadChar()}");
            case ']':
                return new PandoraSyntaxToken(SyntaxTokenKind.BracketClose, Position, Line, Column, $"{ReadChar()}");
            case '{':
                return new PandoraSyntaxToken(SyntaxTokenKind.CurlyOpen, Position, Line, Column, $"{ReadChar()}");
            case '}':
                return new PandoraSyntaxToken(SyntaxTokenKind.CurlyClose, Position, Line, Column, $"{ReadChar()}");

            case ' ':
            case '\t':
            case '\r':
            case '\n':
                return ReadTriviaAndComments();

            case '"':
                return ReadStringLiteral();

            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return ReadNumericLiteral();

            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
            case 'h':
            case 'i':
            case 'j':
            case 'k':
            case 'l':
            case 'm':
            case 'n':
            case 'o':
            case 'p':
            case 'q':
            case 'r':
            case 's':
            case 't':
            case 'u':
            case 'v':
            case 'w':
            case 'x':
            case 'y':
            case 'z':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'G':
            case 'H':
            case 'I':
            case 'J':
            case 'K':
            case 'L':
            case 'M':
            case 'N':
            case 'O':
            case 'P':
            case 'Q':
            case 'R':
            case 'S':
            case 'T':
            case 'U':
            case 'V':
            case 'W':
            case 'X':
            case 'Y':
            case 'Z':
            case '_':
            case '@':
                return ReadIdentifierOrKeyword();
        }

        throw CreateException("Invalid character.");
    }

    private PandoraSyntaxToken ReadTriviaAndComments()
    {
        int position = Position;
        int line = Line;
        int column = Column;

        _sb.Clear();

        while (TryPeekChar(out char character))
        {
            switch (character)
            {
                case '/':
                    if (IsPeekedChar(1, '/'))
                    {
                        _sb.Append(ReadChar());
                        _sb.Append(ReadChar());

                        while (!IsPeekedChar('\n'))
                            _sb.Append(ReadChar());

                        continue;
                    }

                    if (IsPeekedChar(1, '*'))
                    {
                        _sb.Append(ReadChar());
                        _sb.Append(ReadChar());

                        while (!IsPeekedChar('*') || !IsPeekedChar(1, '/'))
                            _sb.Append(ReadChar());

                        _sb.Append(ReadChar());
                        _sb.Append(ReadChar());

                        continue;
                    }

                    break;

                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    _sb.Append(ReadChar());
                    continue;
            }

            break;
        }

        return new PandoraSyntaxToken(SyntaxTokenKind.Trivia, position, line, column, _sb.ToString());
    }

    private PandoraSyntaxToken ReadStringLiteral()
    {
        int position = Position;
        int line = Line;
        int column = Column;

        _sb.Clear();

        if (!IsPeekedChar('"'))
            throw CreateException("Invalid string literal start.", "\"");

        _sb.Append(ReadChar());

        while (!IsPeekedChar('"'))
        {
            if (IsPeekedChar('\\'))
                _sb.Append(ReadChar());

            _sb.Append(ReadChar());
        }

        if (_buffer.IsEndOfInput)
            throw CreateException("Invalid string literal end.", "\"");

        _sb.Append(ReadChar());

        if (!IsPeekedChar('h'))
            return new PandoraSyntaxToken(SyntaxTokenKind.StringLiteral, position, line, column, _sb.ToString());

        _sb.Append(ReadChar());
        return new PandoraSyntaxToken(SyntaxTokenKind.JumpLiteral, position, line, column, _sb.ToString());
    }

    private PandoraSyntaxToken ReadNumericLiteral()
    {
        int position = Position;
        int line = Line;
        int column = Column;

        _sb.Clear();

        while (TryPeekChar(out char character))
        {
            switch (character)
            {
                case '-':
                    if (_sb.Length < 1)
                    {
                        _sb.Append(ReadChar());
                        continue;
                    }
                    break;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    _sb.Append(ReadChar());
                    continue;
            }

            break;
        }

        var value = _sb.ToString();

        if (_sb.Length < 1 || value[^1] == '-')
            throw CreateException("Not a valid number.");

        return new PandoraSyntaxToken(SyntaxTokenKind.NumericLiteral, position, line, column, value);
    }

    private PandoraSyntaxToken ReadIdentifierOrKeyword()
    {
        int position = Position;
        int line = Line;
        int column = Column;

        _sb.Clear();

        var firstChar = true;
        while (TryPeekChar(out char character))
        {
            switch (character)
            {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '_':
                    firstChar = false;

                    _sb.Append(ReadChar());
                    continue;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (firstChar)
                        throw CreateException("Invalid identifier starting with numbers.");

                    firstChar = false;

                    _sb.Append(ReadChar());
                    continue;
            }

            if (firstChar)
                throw CreateException("Invalid identifier.");

            break;
        }

        var finalValue = _sb.ToString();
        switch (finalValue)
        {
            case "vars":
                return new PandoraSyntaxToken(SyntaxTokenKind.VarsKeyword, position, line, column, finalValue);

            case "and":
                return new PandoraSyntaxToken(SyntaxTokenKind.AndKeyword, position, line, column, finalValue);

            case "or":
                return new PandoraSyntaxToken(SyntaxTokenKind.OrKeyword, position, line, column, finalValue);

            default:
                return new PandoraSyntaxToken(SyntaxTokenKind.Identifier, position, line, column, finalValue);
        }
    }

    private bool IsPeekedChar(char expected)
    {
        return IsPeekedChar(0, expected);
    }

    private bool IsPeekedChar(int position, char expected)
    {
        return TryPeekChar(position, out char character) && character == expected;
    }

    private bool TryPeekChar(out char character)
    {
        return TryPeekChar(0, out character);
    }

    private bool TryPeekChar(int position, out char character)
    {
        character = default;

        int result = _buffer.Peek(position);
        if (result < 0)
            return false;

        character = (char)result;
        return true;
    }

    private char ReadChar()
    {
        int result = _buffer.Read();
        if (result < 0)
            throw CreateException("Could not read character.");

        if (result == '\n')
        {
            Line++;
            Column = 0;
        }

        if (result == '\t')
            Column += 3;

        Column++;
        Position++;

        return (char)result;
    }

    private Exception CreateException(string message, string? expected = null)
    {
        message = $"{message} (Line {Line}, Column {Column})";

        if (!string.IsNullOrEmpty(expected))
            message = $"{message} (Expected \"{expected}\")";

        throw new LexerException(message, Line, Column);
    }
}