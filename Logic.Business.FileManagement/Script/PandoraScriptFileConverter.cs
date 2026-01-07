using Logic.Business.FileManagement.InternalContract.Script;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.CodeAnalysis.Contract.Pandora;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Enums.Script;

namespace Logic.Business.FileManagement.Script;

internal class PandoraScriptFileConverter(IPandoraSyntaxFactory syntaxFactory) : IPandoraScriptFileConverter
{
    private readonly Dictionary<Operation, int> _operatorPrecedence = new()
    {
        [Operation.Multiply] = 0,
        [Operation.Divide] = 0,
        [Operation.Modulo] = 0,
        [Operation.Add] = 1,
        [Operation.Subtract] = 1,
        [Operation.ShiftLeft] = 2,
        [Operation.ShiftRight] = 2,
        [Operation.GreaterThan] = 3,
        [Operation.GreaterEquals] = 3,
        [Operation.SmallerThan] = 3,
        [Operation.SmallerEquals] = 3,
        [Operation.Equals] = 4,
        [Operation.NotEquals] = 4,
        [Operation.BitwiseAnd] = 5,
        [Operation.BitwiseXor] = 6,
        [Operation.BitwiseOr] = 7,
        [Operation.LogicalAnd] = 8,
        [Operation.LogicalOr] = 9
    };

    private readonly Dictionary<SyntaxTokenKind, int> _tokenPrecedence = new()
    {
        [SyntaxTokenKind.Asterisk] = 0,
        [SyntaxTokenKind.Slash] = 0,
        [SyntaxTokenKind.Percent] = 0,
        [SyntaxTokenKind.Plus] = 1,
        [SyntaxTokenKind.Minus] = 1,
        [SyntaxTokenKind.ShiftLeft] = 2,
        [SyntaxTokenKind.ShiftRight] = 2,
        [SyntaxTokenKind.GreaterThan] = 3,
        [SyntaxTokenKind.GreaterEquals] = 3,
        [SyntaxTokenKind.SmallerThan] = 3,
        [SyntaxTokenKind.SmallerEquals] = 3,
        [SyntaxTokenKind.EqualsEquals] = 4,
        [SyntaxTokenKind.NotEquals] = 4,
        [SyntaxTokenKind.Ampersand] = 5,
        [SyntaxTokenKind.Caret] = 6,
        [SyntaxTokenKind.Pipe] = 7,
        [SyntaxTokenKind.AndKeyword] = 8,
        [SyntaxTokenKind.OrKeyword] = 9
    };

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
            case ScriptArgumentExpression expression:
                return CreateExpression(expression.Operations);

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

    private ExpressionSyntax CreateExpression(ExpressionOperation[] operations)
    {
        var stack = new Stack<ExpressionSyntax>();

        foreach (var operation in operations)
        {
            switch (operation.Operation)
            {
                case Operation.LoadInt:
                    stack.Push(CreateNumberLiteralExpression(operation.Value));
                    break;

                case Operation.LoadVariable:
                    stack.Push(CreateVariableExpression(operation.Value));
                    break;

                case Operation.ValueToVariableValue:
                    stack.Push(CreateVariableExpression(stack.Pop()));
                    break;

                case Operation.Multiply:
                case Operation.Divide:
                case Operation.Modulo:
                case Operation.Add:
                case Operation.Subtract:
                case Operation.ShiftLeft:
                case Operation.ShiftRight:
                case Operation.GreaterEquals:
                case Operation.GreaterThan:
                case Operation.SmallerEquals:
                case Operation.SmallerThan:
                case Operation.Equals:
                case Operation.NotEquals:
                case Operation.BitwiseAnd:
                case Operation.BitwiseXor:
                case Operation.BitwiseOr:
                    stack.Push(CreateBinaryExpression(stack, operation.Operation));
                    break;

                case Operation.LogicalAnd:
                case Operation.LogicalOr:
                    stack.Push(CreateLogicalExpression(stack, operation.Operation));
                    break;

                default:
                    throw new InvalidOperationException($"Unknown operation {operation}.");
            }
        }

        return stack.Pop();
    }

    private BinaryExpressionSyntax CreateBinaryExpression(Stack<ExpressionSyntax> syntax, Operation operation)
    {
        var right = syntax.Pop();
        var left = syntax.Pop();

        SyntaxToken operatorToken;
        switch (operation)
        {
            case Operation.Multiply:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.Asterisk);
                break;

            case Operation.Divide:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.Slash);
                break;

            case Operation.Modulo:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.Percent);
                break;

            case Operation.Add:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.Plus);
                break;

            case Operation.Subtract:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.Minus);
                break;

            case Operation.ShiftLeft:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.ShiftLeft);
                break;

            case Operation.ShiftRight:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.ShiftRight);
                break;

            case Operation.GreaterEquals:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.GreaterEquals);
                break;

            case Operation.GreaterThan:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.GreaterThan);
                break;

            case Operation.SmallerEquals:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.SmallerEquals);
                break;

            case Operation.SmallerThan:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.SmallerThan);
                break;

            case Operation.Equals:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.EqualsEquals);
                break;

            case Operation.NotEquals:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.NotEquals);
                break;

            case Operation.BitwiseAnd:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.Ampersand);
                break;

            case Operation.BitwiseXor:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.Caret);
                break;

            case Operation.BitwiseOr:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.Pipe);
                break;

            default:
                throw new InvalidOperationException($"Unknown binary operation {operation}.");
        }

        int currentPrecedence = _operatorPrecedence[operation];

        if (left is LogicalExpressionSyntax)
            left = CreateParenthesizedExpression(left);
        else if (left is BinaryExpressionSyntax binary)
        {
            int leftPrecedence = _tokenPrecedence[(SyntaxTokenKind)binary.Operation.RawKind];

            if (currentPrecedence < leftPrecedence)
                left = CreateParenthesizedExpression(left);
        }

        if (right is LogicalExpressionSyntax)
            right = CreateParenthesizedExpression(right);
        else if (right is BinaryExpressionSyntax binary)
        {
            int rightPrecedence = _tokenPrecedence[(SyntaxTokenKind)binary.Operation.RawKind];

            if (currentPrecedence <= rightPrecedence)
                right = CreateParenthesizedExpression(right);
        }

        return new BinaryExpressionSyntax(left, operatorToken, right);
    }

    private LogicalExpressionSyntax CreateLogicalExpression(Stack<ExpressionSyntax> syntax, Operation operation)
    {
        var right = syntax.Pop();
        var left = syntax.Pop();

        SyntaxToken operatorToken;
        switch (operation)
        {
            case Operation.LogicalAnd:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.AndKeyword);
                break;

            case Operation.LogicalOr:
                operatorToken = syntaxFactory.Token(SyntaxTokenKind.OrKeyword);
                break;

            default:
                throw new InvalidOperationException($"Unknown logical operation {operation}.");
        }

        int currentPrecedence = _operatorPrecedence[operation];

        if (left is LogicalExpressionSyntax logical)
        {
            int leftPrecedence = _tokenPrecedence[(SyntaxTokenKind)logical.Operation.RawKind];

            if (currentPrecedence < leftPrecedence)
                left = CreateParenthesizedExpression(left);
        }

        if (right is LogicalExpressionSyntax logical1)
        {
            int rightPrecedence = _tokenPrecedence[(SyntaxTokenKind)logical1.Operation.RawKind];

            if (currentPrecedence < rightPrecedence)
                right = CreateParenthesizedExpression(right);
        }

        return new LogicalExpressionSyntax(left, operatorToken, right);
    }

    private ParenthesizedExpressionSyntax CreateParenthesizedExpression(ExpressionSyntax expression)
    {
        var parenOpen = syntaxFactory.Token(SyntaxTokenKind.ParenOpen);
        var parenClose = syntaxFactory.Token(SyntaxTokenKind.ParenClose);

        return new ParenthesizedExpressionSyntax(parenOpen, expression, parenClose);
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