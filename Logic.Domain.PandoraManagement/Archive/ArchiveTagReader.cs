using Komponent.IO;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Archive;

internal class ArchiveTagReader : IArchiveTagReader
{
    public TagEntry[] Read(Stream tagStream)
    {
        using var reader = new BinaryReaderX(tagStream, true);

        var result = new List<TagEntry>();

        while (tagStream.Position < tagStream.Length)
            result.Add(ReadEntry(reader));

        return [.. result];
    }

    private TagEntry ReadEntry(BinaryReaderX reader)
    {
        var attributes = reader.ReadInt16();
        var date = reader.ReadUInt16();
        var time = reader.ReadUInt16();

        reader.SeekAlignment();

        return new TagEntry
        {
            Attributes = attributes,
            DateTime = new DateTime(((date >> 9) & 0x7F) + 1980, (date >> 5) & 0xF, date & 0x1F, (time >> 11) & 0x1F, (time >> 5) & 0x3F, (time & 0x1F) * 2)
        };
    }
}