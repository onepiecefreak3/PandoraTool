using Logic.Domain.PandoraManagement.Contract.DataClasses.Sound;
using Logic.Domain.PandoraManagement.Contract.Enums.Sound;

namespace Logic.Domain.PandoraManagement.Contract.Sound;

public interface ISoundReader
{
    SoundCompression ReadCompression(byte[] data);
    SoundData Read(byte[] data);
}