using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Contract.Archive;

public interface IArchiveListReader
{
    FileEntry[] Read(Stream listStream);
}