using Komponent.IO;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Sound;
using Logic.Domain.PandoraManagement.Contract.Enums.Sound;
using Logic.Domain.PandoraManagement.Contract.Sound;

namespace Logic.Domain.PandoraManagement.Sound;

internal class SoundWriter : ISoundWriter
{
    public void Write(SoundData data, Stream output)
    {
        using var writer = new BinaryWriterX(output, true);

        if (data.Compression is not SoundCompression.None)
            writer.Write((int)data.Compression);

        writer.Write(data.Data);
    }
}