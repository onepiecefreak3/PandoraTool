using Komponent.IO;
using Komponent.Streams;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;
using Logic.Domain.PandoraManagement.Contract.Enums;

namespace Logic.Domain.PandoraManagement.Archive;

internal class ArchiveParser(IArchiveListReader listReader, IArchiveTagReader tagReader) : IArchiveParser
{
    public ArchiveFile[] Parse(Stream dataStream, Stream listStream, Stream? tagStream)
    {
        FileEntry[] entries = listReader.Read(listStream);

        TagEntry[] tags = [];
        if (tagStream is not null)
            tags = tagReader.Read(tagStream);

        return Parse(dataStream, entries, tags);
    }

    public ArchiveFile[] Parse(Stream dataStream, FileEntry[] entries, TagEntry[]? tags)
    {
        var result = new List<ArchiveFile>();

        for (var i = 0; i < entries.Length; i++)
        {
            TagEntry? tag = null;
            if (tags is not null && i < tags.Length)
                tag = tags[i];

            result.Add(CreateFile(dataStream, entries[i], tag));
        }

        return [.. result];
    }

    private ArchiveFile CreateFile(Stream dataStream, FileEntry entry, TagEntry? tag)
    {
        var fileStream = new SubStream(dataStream, entry.Offset, entry.Size);

        return new ArchiveFile
        {
            Compression = PeekCompression(fileStream),
            Type = PeekType(entry),
            Name = entry.FileName,
            Data = fileStream,
            Attributes = tag?.Attributes,
            DateTime = tag?.DateTime
        };
    }

    private static FileCompression PeekCompression(Stream fileStream)
    {
        using var reader = new BinaryReaderX(fileStream, true);

        int compressedSize = reader.ReadInt32();
        fileStream.Position -= 4;

        return compressedSize + 8 == fileStream.Length ? FileCompression.Lzss01 : FileCompression.None;
    }

    private static FileType PeekType(FileEntry entry)
    {
        return Path.GetExtension(entry.FileName).ToLower() switch
        {
            ".pt1" => FileType.Image,
            ".wa1" => FileType.Sound,
            ".so4" => FileType.Script,
            _ => FileType.Binary
        };
    }
}