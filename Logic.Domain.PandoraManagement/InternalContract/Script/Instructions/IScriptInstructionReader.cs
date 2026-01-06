using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.InternalContract.Script.Instructions;

internal interface IScriptInstructionReader
{
    ScriptInstructionData Read(byte[] data, ref int offset);
}