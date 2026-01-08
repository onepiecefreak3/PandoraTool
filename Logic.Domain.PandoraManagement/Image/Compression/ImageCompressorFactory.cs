using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.PandoraManagement.Contract.Enums.Image;
using Logic.Domain.PandoraManagement.Contract.Image;
using Logic.Domain.PandoraManagement.InternalContract.Image;

namespace Logic.Domain.PandoraManagement.Image.Compression;

internal class ImageCompressorFactory(ICoCoKernel kernel) : IImageCompressorFactory
{
    public IImageCompressor Get(ImageCompression compression)
    {
        switch (compression)
        {
            case ImageCompression.Pixel:
                return kernel.Get<IImageCompressorPixel>();

            case ImageCompression.Lzss01:
                return kernel.Get<IImageCompressorLzss01>();

            default:
                throw new InvalidOperationException($"Unknown image compression {compression}.");
        }
    }
}