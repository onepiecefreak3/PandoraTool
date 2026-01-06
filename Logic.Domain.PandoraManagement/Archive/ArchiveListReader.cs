using System.Text;
using Komponent.IO;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Archive;

internal class ArchiveListReader : IArchiveListReader
{
    public FileEntry[] Read(Stream listStream)
    {
        using var reader = new BinaryReaderX(listStream, true);

        var result = new List<FileEntry>();

        while (listStream.Position < listStream.Length)
            result.Add(ReadEntry(reader));

        return [.. result];
    }

    private FileEntry ReadEntry(BinaryReaderX reader)
    {
        return new FileEntry
        {
            FileName = Encoding.ASCII.GetString(reader.ReadBytes(0xE)).Trim('\0'),
            Offset = reader.ReadInt32(),
            Size = reader.ReadInt32()
        };
    }
}