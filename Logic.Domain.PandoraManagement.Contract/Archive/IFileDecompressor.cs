using Logic.Domain.PandoraManagement.Contract.Enums;

namespace Logic.Domain.PandoraManagement.Contract.Archive;

public interface IFileDecompressor
{
    Stream DecompressStream(Stream stream, FileCompression fileCompression);
    byte[] DecompressBytes(Stream stream, FileCompression fileCompression);
}