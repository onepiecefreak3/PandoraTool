using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;

namespace Logic.Domain.PandoraManagement.Contract.Image;

public interface IImageReader
{
    ImageData Read(byte[] data);
}