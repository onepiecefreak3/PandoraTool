using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.CodeAnalysis.Contract;
using Logic.Domain.CodeAnalysis.DataClasses.Pandora;

namespace Logic.Domain.CodeAnalysis.Pandora;

internal class PandoraScriptFactory : ITokenFactory<PandoraSyntaxToken>
{
    private readonly ICoCoKernel _kernel;

    public PandoraScriptFactory(ICoCoKernel kernel)
    {
        _kernel = kernel;
    }

    public ILexer<PandoraSyntaxToken> CreateLexer(string text)
    {
        var buffer = _kernel.Get<IBuffer<int>>(
            new ConstructorParameter("text", text));
        return _kernel.Get<ILexer<PandoraSyntaxToken>>(
            new ConstructorParameter("buffer", buffer));
    }

    public IBuffer<PandoraSyntaxToken> CreateTokenBuffer(ILexer<PandoraSyntaxToken> lexer)
    {
        return _kernel.Get<IBuffer<PandoraSyntaxToken>>(new ConstructorParameter("lexer", lexer));
    }
}