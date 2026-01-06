using Komponent.IO;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Archive;

internal class ArchiveListWriter : IArchiveListWriter
{
    public void Write(FileEntry[] entries, Stream listStream)
    {
        using var writer = new BinaryWriterX(listStream, true);

        foreach (FileEntry entry in entries)
        {
            writer.WriteString(entry.FileName.PadRight(0xE, '\0')[..0xE], writeNullTerminator: false);
            writer.Write(entry.Offset);
            writer.Write(entry.Size);
        }
    }
}