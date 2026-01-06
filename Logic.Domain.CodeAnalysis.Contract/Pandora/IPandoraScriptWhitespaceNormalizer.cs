using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;

namespace Logic.Domain.CodeAnalysis.Contract.Pandora;

public interface IPandoraScriptWhitespaceNormalizer
{
    void NormalizeCodeUnit(CodeUnitSyntax codeUnit);
}