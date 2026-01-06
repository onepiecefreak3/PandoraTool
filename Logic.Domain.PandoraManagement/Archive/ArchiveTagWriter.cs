using Komponent.IO;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Archive;

internal class ArchiveTagWriter : IArchiveTagWriter
{
    public void Write(TagEntry[] tags, Stream tagStream)
    {
        using var writer = new BinaryWriterX(tagStream, true);

        foreach (TagEntry tag in tags)
        {
            var date = ((tag.DateTime.Year - 1980) << 9) | (tag.DateTime.Month << 5) | tag.DateTime.Day;
            var time = (tag.DateTime.Hour << 11) | (tag.DateTime.Minute << 5) | (tag.DateTime.Second / 2);

            writer.Write((short)tag.Attributes);
            writer.Write((ushort)date);
            writer.Write((ushort)time);
            writer.WriteAlignment(0x10);
        }
    }
}