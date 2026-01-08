using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.Enums;
using Logic.Domain.PandoraManagement.InternalContract.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.PandoraManagement.Image.Compression;

internal class ImageDecompressorLzss01(IFileDecompressor decompressor) : IImageDecompressorLzss01
{
    public Image<Bgr24> Decompress(byte[] data, int width, int height)
    {
        byte[] imageData = decompressor.DecompressBytes(new MemoryStream(data), FileCompression.Lzss01);

        return SixLabors.ImageSharp.Image.LoadPixelData<Bgr24>(imageData, width, height);
    }
}