using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.InternalContract.Script.Instructions;

internal interface IScriptInstructionCalculator
{
    int CalculateLength(ScriptInstruction instruction);
}