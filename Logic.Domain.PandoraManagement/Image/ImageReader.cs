using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;
using Logic.Domain.PandoraManagement.Contract.Image;
using System.Buffers.Binary;
using Logic.Domain.PandoraManagement.Contract.Enums.Image;

namespace Logic.Domain.PandoraManagement.Image;

internal class ImageReader : IImageReader
{
    public ImageData Read(byte[] data)
    {
        int compressionFormat = BinaryPrimitives.ReadInt32LittleEndian(data);

        int width = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(0x10));
        int height = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(0x14));

        return new ImageData
        {
            CompressionType = compressionFormat is 2 ? ImageCompression.Pixel : ImageCompression.Lzss01,
            Width = width,
            Height = height,
            Data = data[0x18..]
        };
    }
}