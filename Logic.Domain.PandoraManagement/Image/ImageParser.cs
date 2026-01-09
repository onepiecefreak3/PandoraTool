using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;
using Logic.Domain.PandoraManagement.Contract.Image;
using Logic.Domain.PandoraManagement.Contract.Image.Compression;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.PandoraManagement.Image;

internal class ImageParser(IImageReader reader, IImageDecompressorFactory decompressorFactory) : IImageParser
{
    public ImageFile Parse(byte[] data)
    {
        ImageData imageData = reader.Read(data);
        IImageDecompressor decompressor = decompressorFactory.Get(imageData.MetaData.Compression);

        Image<Bgr24> image = decompressor.Decompress(imageData.Data, imageData.MetaData.Width, imageData.MetaData.Height);

        return new ImageFile
        {
            Compression = imageData.MetaData.Compression,
            X = imageData.MetaData.X,
            Y = imageData.MetaData.Y,
            Image = image
        };
    }
}