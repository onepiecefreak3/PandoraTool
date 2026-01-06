using Kompression.Contract;
using Logic.Domain.PandoraManagement.Compression;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.Enums;

namespace Logic.Domain.PandoraManagement.Archive;

internal class FileDecompressor : IFileDecompressor
{
    public Stream DecompressStream(Stream stream, FileCompression fileCompression)
    {
        stream.Position = 0;

        switch (fileCompression)
        {
            case FileCompression.None:
                return stream;

            case FileCompression.Lzss01:
                ICompression compression = new Kompression.Configuration.CompressionConfigurationBuilder()
                    .Decode.With(() => new Nanako2Decoder())
                    .Build();

                var output = new MemoryStream();
                compression.Decompress(stream, output);

                return output;

            default:
                throw new InvalidOperationException($"Unknown file compression {fileCompression}.");
        }
    }

    public byte[] DecompressBytes(Stream stream, FileCompression fileCompression)
    {
        Stream decompressedStream = DecompressStream(stream, fileCompression);

        if (decompressedStream is MemoryStream memoryStream)
            return memoryStream.ToArray();

        using var reader = new BinaryReader(decompressedStream);
        return reader.ReadBytes((int)decompressedStream.Length);
    }
}