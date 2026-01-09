using Logic.Domain.PandoraManagement.Contract.Enums.Sound;

namespace Logic.Domain.PandoraManagement.Contract.Sound.Compression;

public interface ISoundDecompressorFactory
{
    ISoundDecompressor Get(SoundCompression compression);
}