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

        writer.Write(data.CompressionType is ImageCompression.Pixel ? 2 : 0);
        writer.Write(-1);

        output.Position += 8;
        writer.Write(data.Width);
        writer.Write(data.Height);
        writer.Write(data.Data);

        output.Position = 0;
    }
}