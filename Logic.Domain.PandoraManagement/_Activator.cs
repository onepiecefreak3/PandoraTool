using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Domain.PandoraManagement.Archive;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.Image;
using Logic.Domain.PandoraManagement.Contract.Image.Compression;
using Logic.Domain.PandoraManagement.Contract.Script;
using Logic.Domain.PandoraManagement.Contract.Sound;
using Logic.Domain.PandoraManagement.Contract.Sound.Compression;
using Logic.Domain.PandoraManagement.Image;
using Logic.Domain.PandoraManagement.Image.Compression;
using Logic.Domain.PandoraManagement.InternalContract.Image;
using Logic.Domain.PandoraManagement.InternalContract.Sound;
using Logic.Domain.PandoraManagement.Script;
using Logic.Domain.PandoraManagement.Sound;
using Logic.Domain.PandoraManagement.Sound.Compression;

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

        kernel.Register<IImageDecompressorFactory, ImageDecompressorFactory>(ActivationScope.Unique);
        kernel.Register<IImageDecompressorPixel, ImageDecompressorPixel>(ActivationScope.Unique);
        kernel.Register<IImageDecompressorLzss01, ImageDecompressorLzss01>(ActivationScope.Unique);
        kernel.Register<IImageCompressorFactory, ImageCompressorFactory>(ActivationScope.Unique);
        kernel.Register<IImageCompressorPixel, ImageCompressorPixel>(ActivationScope.Unique);
        kernel.Register<IImageCompressorLzss01, ImageCompressorLzss01>(ActivationScope.Unique);
        kernel.Register<IImageReader, ImageReader>(ActivationScope.Unique);
        kernel.Register<IImageParser, ImageParser>(ActivationScope.Unique);
        kernel.Register<IImageWriter, ImageWriter>(ActivationScope.Unique);
        kernel.Register<IImageComposer, ImageComposer>(ActivationScope.Unique);

        kernel.Register<ISoundDecompressorFactory, SoundDecompressorFactory>(ActivationScope.Unique);
        kernel.Register<ISoundDecompressor8, SoundDecompressor8>(ActivationScope.Unique);
        kernel.Register<ISoundDecompressor12, SoundDecompressor12>(ActivationScope.Unique);
        kernel.Register<ISoundCompressorFactory, SoundCompressorFactory>(ActivationScope.Unique);
        kernel.Register<ISoundCompressor8, SoundCompressor8>(ActivationScope.Unique);
        kernel.Register<ISoundCompressor12, SoundCompressor12>(ActivationScope.Unique);
        kernel.Register<ISoundReader, SoundReader>(ActivationScope.Unique);
        kernel.Register<ISoundParser, SoundParser>(ActivationScope.Unique);
        kernel.Register<ISoundWriter, SoundWriter>(ActivationScope.Unique);
        kernel.Register<ISoundComposer, SoundComposer>(ActivationScope.Unique);

        kernel.Register<IScriptReader, ScriptReader>(ActivationScope.Unique);
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