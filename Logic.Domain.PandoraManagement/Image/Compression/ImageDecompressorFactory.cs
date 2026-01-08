using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.PandoraManagement.Contract.Enums.Image;
using Logic.Domain.PandoraManagement.Contract.Image;
using Logic.Domain.PandoraManagement.InternalContract.Image;

namespace Logic.Domain.PandoraManagement.Image.Compression;

internal class ImageDecompressorFactory(ICoCoKernel kernel) : IImageDecompressorFactory
{
    public IImageDecompressor Get(ImageCompression compression)
    {
        switch (compression)
        {
            case ImageCompression.Pixel:
                return kernel.Get<IImageDecompressorPixel>();

            case ImageCompression.Lzss01:
                return kernel.Get<IImageDecompressorLzss01>();

            default:
                throw new InvalidOperationException($"Unknown image compression {compression}.");
        }
    }
}