using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Contract.Script;

public interface IScriptParser
{
    ScriptInstruction[] Parse(byte[] data);
}