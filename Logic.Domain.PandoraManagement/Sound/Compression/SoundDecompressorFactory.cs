using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.PandoraManagement.Contract.Enums.Sound;
using Logic.Domain.PandoraManagement.Contract.Sound.Compression;
using Logic.Domain.PandoraManagement.InternalContract.Sound;

namespace Logic.Domain.PandoraManagement.Sound.Compression;

internal class SoundDecompressorFactory(ICoCoKernel kernel) : ISoundDecompressorFactory
{
    public ISoundDecompressor Get(SoundCompression compression)
    {
        return compression switch
        {
            SoundCompression.Sound8 => kernel.Get<ISoundDecompressor8>(),
            SoundCompression.Sound12 => kernel.Get<ISoundDecompressor12>(),
            _ => throw new InvalidOperationException($"Unknown sound compression {compression}.")
        };
    }
}