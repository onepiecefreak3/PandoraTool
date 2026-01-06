using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Contract.Script;

public interface IScriptReader
{
    ScriptInstructionData[] Read(byte[] data);
}