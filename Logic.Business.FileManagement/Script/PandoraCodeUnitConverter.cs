using Logic.Business.FileManagement.InternalContract.Script;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Enums.Script;
using System.Text.RegularExpressions;

namespace Logic.Business.FileManagement.Script;

internal class PandoraCodeUnitConverter : IPandoraCodeUnitConverter
{
    private readonly Regex _subPattern = new("^sub[0-9]+$", RegexOptions.Compiled);

    public ScriptInstruction[] CreateInstructions(CodeUnitSyntax codeUnit)
    {
        var instructions = new List<ScriptInstruction>();

        AddMethods(instructions, codeUnit.MethodDeclarations);

        return [.. instructions];
    }

    private void AddMethods(List<ScriptInstruction> instructions, IReadOnlyList<MethodDeclarationSyntax> methods)
    {
        if (methods.Count <= 0)
            return;

        AddStatements(instructions, methods[0].Body.Statements);
    }

    private void AddStatements(List<ScriptInstruction> instructions, IReadOnlyList<StatementSyntax> statements)
    {
        string? jumpLabel = null;

        foreach (StatementSyntax statement in statements)
        {
            switch (statement)
            {
                case GotoLabelStatementSyntax gotoStatement:
                    jumpLabel = gotoStatement.Label.Literal.Text[1..^2].Replace(@"\\", @"\");
                    break;

                case MethodInvocationStatementSyntax methodInvocation:
                    AddMethodInvocationStatement(instructions, methodInvocation, jumpLabel);
                    jumpLabel = null;
                    break;

                default:
                    throw CreateException("Only method invocations are allowed.", statement.Location);
            }
        }
    }

    private void AddMethodInvocationStatement(List<ScriptInstruction> instructions, MethodInvocationStatementSyntax methodInvocation, string? jumpLabel)
    {
        int instructionType = GetInstruction(methodInvocation.Name);

        var arguments = new List<ScriptArgument>();

        if (methodInvocation.Parameters.ParameterList != null)
        {
            foreach (ExpressionSyntax parameter in methodInvocation.Parameters.ParameterList.Elements)
            {
                AddArgument(arguments, parameter);
            }
        }

        instructions.Add(new ScriptInstruction
        {
            Instruction = instructionType,
            Arguments = [.. arguments],
            JumpLabel = jumpLabel
        });
    }

    private void AddArgument(List<ScriptArgument> arguments, ExpressionSyntax parameter)
    {
        switch (parameter)
        {
            case LiteralExpressionSyntax literal:
                AddArgument(arguments, literal);
                break;

            default:
                var operations = new List<ExpressionOperation>();
                AddOperations(operations, parameter);

                arguments.Add(new ScriptArgumentExpression { Operations = [.. operations] });
                break;
        }
    }

    private void AddOperations(List<ExpressionOperation> operations, ExpressionSyntax expression)
    {
        switch (expression)
        {
            case LiteralExpressionSyntax literal:
                if (literal.Literal.RawKind is not (int)SyntaxTokenKind.NumericLiteral)
                    throw CreateException("Literals should only be numeric in expressions.", literal.Location, SyntaxTokenKind.NumericLiteral);

                operations.Add(new ExpressionOperation(Operation.LoadInt, int.Parse(literal.Literal.Text)));
                break;

            case VariableExpressionSyntax variable:
                if (variable.Expression is LiteralExpressionSyntax variableLiteral)
                {
                    if (variableLiteral.Literal.RawKind is not (int)SyntaxTokenKind.NumericLiteral)
                        throw CreateException("Literals should only be numeric in expressions.", variableLiteral.Location, SyntaxTokenKind.NumericLiteral);

                    operations.Add(new ExpressionOperation(Operation.LoadVariable, int.Parse(variableLiteral.Literal.Text)));
                }
                else
                {
                    AddOperations(operations, variable.Expression);
                    operations.Add(new ExpressionOperation(Operation.ValueToVariableValue, -1));
                }
                break;

            case ParenthesizedExpressionSyntax parenthesizedExpression:
                AddOperations(operations, parenthesizedExpression.Expression);
                break;

            case BinaryExpressionSyntax binary:
                AddOperations(operations, binary.Left);
                AddOperations(operations, binary.Right);

                switch (binary.Operation.RawKind)
                {
                    case (int)SyntaxTokenKind.Asterisk:
                        operations.Add(new ExpressionOperation(Operation.Multiply, -1));
                        break;

                    case (int)SyntaxTokenKind.Slash:
                        operations.Add(new ExpressionOperation(Operation.Divide, -1));
                        break;

                    case (int)SyntaxTokenKind.Percent:
                        operations.Add(new ExpressionOperation(Operation.Modulo, -1));
                        break;

                    case (int)SyntaxTokenKind.Plus:
                        operations.Add(new ExpressionOperation(Operation.Add, -1));
                        break;

                    case (int)SyntaxTokenKind.Minus:
                        operations.Add(new ExpressionOperation(Operation.Subtract, -1));
                        break;

                    case (int)SyntaxTokenKind.ShiftLeft:
                        operations.Add(new ExpressionOperation(Operation.ShiftLeft, -1));
                        break;

                    case (int)SyntaxTokenKind.ShiftRight:
                        operations.Add(new ExpressionOperation(Operation.ShiftRight, -1));
                        break;

                    case (int)SyntaxTokenKind.GreaterEquals:
                        operations.Add(new ExpressionOperation(Operation.GreaterEquals, -1));
                        break;

                    case (int)SyntaxTokenKind.GreaterThan:
                        operations.Add(new ExpressionOperation(Operation.GreaterThan, -1));
                        break;

                    case (int)SyntaxTokenKind.SmallerEquals:
                        operations.Add(new ExpressionOperation(Operation.SmallerEquals, -1));
                        break;

                    case (int)SyntaxTokenKind.SmallerThan:
                        operations.Add(new ExpressionOperation(Operation.SmallerThan, -1));
                        break;

                    case (int)SyntaxTokenKind.EqualsEquals:
                        operations.Add(new ExpressionOperation(Operation.Equals, -1));
                        break;

                    case (int)SyntaxTokenKind.NotEquals:
                        operations.Add(new ExpressionOperation(Operation.NotEquals, -1));
                        break;

                    case (int)SyntaxTokenKind.Ampersand:
                        operations.Add(new ExpressionOperation(Operation.BitwiseAnd, -1));
                        break;

                    case (int)SyntaxTokenKind.Caret:
                        operations.Add(new ExpressionOperation(Operation.BitwiseXor, -1));
                        break;

                    case (int)SyntaxTokenKind.Pipe:
                        operations.Add(new ExpressionOperation(Operation.BitwiseOr, -1));
                        break;

                    default:
                        throw CreateException("Invalid binary expression.", expression.Location, SyntaxTokenKind.Asterisk, SyntaxTokenKind.Slash,
                            SyntaxTokenKind.Percent, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.ShiftLeft, SyntaxTokenKind.ShiftRight,
                            SyntaxTokenKind.GreaterEquals, SyntaxTokenKind.GreaterThan, SyntaxTokenKind.SmallerEquals, SyntaxTokenKind.SmallerThan,
                            SyntaxTokenKind.EqualsEquals, SyntaxTokenKind.NotEquals, SyntaxTokenKind.Ampersand, SyntaxTokenKind.Caret,
                            SyntaxTokenKind.Pipe);
                }
                break;

            case LogicalExpressionSyntax logical:
                AddOperations(operations, logical.Left);
                AddOperations(operations, logical.Right);

                switch (logical.Operation.RawKind)
                {
                    case (int)SyntaxTokenKind.AndKeyword:
                        operations.Add(new ExpressionOperation(Operation.LogicalAnd, -1));
                        break;

                    case (int)SyntaxTokenKind.OrKeyword:
                        operations.Add(new ExpressionOperation(Operation.LogicalOr, -1));
                        break;

                    default:
                        throw CreateException("Invalid logical expression.", expression.Location, SyntaxTokenKind.AndKeyword, SyntaxTokenKind.OrKeyword);
                }
                break;
        }
    }

    private void AddArgument(List<ScriptArgument> arguments, LiteralExpressionSyntax literal)
    {
        switch (literal.Literal.RawKind)
        {
            case (int)SyntaxTokenKind.StringLiteral:
                arguments.Add(new ScriptArgumentString { Text = literal.Literal.Text[1..^1].Replace(@"\\", @"\") });
                break;

            case (int)SyntaxTokenKind.JumpLiteral:
                arguments.Add(new ScriptArgumentJump { Label = literal.Literal.Text[1..^2].Replace(@"\\", @"\") });
                break;

            case (int)SyntaxTokenKind.NumericLiteral:
                arguments.Add(new ScriptArgumentInt { Value = int.Parse(literal.Literal.Text) });
                break;
        }
    }

    private int GetInstruction(NameSyntax name)
    {
        string composedName = GetName(name);

        if (_subPattern.IsMatch(composedName))
            return GetNumberFromStringEnd(composedName);

        throw CreateException("Could not determine instruction type.", name.Location);
    }

    private string GetName(NameSyntax name)
    {
        switch (name)
        {
            case SimpleNameSyntax simpleName:
                return simpleName.Identifier.Text;

            case QualifiedNameSyntax qualifiedName:
                return GetName(qualifiedName.Left) + "." + GetName(qualifiedName.Right);

            default:
                throw CreateException("Invalid name syntax.", name.Location);
        }
    }

    private int GetNumberFromStringEnd(string text)
    {
        int startIndex = text.Length;
        while (text[startIndex - 1] >= '0' && text[startIndex - 1] <= '9')
            startIndex--;

        return int.Parse(text[startIndex..]);
    }

    private Exception CreateException(string message, SyntaxLocation location, params SyntaxTokenKind[] expected)
    {
        message = $"{message} (Line {location.Line}, Column {location.Column})";

        if (expected.Length > 0)
        {
            message = expected.Length == 1 ?
                $"{message} (Expected {expected[0]})" :
                $"{message} (Expected any of {string.Join(", ", expected)})";
        }

        return new InvalidOperationException(message);
    }
}