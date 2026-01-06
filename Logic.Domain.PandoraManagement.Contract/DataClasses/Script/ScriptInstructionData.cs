namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

public class ScriptInstructionData
{
    public required int Offset { get; init; }
    public required int Instruction { get; init; }
    public required ScriptArgumentData[] Arguments { get; init; }
}