using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Domain.PandoraManagement.Archive;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.Image;
using Logic.Domain.PandoraManagement.Contract.Script;
using Logic.Domain.PandoraManagement.Contract.Sound;
using Logic.Domain.PandoraManagement.Image;
using Logic.Domain.PandoraManagement.Script;
using Logic.Domain.PandoraManagement.Sound;

namespace Logic.Domain.PandoraManagement;

public class PandoraManagementActivator : IComponentActivator
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
        kernel.Register<IArchiveListReader, ArchiveListReader>(ActivationScope.Unique);
        kernel.Register<IArchiveListWriter, ArchiveListWriter>(ActivationScope.Unique);
        kernel.Register<IArchiveTagReader, ArchiveTagReader>(ActivationScope.Unique);
        kernel.Register<IArchiveTagWriter, ArchiveTagWriter>(ActivationScope.Unique);
        kernel.Register<IArchiveParser, ArchiveParser>(ActivationScope.Unique);
        kernel.Register<IArchiveComposer, ArchiveComposer>(ActivationScope.Unique);

        kernel.Register<IFileDecompressor, FileDecompressor>(ActivationScope.Unique);
        kernel.Register<IFileCompressor, FileCompressor>(ActivationScope.Unique);

        kernel.Register<IImageParser, ImageParser>(ActivationScope.Unique);

        kernel.Register<ISoundParser, SoundParser>(ActivationScope.Unique);

        kernel.Register<IScriptReader , ScriptReader>(ActivationScope.Unique);
        kernel.Register<IScriptParser, ScriptParser>(ActivationScope.Unique);
        kernel.Register<IScriptWriter, ScriptWriter>(ActivationScope.Unique);
        kernel.Register<IScriptComposer, ScriptComposer>(ActivationScope.Unique);

        kernel.RegisterConfiguration<PandoraManagementConfiguration>();
    }

    public void AddMessageSubscriptions(IEventBroker broker)
    {
    }

    public void Configure(IConfigurator configurator)
    {
    }
}