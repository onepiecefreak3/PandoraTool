using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;

namespace Logic.Domain.CodeAnalysis.Contract.Pandora;

public interface IPandoraScriptComposer
{
    string ComposeCodeUnit(CodeUnitSyntax codeUnit);
}