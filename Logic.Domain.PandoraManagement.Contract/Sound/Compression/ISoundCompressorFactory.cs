using Logic.Domain.PandoraManagement.Contract.Enums.Sound;

namespace Logic.Domain.PandoraManagement.Contract.Sound.Compression;

public interface ISoundCompressorFactory
{
    ISoundCompressor Get(SoundCompression compression);
}