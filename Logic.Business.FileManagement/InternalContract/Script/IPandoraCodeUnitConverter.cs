using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Business.FileManagement.InternalContract.Script;

internal interface IPandoraCodeUnitConverter
{
    ScriptInstruction[] CreateInstructions(CodeUnitSyntax codeUnit);
}