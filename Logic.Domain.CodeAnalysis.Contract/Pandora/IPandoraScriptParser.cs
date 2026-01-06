using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;

namespace Logic.Domain.CodeAnalysis.Contract.Pandora;

public interface IPandoraScriptParser
{
    CodeUnitSyntax ParseCodeUnit(string text);
}