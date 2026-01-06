using Logic.Domain.PandoraManagement.Contract.Enums;

namespace Logic.Domain.PandoraManagement.Contract.Archive;

public interface IFileCompressor
{
    Stream Compress(Stream stream, FileCompression fileCompression);
}