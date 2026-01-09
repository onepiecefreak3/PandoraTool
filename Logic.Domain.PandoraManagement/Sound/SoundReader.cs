using Logic.Domain.PandoraManagement.Contract.DataClasses.Sound;
using Logic.Domain.PandoraManagement.Contract.Enums.Sound;
using Logic.Domain.PandoraManagement.Contract.Sound;
using System.Buffers.Binary;

namespace Logic.Domain.PandoraManagement.Sound;

internal class SoundReader : ISoundReader
{
    public SoundCompression ReadCompression(byte[] data)
    {
        int format = BinaryPrimitives.ReadInt32LittleEndian(data);

        if (format == 0x46464952)
            return SoundCompression.None;

        return (SoundCompression)format;
    }

    public SoundData Read(byte[] data)
    {
        SoundCompression compression = ReadCompression(data);

        return new SoundData
        {
            Compression = compression,
            Data = compression is SoundCompression.None ? data : data[4..]
        };
    }
}