using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;
using Logic.Domain.PandoraManagement.Contract.Enums.Image;

namespace Logic.Domain.PandoraManagement.Contract.Image;

public interface IImageReader
{
    ImageCompression ReadCompression(byte[] data);
    ImageData Read(byte[] data);
}