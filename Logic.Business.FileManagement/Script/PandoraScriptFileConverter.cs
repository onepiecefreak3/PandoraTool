using Logic.Business.FileManagement.InternalContract.Script;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.CodeAnalysis.Contract.Pandora;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Business.FileManagement.Script;

internal class PandoraScriptFileConverter(IPandoraSyntaxFactory syntaxFactory) : IPandoraScriptFileConverter
{
    public CodeUnitSyntax CreateCodeUnit(ScriptInstruction[] instructions)
    {
        IReadOnlyList<MethodDeclarationSyntax> methods = CreateMethodDeclarations(instructions);

        return new CodeUnitSyntax(methods);
    }

    private IReadOnlyList<MethodDeclarationSyntax> CreateMethodDeclarations(ScriptInstruction[] instructions)
    {
        return [CreateMethodDeclaration(instructions)];
    }

    private MethodDeclarationSyntax CreateMethodDeclaration(ScriptInstruction[] instructions)
    {
        SyntaxToken identifier = syntaxFactory.Identifier("Main");
        var parameters = CreateMethodDeclarationParameters();
        var body = CreateMethodDeclarationBody(instructions);

        return new MethodDeclarationSyntax(identifier, parameters, body);
    }

    private MethodDeclarationParametersSyntax CreateMethodDeclarationParameters()
    {
        SyntaxToken parenOpen = syntaxFactory.Token(SyntaxTokenKind.ParenOpen);
        SyntaxToken parenClose = syntaxFactory.Token(SyntaxTokenKind.ParenClose);

        return new MethodDeclarationParametersSyntax(parenOpen, null, parenClose);
    }

    private MethodDeclarationBodySyntax CreateMethodDeclarationBody(ScriptInstruction[] instructions)
    {
        SyntaxToken curlyOpen = syntaxFactory.Token(SyntaxTokenKind.CurlyOpen);
        var expressions = CreateStatements(instructions);
        SyntaxToken curlyClose = syntaxFactory.Token(SyntaxTokenKind.CurlyClose);

        return new MethodDeclarationBodySyntax(curlyOpen, expressions, curlyClose);
    }

    private IReadOnlyList<StatementSyntax> CreateStatements(ScriptInstruction[] instructions)
    {
        var result = new List<StatementSyntax>();

        foreach (ScriptInstruction instruction in instructions)
        {
            if (instruction.JumpLabel is not null)
                result.Add(CreateGotoLabelStatement(instruction.JumpLabel));

            result.Add(CreateMethodInvocationStatement(instruction));
        }

        return result;
    }

    private GotoLabelStatementSyntax CreateGotoLabelStatement(string label)
    {
        var literal = CreateJumpLiteralExpression(label);
        SyntaxToken colon = syntaxFactory.Token(SyntaxTokenKind.Colon);

        return new GotoLabelStatementSyntax(literal, colon);
    }

    private MethodInvocationStatementSyntax CreateMethodInvocationStatement(ScriptInstruction instruction)
    {
        NameSyntax identifier = CreateMethodNameIdentifier(instruction);

        return CreateMethodInvocationStatement(identifier, instruction);
    }

    private MethodInvocationStatementSyntax CreateMethodInvocationStatement(NameSyntax methodName, ScriptInstruction instruction)
    {
        var parameters = CreateMethodInvocationExpressionParameters(instruction);
        SyntaxToken semicolon = syntaxFactory.Token(SyntaxTokenKind.Semicolon);

        return new MethodInvocationStatementSyntax(methodName, parameters, semicolon);
    }

    private NameSyntax CreateMethodNameIdentifier(ScriptInstruction instruction)
    {
        return CreateName($"sub{instruction.Instruction}");
    }

    private MethodInvocationParametersSyntax CreateMethodInvocationExpressionParameters(ScriptInstruction instruction)
    {
        SyntaxToken parenOpen = syntaxFactory.Token(SyntaxTokenKind.ParenOpen);
        var parameterList = CreateParameters(instruction);
        SyntaxToken parenClose = syntaxFactory.Token(SyntaxTokenKind.ParenClose);

        return new MethodInvocationParametersSyntax(parenOpen, parameterList, parenClose);
    }

    private CommaSeparatedSyntaxList<ExpressionSyntax>? CreateParameters(ScriptInstruction instruction)
    {
        if (instruction.Arguments.Length <= 0)
            return null;

        var result = new List<ExpressionSyntax>();

        foreach (ScriptArgument argument in instruction.Arguments)
        {
            result.Add(CreateValueExpression(argument));
        }

        return new CommaSeparatedSyntaxList<ExpressionSyntax>(result);
    }

    private ExpressionSyntax CreateValueExpression(ScriptArgument argument)
    {
        switch (argument)
        {
            case ScriptArgumentBytes data:
                return CreateDataLiteralExpression(data.Data);

            case ScriptArgumentString text:
                return CreateStringLiteralExpression(text.Text);

            case ScriptArgumentInt number:
                return CreateNumberLiteralExpression(number.Value);

            case ScriptArgumentJump jump:
                return CreateJumpLiteralExpression(jump.Label);

            case ScriptArgumentVariable variable:
                return CreateVariableExpression(variable.Value);

            default:
                throw new InvalidOperationException("Unknown argument.");
        }
    }

    private VariableExpressionSyntax CreateVariableExpression(int variable)
    {
        var variableExpression = CreateNumberLiteralExpression(variable);

        return CreateVariableExpression(variableExpression);
    }

    private VariableExpressionSyntax CreateVariableExpression(ExpressionSyntax valueExpression)
    {
        var varsKeyword = syntaxFactory.Token(SyntaxTokenKind.VarsKeyword);
        var bracketOpen = syntaxFactory.Token(SyntaxTokenKind.BracketOpen);
        var bracketClose = syntaxFactory.Token(SyntaxTokenKind.BracketClose);

        return new VariableExpressionSyntax(varsKeyword, bracketOpen, valueExpression, bracketClose);
    }

    private LiteralExpressionSyntax CreateStringLiteralExpression(string value)
    {
        return new LiteralExpressionSyntax(syntaxFactory.StringLiteral(value.Replace(@"\", @"\\")));
    }

    private LiteralExpressionSyntax CreateDataLiteralExpression(byte[] value)
    {
        return new LiteralExpressionSyntax(syntaxFactory.DataLiteral(Convert.ToBase64String(value)));
    }

    private LiteralExpressionSyntax CreateNumberLiteralExpression(int value)
    {
        return new LiteralExpressionSyntax(syntaxFactory.NumberLiteral(value));
    }

    private LiteralExpressionSyntax CreateJumpLiteralExpression(string value)
    {
        return new LiteralExpressionSyntax(syntaxFactory.JumpLiteral(value.Replace(@"\", @"\\")));
    }

    private NameSyntax CreateName(string name)
    {
        if (name.Contains('.'))
            return new SimpleNameSyntax(syntaxFactory.Identifier(name));

        NameSyntax? result = null;

        foreach (string part in name.Split('.').Reverse())
        {
            if (result is null)
                result = new SimpleNameSyntax(syntaxFactory.Identifier(part));
            else
                result = new QualifiedNameSyntax(new SimpleNameSyntax(syntaxFactory.Identifier(part)), syntaxFactory.Token(SyntaxTokenKind.Dot), result);
        }

        return result!;
    }
}