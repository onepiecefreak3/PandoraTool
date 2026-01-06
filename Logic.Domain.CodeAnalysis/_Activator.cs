using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.EventBrokerage;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.CodeAnalysis.Contract;
using Logic.Domain.CodeAnalysis.Contract.Pandora;
using Logic.Domain.CodeAnalysis.DataClasses.Pandora;
using Logic.Domain.CodeAnalysis.Pandora;

namespace Logic.Domain.CodeAnalysis;

public class CodeAnalysisActivator : IComponentActivator
{
    public void Activating()
    {
    }

    public void Activated()
    {
    }

    public void Deactivating()
    {
    }

    public void Deactivated()
    {
    }

    public void Register(ICoCoKernel kernel)
    {
        kernel.Register<ITokenFactory<PandoraSyntaxToken>, PandoraScriptFactory>(ActivationScope.Unique);
        kernel.Register<ILexer<PandoraSyntaxToken>, PandoraScriptLexer>();
        kernel.Register<IBuffer<PandoraSyntaxToken>, TokenBuffer<PandoraSyntaxToken>>();
        kernel.Register<IBuffer<int>, StringBuffer>();

        kernel.Register<IPandoraScriptParser, PandoraScriptParser>(ActivationScope.Unique);
        kernel.Register<IPandoraScriptComposer, PandoraScriptComposer>(ActivationScope.Unique);
        kernel.Register<IPandoraScriptWhitespaceNormalizer, PandoraScriptWhitespaceNormalizer>(ActivationScope.Unique);

        kernel.Register<IPandoraSyntaxFactory, PandoraSyntaxFactory>();

        kernel.RegisterConfiguration<CodeAnalysisConfiguration>();
    }

    public void AddMessageSubscriptions(IEventBroker broker)
    {
    }

    public void Configure(IConfigurator configurator)
    {
    }
}