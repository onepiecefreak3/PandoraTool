using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;

namespace Logic.Domain.PandoraManagement.Contract.Image;

public interface IImageReader
{
    ImageMetaData ReadMetaData(byte[] data);
    ImageData Read(byte[] data);
}