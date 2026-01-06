using Logic.Domain.PandoraManagement.Contract.Enums.Script;

namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

public class ScriptArgumentData
{
    public required int Offset { get; init; }
    public required ArgumentType Type { get; init; }
    public required byte[] Data { get; init; }
}