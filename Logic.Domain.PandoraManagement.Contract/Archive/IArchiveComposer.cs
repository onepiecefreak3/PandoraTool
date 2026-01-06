using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Contract.Archive;

public interface IArchiveComposer
{
    void Compose(ArchiveFile[] files, Stream dataStream, Stream listStream, Stream? tagStream = null);
}