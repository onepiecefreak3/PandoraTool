using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Contract.Archive;

public interface IArchiveParser
{
    ArchiveFile[] Parse(Stream dataStream, Stream listStream, Stream? tagStream = null);
}