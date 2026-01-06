using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Archive;

internal class ArchiveComposer(IArchiveListWriter listWriter, IArchiveTagWriter tagWriter) : IArchiveComposer
{
    public void Compose(ArchiveFile[] files, Stream dataStream, Stream listStream, Stream? tagStream)
    {
        FileEntry[] entries = WriteFiles(files, dataStream);

        listWriter.Write(entries, listStream);

        if (tagStream is not null)
        {
            TagEntry[] tags = CreateTags(files);
            tagWriter.Write(tags, tagStream);
        }
    }

    private FileEntry[] WriteFiles(ArchiveFile[] files, Stream dataStream)
    {
        var result = new FileEntry[files.Length];

        for (var i = 0; i < files.Length; i++)
        {
            ArchiveFile file = files[i];

            var offset = (int)dataStream.Position;

            file.Data.Position = 0;
            file.Data.CopyTo(dataStream);

            result[i] = new FileEntry
            {
                FileName = file.Name,
                Offset = offset,
                Size = (int)file.Data.Length
            };
        }

        return result;
    }

    private TagEntry[] CreateTags(ArchiveFile[] files)
    {
        var result = new TagEntry[files.Length];

        for (var i = 0; i < files.Length; i++)
        {
            ArchiveFile file = files[i];

            result[i] = new TagEntry
            {
                Attributes = file.Attributes ?? 0x20,
                DateTime = file.DateTime ?? DateTime.Now
            };
        }

        return result;
    }
}