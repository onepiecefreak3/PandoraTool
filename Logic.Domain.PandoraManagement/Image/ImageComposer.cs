using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;
using Logic.Domain.PandoraManagement.Contract.Image;
using Logic.Domain.PandoraManagement.Contract.Image.Compression;

namespace Logic.Domain.PandoraManagement.Image;

internal class ImageComposer(IImageWriter writer, IImageCompressorFactory compressorFactory) : IImageComposer
{
    public void Compose(ImageFile file, Stream output)
    {
        IImageCompressor compressor = compressorFactory.Get(file.Compression);
        byte[] data = compressor.Compress(file.Image);

        var imageData = new ImageData
        {
            MetaData = new ImageMetaData
            {
                Compression = file.Compression,
                X = file.X,
                Y = file.Y,
                Width = file.Image.Width,
                Height = file.Image.Height
            },
            Data = data
        };

        writer.Write(imageData, output);
    }
}