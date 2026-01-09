using Logic.Domain.PandoraManagement.Contract.Image;
using Komponent.IO;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;
using Logic.Domain.PandoraManagement.Contract.Enums.Image;

namespace Logic.Domain.PandoraManagement.Image;

internal class ImageWriter : IImageWriter
{
    public void Write(ImageData data, Stream output)
    {
        using var writer = new BinaryWriterX(output, true);

        writer.Write(data.MetaData.Compression is ImageCompression.Pixel ? 2 : 0);
        writer.Write(-1);
        writer.Write(data.MetaData.X);
        writer.Write(data.MetaData.Y);
        writer.Write(data.MetaData.Width);
        writer.Write(data.MetaData.Height);
        writer.Write(data.Data);

        output.Position = 0;
    }
}