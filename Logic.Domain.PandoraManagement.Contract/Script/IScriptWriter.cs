using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Contract.Script;

public interface IScriptWriter
{
    byte[] Write(ScriptInstructionData[] instructions);
}