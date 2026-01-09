using Logic.Domain.PandoraManagement.Contract.DataClasses.Sound;
using Logic.Domain.PandoraManagement.Contract.Enums.Sound;
using Logic.Domain.PandoraManagement.Contract.Sound;
using Logic.Domain.PandoraManagement.Contract.Sound.Compression;

namespace Logic.Domain.PandoraManagement.Sound;

internal class SoundComposer(ISoundWriter writer, ISoundCompressorFactory compressorFactory) : ISoundComposer
{
    public void Compose(SoundFile file, Stream output)
    {
        byte[] compressedData = file.Data;
        if (file.Compression is not SoundCompression.None)
        {
            ISoundCompressor compressor = compressorFactory.Get(file.Compression);
            compressedData = compressor.Compress(file.Data);
        }

        var data = new SoundData
        {
            Compression = file.Compression,
            Data = compressedData
        };

        writer.Write(data, output);
    }
}