using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;
using Logic.Domain.PandoraManagement.Contract.Image;
using System.Buffers.Binary;
using Logic.Domain.PandoraManagement.Contract.Enums.Image;

namespace Logic.Domain.PandoraManagement.Image;

internal class ImageReader : IImageReader
{
    public ImageMetaData ReadMetaData(byte[] data)
    {
        int format = BinaryPrimitives.ReadInt32LittleEndian(data);

        ImageCompression compressionFormat = format is 2 ? ImageCompression.Pixel : ImageCompression.Lzss01;
        int x = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(0x8));
        int y = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(0xC));
        int width = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(0x10));
        int height = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(0x14));

        return new ImageMetaData
        {
            Compression = compressionFormat,
            X = x,
            Y = y,
            Width = width,
            Height = height,
        };
    }

    public ImageData Read(byte[] data)
    {
        return new ImageData
        {
            MetaData = ReadMetaData(data),
            Data = data[0x18..]
        };
    }
}