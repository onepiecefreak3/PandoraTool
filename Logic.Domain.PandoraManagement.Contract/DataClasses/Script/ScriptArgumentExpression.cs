namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

public class ScriptArgumentExpression : ScriptArgument
{
    public required ExpressionOperation[] Operations { get; init; }
}