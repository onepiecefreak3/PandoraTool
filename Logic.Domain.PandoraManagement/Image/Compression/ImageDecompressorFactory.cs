using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.PandoraManagement.Contract.Enums.Image;
using Logic.Domain.PandoraManagement.Contract.Image.Compression;
using Logic.Domain.PandoraManagement.InternalContract.Image;

namespace Logic.Domain.PandoraManagement.Image.Compression;

internal class ImageDecompressorFactory(ICoCoKernel kernel) : IImageDecompressorFactory
{
    public IImageDecompressor Get(ImageCompression compression)
    {
        return compression switch
        {
            ImageCompression.Pixel => kernel.Get<IImageDecompressorPixel>(),
            ImageCompression.Lzss01 => kernel.Get<IImageDecompressorLzss01>(),
            _ => throw new InvalidOperationException($"Unknown image compression {compression}.")
        };
    }
}