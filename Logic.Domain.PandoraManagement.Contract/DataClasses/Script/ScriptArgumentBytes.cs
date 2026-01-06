namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

public class ScriptArgumentBytes : ScriptArgument
{
    public required byte[] Data { get; init; }
}