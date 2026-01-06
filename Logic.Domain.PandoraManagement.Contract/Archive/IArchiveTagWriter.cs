using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

namespace Logic.Domain.PandoraManagement.Contract.Archive;

public interface IArchiveTagWriter
{
    void Write(TagEntry[] tags, Stream tagStream);
}