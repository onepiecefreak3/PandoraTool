using Logic.Domain.PandoraManagement.Contract.Enums.Sound;
using Logic.Domain.PandoraManagement.Contract.Sound.Compression;
using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.PandoraManagement.InternalContract.Sound;

namespace Logic.Domain.PandoraManagement.Sound.Compression;

internal class SoundCompressorFactory(ICoCoKernel kernel) : ISoundCompressorFactory
{
    public ISoundCompressor Get(SoundCompression compression)
    {
        return compression switch
        {
            SoundCompression.Sound8 => kernel.Get<ISoundCompressor8>(),
            SoundCompression.Sound12 => kernel.Get<ISoundCompressor12>(),
            _ => throw new InvalidOperationException($"Unknown sound compression {compression}.")
        };
    }
}