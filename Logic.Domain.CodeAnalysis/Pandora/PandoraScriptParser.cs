using Logic.Domain.CodeAnalysis.Contract;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.CodeAnalysis.Contract.Exceptions.Pandora;
using Logic.Domain.CodeAnalysis.Contract.Pandora;
using Logic.Domain.CodeAnalysis.DataClasses.Pandora;

namespace Logic.Domain.CodeAnalysis.Pandora;

internal class PandoraScriptParser : IPandoraScriptParser
{
    private readonly ITokenFactory<PandoraSyntaxToken> _scriptFactory;
    private readonly IPandoraSyntaxFactory _syntaxFactory;

    public PandoraScriptParser(ITokenFactory<PandoraSyntaxToken> scriptFactory, IPandoraSyntaxFactory syntaxFactory)
    {
        _scriptFactory = scriptFactory;
        _syntaxFactory = syntaxFactory;
    }

    public CodeUnitSyntax ParseCodeUnit(string text)
    {
        IBuffer<PandoraSyntaxToken> buffer = CreateTokenBuffer(text);

        return ParseCodeUnit(buffer);
    }

    private CodeUnitSyntax ParseCodeUnit(IBuffer<PandoraSyntaxToken> buffer)
    {
        var methodDeclarations = ParseMethodDeclarations(buffer);

        return new CodeUnitSyntax(methodDeclarations);
    }

    private IReadOnlyList<MethodDeclarationSyntax> ParseMethodDeclarations(IBuffer<PandoraSyntaxToken> buffer)
    {
        var result = new List<MethodDeclarationSyntax>();

        while (buffer.Peek().Kind != SyntaxTokenKind.EndOfFile)
            result.Add(ParseMethodDeclaration(buffer));

        return result;
    }

    private MethodDeclarationSyntax ParseMethodDeclaration(IBuffer<PandoraSyntaxToken> buffer)
    {
        SyntaxToken identifier = ParseIdentifierToken(buffer);
        var parameters = ParseMethodDeclarationParameters(buffer);
        var body = ParseMethodDeclarationBody(buffer);

        return new MethodDeclarationSyntax(identifier, parameters, body);
    }

    private MethodDeclarationParametersSyntax ParseMethodDeclarationParameters(IBuffer<PandoraSyntaxToken> buffer)
    {
        SyntaxToken parenOpenToken = ParseParenOpenToken(buffer);
        SyntaxToken parenCloseToken = ParseParenCloseToken(buffer);

        return new MethodDeclarationParametersSyntax(parenOpenToken, null, parenCloseToken);
    }

    private MethodDeclarationBodySyntax ParseMethodDeclarationBody(IBuffer<PandoraSyntaxToken> buffer)
    {
        SyntaxToken curlyOpenToken = ParseCurlyOpenToken(buffer);
        var expressions = ParseStatements(buffer);
        SyntaxToken curlyCloseToken = ParseCurlyCloseToken(buffer);

        return new MethodDeclarationBodySyntax(curlyOpenToken, expressions, curlyCloseToken);
    }

    private IReadOnlyList<StatementSyntax> ParseStatements(IBuffer<PandoraSyntaxToken> buffer)
    {
        var result = new List<StatementSyntax>();

        while (IsStatement(buffer))
            result.Add(ParseStatement(buffer));

        return result;
    }

    private bool IsStatement(IBuffer<PandoraSyntaxToken> buffer)
    {
        return IsMethodInvocation(buffer) ||
               IsGotoLabel(buffer);
    }

    private bool IsMethodInvocation(IBuffer<PandoraSyntaxToken> buffer)
    {
        return HasTokenKind(buffer, SyntaxTokenKind.Identifier) &&
               HasTokenKind(buffer, 1, SyntaxTokenKind.ParenOpen);
    }

    private bool IsGotoLabel(IBuffer<PandoraSyntaxToken> buffer)
    {
        return HasTokenKind(buffer, SyntaxTokenKind.JumpLiteral);
    }

    private StatementSyntax ParseStatement(IBuffer<PandoraSyntaxToken> buffer)
    {
        if (IsMethodInvocation(buffer))
            return ParseMethodInvocationStatement(buffer);

        if (IsGotoLabel(buffer))
            return ParseGotoLabelStatement(buffer);

        throw CreateException(buffer, "Unknown statement.", SyntaxTokenKind.Identifier);
    }

    private GotoLabelStatementSyntax ParseGotoLabelStatement(IBuffer<PandoraSyntaxToken> buffer)
    {
        var label = ParseJumpLiteralExpression(buffer);
        SyntaxToken colon = ParseColonToken(buffer);

        return new GotoLabelStatementSyntax(label, colon);
    }

    private MethodInvocationStatementSyntax ParseMethodInvocationStatement(IBuffer<PandoraSyntaxToken> buffer)
    {
        NameSyntax name = ParseName(buffer);
        var methodInvocationParameters = ParseMethodInvocationParameters(buffer);
        SyntaxToken semicolon = ParseSemicolonToken(buffer);

        return new MethodInvocationStatementSyntax(name, methodInvocationParameters, semicolon);
    }

    private MethodInvocationParametersSyntax ParseMethodInvocationParameters(IBuffer<PandoraSyntaxToken> buffer)
    {
        SyntaxToken parenOpen = ParseParenOpenToken(buffer);
        var parameters = ParseParameters(buffer);
        SyntaxToken parenClose = ParseParenCloseToken(buffer);

        return new MethodInvocationParametersSyntax(parenOpen, parameters, parenClose);
    }

    private CommaSeparatedSyntaxList<ExpressionSyntax>? ParseParameters(IBuffer<PandoraSyntaxToken> buffer)
    {
        if (!IsLiteralExpression(buffer) && !IsVariableExpression(buffer))
            return null;

        var result = new List<ExpressionSyntax>();

        ExpressionSyntax parameter = ParseExpression(buffer);
        result.Add(parameter);

        while (HasTokenKind(buffer, SyntaxTokenKind.Comma))
        {
            SkipTokenKind(buffer, SyntaxTokenKind.Comma);

            parameter = ParseExpression(buffer);

            result.Add(parameter);
        }

        return new CommaSeparatedSyntaxList<ExpressionSyntax>(result);
    }

    private ExpressionSyntax ParseExpression(IBuffer<PandoraSyntaxToken> buffer)
    {
        if (IsLiteralExpression(buffer))
            return ParseLiteralExpression(buffer);

        if (IsVariableExpression(buffer))
            return ParseVariableExpression(buffer);

        throw CreateException(buffer, "Invalid expression.", SyntaxTokenKind.StringLiteral, SyntaxTokenKind.DataLiteral, SyntaxTokenKind.NumericLiteral,
            SyntaxTokenKind.JumpLiteral, SyntaxTokenKind.VarsKeyword);
    }

    private bool IsLiteralExpression(IBuffer<PandoraSyntaxToken> buffer)
    {
        return HasTokenKind(buffer, SyntaxTokenKind.StringLiteral) ||
               HasTokenKind(buffer, SyntaxTokenKind.DataLiteral) ||
               HasTokenKind(buffer, SyntaxTokenKind.JumpLiteral) ||
               HasTokenKind(buffer, SyntaxTokenKind.NumericLiteral);
    }

    private bool IsVariableExpression(IBuffer<PandoraSyntaxToken> buffer)
    {
        return HasTokenKind(buffer, SyntaxTokenKind.VarsKeyword);
    }

    private LiteralExpressionSyntax ParseLiteralExpression(IBuffer<PandoraSyntaxToken> buffer)
    {
        if (HasTokenKind(buffer, SyntaxTokenKind.StringLiteral))
            return ParseStringLiteralExpression(buffer);

        if (HasTokenKind(buffer, SyntaxTokenKind.DataLiteral))
            return ParseDataLiteralExpression(buffer);

        if (HasTokenKind(buffer, SyntaxTokenKind.JumpLiteral))
            return ParseJumpLiteralExpression(buffer);

        if (HasTokenKind(buffer, SyntaxTokenKind.NumericLiteral))
            return ParseNumericLiteralExpression(buffer);

        throw CreateException(buffer, "Unknown literal expression.", SyntaxTokenKind.StringLiteral, SyntaxTokenKind.DataLiteral);
    }

    private VariableExpressionSyntax ParseVariableExpression(IBuffer<PandoraSyntaxToken> buffer)
    {
        SyntaxToken varsKeyword = ParseVarsKeywordToken(buffer);
        SyntaxToken bracketOpen = ParseBracketOpenToken(buffer);
        ExpressionSyntax expression = ParseExpression(buffer);
        SyntaxToken bracketClose = ParseBracketCloseToken(buffer);

        return new VariableExpressionSyntax(varsKeyword, bracketOpen, expression, bracketClose);
    }

    private LiteralExpressionSyntax ParseStringLiteralExpression(IBuffer<PandoraSyntaxToken> buffer)
    {
        SyntaxToken literal = ParseStringLiteralToken(buffer);

        return new LiteralExpressionSyntax(literal);
    }

    private LiteralExpressionSyntax ParseDataLiteralExpression(IBuffer<PandoraSyntaxToken> buffer)
    {
        SyntaxToken literal = ParseDataLiteralToken(buffer);

        return new LiteralExpressionSyntax(literal);
    }

    private LiteralExpressionSyntax ParseJumpLiteralExpression(IBuffer<PandoraSyntaxToken> buffer)
    {
        SyntaxToken literal = ParseJumpLiteralToken(buffer);

        return new LiteralExpressionSyntax(literal);
    }

    private LiteralExpressionSyntax ParseNumericLiteralExpression(IBuffer<PandoraSyntaxToken> buffer)
    {
        SyntaxToken literal = ParseNumericLiteralToken(buffer);

        return new LiteralExpressionSyntax(literal);
    }

    private NameSyntax ParseName(IBuffer<PandoraSyntaxToken> buffer)
    {
        if (!HasTokenKind(buffer, SyntaxTokenKind.Identifier))
            throw CreateException(buffer, "Invalid name syntax.", SyntaxTokenKind.Identifier);

        NameSyntax left = new SimpleNameSyntax(ParseIdentifierToken(buffer));
        if (!HasTokenKind(buffer, SyntaxTokenKind.Dot))
            return left;

        SyntaxToken dot = ParseDotToken(buffer);

        return new QualifiedNameSyntax(left, dot, ParseName(buffer));
    }

    private SyntaxToken ParseDotToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.Dot);
    }

    private SyntaxToken ParseColonToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.Colon);
    }

    private SyntaxToken ParseSemicolonToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.Semicolon);
    }

    private SyntaxToken ParseParenOpenToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.ParenOpen);
    }

    private SyntaxToken ParseParenCloseToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.ParenClose);
    }

    private SyntaxToken ParseBracketOpenToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.BracketOpen);
    }

    private SyntaxToken ParseBracketCloseToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.BracketClose);
    }

    private SyntaxToken ParseCurlyOpenToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.CurlyOpen);
    }

    private SyntaxToken ParseCurlyCloseToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.CurlyClose);
    }

    private SyntaxToken ParseStringLiteralToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.StringLiteral);
    }

    private SyntaxToken ParseDataLiteralToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.DataLiteral);
    }

    private SyntaxToken ParseJumpLiteralToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.JumpLiteral);
    }

    private SyntaxToken ParseNumericLiteralToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.NumericLiteral);
    }

    private SyntaxToken ParseIdentifierToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.Identifier);
    }

    private SyntaxToken ParseVarsKeywordToken(IBuffer<PandoraSyntaxToken> buffer)
    {
        return CreateToken(buffer, SyntaxTokenKind.VarsKeyword);
    }

    private SyntaxToken CreateToken(IBuffer<PandoraSyntaxToken> buffer, SyntaxTokenKind expectedKind)
    {
        SyntaxTokenTrivia? leadingTrivia = ReadTrivia(buffer);

        if (buffer.Peek().Kind != expectedKind)
            throw CreateException(buffer, $"Unexpected token {buffer.Peek().Kind}.", expectedKind);
        PandoraSyntaxToken content = buffer.Read();

        SyntaxTokenTrivia? trailingTrivia = ReadTrivia(buffer);

        return _syntaxFactory.Create(content.Text, (int)expectedKind, leadingTrivia, trailingTrivia);
    }

    private SyntaxTokenTrivia? ReadTrivia(IBuffer<PandoraSyntaxToken> buffer)
    {
        if (buffer.Peek().Kind == SyntaxTokenKind.Trivia)
        {
            PandoraSyntaxToken token = buffer.Read();
            return new SyntaxTokenTrivia(token.Text);
        }

        return null;
    }

    private void SkipTokenKind(IBuffer<PandoraSyntaxToken> buffer, SyntaxTokenKind expectedKind)
    {
        int toSkip = 1;

        PandoraSyntaxToken peekedToken = buffer.Peek();
        if (peekedToken.Kind == SyntaxTokenKind.Trivia)
        {
            peekedToken = buffer.Peek(1);
            toSkip++;
        }

        if (peekedToken.Kind != expectedKind)
            throw CreateException(buffer, $"Unexpected token {peekedToken.Kind}.", expectedKind);

        for (var i = 0; i < toSkip; i++)
            buffer.Read();
    }

    protected bool HasTokenKind(IBuffer<PandoraSyntaxToken> buffer, SyntaxTokenKind expectedKind)
    {
        return HasTokenKind(buffer, 0, expectedKind);
    }

    protected bool HasTokenKind(IBuffer<PandoraSyntaxToken> buffer, int position, SyntaxTokenKind expectedKind)
    {
        var toPeek = 0;
        PandoraSyntaxToken peekedToken = buffer.Peek(toPeek);

        position = Math.Max(0, position);
        for (var i = 0; i < position + 1; i++)
        {
            peekedToken = buffer.Peek(toPeek++);
            if (peekedToken.Kind == SyntaxTokenKind.Trivia)
                peekedToken = buffer.Peek(toPeek++);
        }

        return peekedToken.Kind == expectedKind;
    }

    private (int, int) GetCurrentLineAndColumn(IBuffer<PandoraSyntaxToken> buffer)
    {
        var toPeek = 0;

        if (buffer.Peek().Kind == SyntaxTokenKind.Trivia)
            toPeek++;

        PandoraSyntaxToken token = buffer.Peek(toPeek);
        return (token.Line, token.Column);
    }

    private IBuffer<PandoraSyntaxToken> CreateTokenBuffer(string text)
    {
        ILexer<PandoraSyntaxToken> lexer = _scriptFactory.CreateLexer(text);
        return _scriptFactory.CreateTokenBuffer(lexer);
    }

    private Exception CreateException(IBuffer<PandoraSyntaxToken> buffer, string message, params SyntaxTokenKind[] expected)
    {
        (int line, int column) = GetCurrentLineAndColumn(buffer);
        return CreateException(message, line, column, expected);
    }

    private Exception CreateException(string message, int line, int column, params SyntaxTokenKind[] expected)
    {
        message = $"{message} (Line {line}, Column {column})";

        if (expected.Length > 0)
        {
            message = expected.Length == 1 ?
                $"{message} (Expected {expected[0]})" :
                $"{message} (Expected any of {string.Join(", ", expected)})";
        }

        throw new PandoraScriptParserException(message, line, column);
    }
}