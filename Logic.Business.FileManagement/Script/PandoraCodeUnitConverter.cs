using Logic.Business.FileManagement.InternalContract.Script;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
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

            case VariableExpressionSyntax variable:
                AddArgument(arguments, variable);
                break;
        }
    }

    private void AddArgument(List<ScriptArgument> arguments, LiteralExpressionSyntax literal)
    {
        switch (literal.Literal.RawKind)
        {
            case (int)SyntaxTokenKind.DataLiteral:
                var dataString = literal.Literal.Text[1..^1];
                var dataLength = dataString.Length * 6;

                if (dataLength % 8 != 0)
                    throw CreateException("Invalid data length.", literal.Location, SyntaxTokenKind.DataLiteral);

                var data = new byte[dataLength / 8];
                if (!Convert.TryFromBase64String(literal.Literal.Text[1..^1], data, out int writtenLength))
                    throw CreateException("Invalid data argument.", literal.Location, SyntaxTokenKind.DataLiteral);

                arguments.Add(new ScriptArgumentBytes { Data = data[..writtenLength] });
                break;

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

    private void AddArgument(List<ScriptArgument> arguments, VariableExpressionSyntax variable)
    {
        if (variable.Expression is not LiteralExpressionSyntax literal)
            throw CreateException("Could not determine index of variable.", variable.Expression.Location);

        arguments.Add(new ScriptArgumentVariable { Value = int.Parse(literal.Literal.Text) });
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