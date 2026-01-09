using Logic.Domain.PandoraManagement.Contract.Enums.Image;

namespace Logic.Domain.PandoraManagement.Contract.Image.Compression;

public interface IImageDecompressorFactory
{
    IImageDecompressor Get(ImageCompression compression);
}