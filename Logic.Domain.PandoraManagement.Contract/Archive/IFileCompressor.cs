using Logic.Domain.PandoraManagement.Contract.Enums;

namespace Logic.Domain.PandoraManagement.Contract.Archive;

public interface IFileCompressor
{
    Stream CompressStream(Stream stream, FileCompression fileCompression);
    byte[] CompressBytes(Stream stream, FileCompression fileCompression);
}