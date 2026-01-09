using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.PandoraManagement.Contract.Enums.Image;
using Logic.Domain.PandoraManagement.Contract.Image.Compression;
using Logic.Domain.PandoraManagement.InternalContract.Image;

namespace Logic.Domain.PandoraManagement.Image.Compression;

internal class ImageCompressorFactory(ICoCoKernel kernel) : IImageCompressorFactory
{
    public IImageCompressor Get(ImageCompression compression)
    {
        return compression switch
        {
            ImageCompression.Pixel => kernel.Get<IImageCompressorPixel>(),
            ImageCompression.Lzss01 => kernel.Get<IImageCompressorLzss01>(),
            _ => throw new InvalidOperationException($"Unknown image compression {compression}.")
        };
    }
}