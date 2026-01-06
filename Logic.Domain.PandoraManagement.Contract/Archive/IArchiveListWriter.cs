using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Contract.Archive;

public interface IArchiveListWriter
{
    void Write(FileEntry[] entries, Stream listStream);
}