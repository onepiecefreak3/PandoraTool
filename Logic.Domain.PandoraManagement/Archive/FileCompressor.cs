using Kompression.Contract;
using Logic.Domain.PandoraManagement.Compression;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.Enums;

namespace Logic.Domain.PandoraManagement.Archive;

internal class FileCompressor : IFileCompressor
{
    public Stream CompressStream(Stream stream, FileCompression fileCompression)
    {
        stream.Position = 0;

        switch (fileCompression)
        {
            case FileCompression.None:
                return stream;

            case FileCompression.Lzss01:
                ICompression compression = new Kompression.Configuration.CompressionConfigurationBuilder()
                    .Encode.With(() => new Nanako2Encoder())
                    .Build();

                var output = new MemoryStream();
                compression.Compress(stream, output);

                output.Position = 0;
                return output;

            default:
                throw new InvalidOperationException($"Unknown file compression {fileCompression}.");
        }
    }

    public byte[] CompressBytes(Stream stream, FileCompression fileCompression)
    {
        Stream compressedStream = CompressStream(stream, fileCompression);

        if (compressedStream is MemoryStream memoryStream)
            return memoryStream.ToArray();

        using var reader = new BinaryReader(compressedStream);
        return reader.ReadBytes((int)compressedStream.Length);
    }
}