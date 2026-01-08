using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;
using Logic.Domain.PandoraManagement.Contract.Image;

namespace Logic.Domain.PandoraManagement.Image;

internal class ImageComposer(IImageWriter writer, IImageCompressorFactory compressorFactory) : IImageComposer
{
    public void Compose(ImageFile file, Stream output)
    {
        IImageCompressor compressor = compressorFactory.Get(file.CompressionType);
        byte[] data = compressor.Compress(file.Image);

        var imageData = new ImageData
        {
            CompressionType = file.CompressionType,
            Width = file.Image.Width,
            Height = file.Image.Height,
            Data = data
        };

        writer.Write(imageData, output);
    }
}