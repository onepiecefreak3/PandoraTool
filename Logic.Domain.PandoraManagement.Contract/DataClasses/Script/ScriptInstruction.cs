namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

public class ScriptInstruction
{
    public required int Instruction { get; init; }
    public required ScriptArgument[] Arguments { get; init; }
    public string? JumpLabel { get; set; }
}