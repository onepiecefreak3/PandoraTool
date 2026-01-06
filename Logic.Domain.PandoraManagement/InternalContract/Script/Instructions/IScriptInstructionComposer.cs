using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.InternalContract.Script.Instructions;

internal interface IScriptInstructionComposer
{
    ScriptInstructionData Compose(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, ref int offset);
}