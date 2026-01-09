using Logic.Domain.PandoraManagement.Contract.Sound;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Sound;
using Logic.Domain.PandoraManagement.Contract.Enums.Sound;
using Logic.Domain.PandoraManagement.Contract.Sound.Compression;

namespace Logic.Domain.PandoraManagement.Sound;

internal class SoundParser(ISoundReader reader, ISoundDecompressorFactory decompressorFactory) : ISoundParser
{
    public SoundFile Parse(byte[] data)
    {
        SoundData soundData = reader.Read(data);

        byte[] decompressedData = data;
        if (soundData.Compression is not SoundCompression.None)
        {
            ISoundDecompressor decompressor = decompressorFactory.Get(soundData.Compression);
            decompressedData = decompressor.Decompress(soundData.Data);
        }

        return new SoundFile
        {
            Compression = soundData.Compression,
            Data = decompressedData
        };
    }
}