using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.CodeAnalysis.Contract.Pandora;
using Logic.Domain.CodeAnalysis.DataClasses.Pandora;

namespace Logic.Domain.CodeAnalysis.Pandora;

internal class PandoraScriptWhitespaceNormalizer : IPandoraScriptWhitespaceNormalizer
{
    public void NormalizeCodeUnit(CodeUnitSyntax codeUnit)
    {
        var ctx = new WhitespaceNormalizeContext();
        NormalizeCodeUnit(codeUnit, ctx);

        codeUnit.Update();
    }

    private void NormalizeCodeUnit(CodeUnitSyntax codeUnit, WhitespaceNormalizeContext ctx)
    {
        foreach (MethodDeclarationSyntax methodDeclaration in codeUnit.MethodDeclarations)
        {
            ctx.IsFirstElement = codeUnit.MethodDeclarations[0] == methodDeclaration;
            ctx.ShouldLineBreak = codeUnit.MethodDeclarations[^1] != methodDeclaration;
            NormalizeMethodDeclaration(methodDeclaration, ctx);
        }
    }

    private void NormalizeMethodDeclaration(MethodDeclarationSyntax methodDeclaration, WhitespaceNormalizeContext ctx)
    {
        bool shouldLineBreak = ctx.ShouldLineBreak;

        SyntaxToken newIdentifier = methodDeclaration.Identifier.WithNoTrivia();

        methodDeclaration.SetIdentifier(newIdentifier, false);

        ctx.ShouldLineBreak = true;
        NormalizeMethodDeclarationParameters(methodDeclaration.Parameters, ctx);

        ctx.ShouldLineBreak = shouldLineBreak;
        NormalizeMethodDeclarationBody(methodDeclaration.Body, ctx);
    }

    private void NormalizeMethodDeclarationParameters(MethodDeclarationParametersSyntax methodDeclarationParameters, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken newParenOpen = methodDeclarationParameters.ParenOpen.WithLeadingTrivia(null).WithLeadingTrivia(null);
        SyntaxToken newParenClose = methodDeclarationParameters.ParenClose.WithLeadingTrivia(null).WithLeadingTrivia(null);

        if (ctx.ShouldLineBreak)
            newParenClose = newParenClose.WithTrailingTrivia("\r\n");

        methodDeclarationParameters.SetParenOpen(newParenOpen, false);
        NormalizeMethodDeclarationParameterList(methodDeclarationParameters.Parameters, ctx);
        methodDeclarationParameters.SetParenClose(newParenClose, false);
    }

    private void NormalizeMethodDeclarationParameterList(CommaSeparatedSyntaxList<ExpressionSyntax>? list,
        WhitespaceNormalizeContext ctx)
    {
        if (list == null)
            return;

        foreach (ExpressionSyntax value in list.Elements)
        {
            ctx.IsFirstElement = list.Elements[0] == value;
            ctx.ShouldLineBreak = false;

            NormalizeExpression(value, ctx);
        }
    }

    private void NormalizeMethodDeclarationBody(MethodDeclarationBodySyntax methodDeclarationBody, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken newCurlyOpen = methodDeclarationBody.CurlyOpen.WithLeadingTrivia(null).WithTrailingTrivia("\r\n");
        SyntaxToken newCurlyClose = methodDeclarationBody.CurlyClose.WithNoTrivia();

        if (ctx.ShouldLineBreak)
            newCurlyClose = newCurlyClose.WithTrailingTrivia("\r\n\r\n");

        methodDeclarationBody.SetCurlyOpen(newCurlyOpen, false);
        methodDeclarationBody.SetCurlyClose(newCurlyClose, false);

        ctx.Indent++;
        foreach (StatementSyntax expression in methodDeclarationBody.Statements)
        {
            ctx.IsFirstElement = methodDeclarationBody.Statements[0] == expression;
            ctx.ShouldLineBreak = true;
            ctx.ShouldIndent = true;

            NormalizeStatement(expression, ctx);
        }
    }

    private void NormalizeStatement(StatementSyntax statement, WhitespaceNormalizeContext ctx)
    {
        switch (statement)
        {
            case GotoLabelStatementSyntax gotoStatement:
                NormalizeGotoStatement(gotoStatement, ctx);
                break;

            case MethodInvocationStatementSyntax methodInvocationStatement:
                NormalizeMethodInvocationStatement(methodInvocationStatement, ctx);
                break;
        }
    }

    private void NormalizeGotoStatement(GotoLabelStatementSyntax gotoStatement, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken newColon = gotoStatement.Colon.WithNoTrivia();

        if (ctx.ShouldLineBreak)
            newColon = newColon.WithTrailingTrivia("\r\n");

        ctx.Indent--;
        ctx.ShouldLineBreak = false;
        ctx.IsFirstElement = true;
        NormalizeLiteralExpression(gotoStatement.Label, ctx);

        gotoStatement.SetColon(newColon, false);
    }

    private void NormalizeMethodInvocationStatement(MethodInvocationStatementSyntax invocation, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken newSemicolon = invocation.Semicolon.WithNoTrivia();

        if (ctx.ShouldLineBreak)
            newSemicolon = newSemicolon.WithTrailingTrivia("\r\n");

        NormalizeName(invocation.Name, ctx);

        invocation.SetSemicolon(newSemicolon, false);

        ctx.ShouldIndent = false;
        ctx.ShouldLineBreak = false;
        NormalizeMethodInvocationParameters(invocation.Parameters, ctx);
    }

    private void NormalizeMethodInvocationParameters(MethodInvocationParametersSyntax invocationParameters, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken parenOpen = invocationParameters.ParenOpen.WithNoTrivia();
        SyntaxToken parenClose = invocationParameters.ParenClose.WithNoTrivia();

        invocationParameters.SetParenOpen(parenOpen, false);
        invocationParameters.SetParenClose(parenClose, false);

        NormalizeExpressionList(invocationParameters.ParameterList, ctx);
    }

    private void NormalizeExpressionList(CommaSeparatedSyntaxList<ExpressionSyntax>? valueList, WhitespaceNormalizeContext ctx)
    {
        if (valueList == null)
            return;

        foreach (ExpressionSyntax value in valueList.Elements)
            NormalizeExpression(value, ctx);
    }

    private void NormalizeExpression(ExpressionSyntax expression, WhitespaceNormalizeContext ctx)
    {
        switch (expression)
        {
            case ParenthesizedExpressionSyntax parens:
                NormalizeParenthesizedExpression(parens, ctx);
                break;

            case BinaryExpressionSyntax binary:
                NormalizeBinaryExpression(binary, ctx);
                break;

            case LogicalExpressionSyntax logical:
                NormalizeLogicalExpression(logical, ctx);
                break;

            case LiteralExpressionSyntax literalExpression:
                NormalizeLiteralExpression(literalExpression, ctx);
                break;

            case VariableExpressionSyntax variableExpression:
                NormalizeVariableExpression(variableExpression, ctx);
                break;
        }
    }

    private void NormalizeParenthesizedExpression(ParenthesizedExpressionSyntax parens, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken parenOpen = parens.ParenOpen.WithNoTrivia();
        SyntaxToken parenClose = parens.ParenClose.WithNoTrivia();

        parens.SetParenOpen(parenOpen, false);
        parens.SetParenClose(parenClose, false);

        NormalizeExpression(parens.Expression, ctx);
    }

    private void NormalizeBinaryExpression(BinaryExpressionSyntax binary, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken operation = binary.Operation.WithLeadingTrivia(" ").WithTrailingTrivia(" ");

        binary.SetOperation(operation, false);

        NormalizeExpression(binary.Left, ctx);
        NormalizeExpression(binary.Right, ctx);
    }

    private void NormalizeLogicalExpression(LogicalExpressionSyntax logical, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken operation = logical.Operation.WithLeadingTrivia(" ").WithTrailingTrivia(" ");

        logical.SetOperation(operation, false);

        NormalizeExpression(logical.Left, ctx);
        NormalizeExpression(logical.Right, ctx);
    }

    private void NormalizeLiteralExpression(LiteralExpressionSyntax literal, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken literalToken = literal.Literal.WithNoTrivia();

        string? leadingTrivia = null;
        if (ctx is { ShouldIndent: true, Indent: > 0 })
            leadingTrivia = new string('\t', ctx.Indent);

        literalToken = literalToken.WithLeadingTrivia(leadingTrivia);

        if (ctx.ShouldLineBreak)
            literalToken = literalToken.WithTrailingTrivia("\r\n");

        literal.SetLiteral(literalToken, false);
    }

    private void NormalizeVariableExpression(VariableExpressionSyntax variable, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken varsKeyword = variable.Vars.WithNoTrivia();
        SyntaxToken bracketOpen = variable.BracketOpen.WithNoTrivia();
        SyntaxToken bracketClose = variable.BracketClose.WithNoTrivia();

        string? leadingTrivia = null;
        if (ctx is { ShouldIndent: true, Indent: > 0 })
            leadingTrivia = new string('\t', ctx.Indent);

        varsKeyword = varsKeyword.WithLeadingTrivia(leadingTrivia);
        if (ctx.ShouldLineBreak)
            bracketClose = bracketClose.WithTrailingTrivia("\r\n");

        ctx.IsFirstElement = true;
        ctx.ShouldIndent = false;
        ctx.ShouldLineBreak = false;
        NormalizeExpression(variable.Expression, ctx);

        variable.SetVars(varsKeyword, false);
        variable.SetBracketOpen(bracketOpen, false);
        variable.SetBracketClose(bracketClose, false);
    }

    private void NormalizeName(NameSyntax name, WhitespaceNormalizeContext ctx)
    {
        switch (name)
        {
            case SimpleNameSyntax simpleName:
                NormalizeSimpleName(simpleName, ctx);
                break;

            case QualifiedNameSyntax qualifiedName:
                NormalizeQualifiedName(qualifiedName, ctx);
                break;
        }
    }

    private void NormalizeSimpleName(SimpleNameSyntax name, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken identifierToken = name.Identifier.WithNoTrivia();

        string? leadingTrivia = null;
        if (ctx is { ShouldIndent: true, Indent: > 0 })
            leadingTrivia = new string('\t', ctx.Indent);

        identifierToken = identifierToken.WithLeadingTrivia(leadingTrivia);

        name.SetIdentifier(identifierToken, false);
    }

    private void NormalizeQualifiedName(QualifiedNameSyntax name, WhitespaceNormalizeContext ctx)
    {
        SyntaxToken dotToken = name.Dot.WithNoTrivia();

        name.SetDot(dotToken, false);

        NormalizeName(name.Left, ctx);
        NormalizeName(name.Right, ctx);
    }
}