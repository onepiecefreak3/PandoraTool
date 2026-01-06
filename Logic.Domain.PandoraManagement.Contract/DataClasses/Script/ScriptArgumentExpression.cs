using Logic.Domain.PandoraManagement.Contract.Enums.Script;

namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

public class ScriptArgumentExpression : ScriptArgumentBytes
{
    public required ArgumentType Type { get; init; }
    public required ExpressionOperation[] Operations { get; init; }
}