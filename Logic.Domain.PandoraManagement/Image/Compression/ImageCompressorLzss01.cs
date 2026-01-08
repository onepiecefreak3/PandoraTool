using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.Enums;
using Logic.Domain.PandoraManagement.InternalContract.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.PandoraManagement.Image.Compression;

internal class ImageCompressorLzss01(IFileCompressor compressor) : IImageCompressorLzss01
{
    public byte[] Compress(Image<Bgr24> image)
    {
        var data = new byte[image.Width * image.Height * 3];

        image.CopyPixelDataTo(data);

        return compressor.CompressBytes(new MemoryStream(data), FileCompression.Lzss01);
    }
}