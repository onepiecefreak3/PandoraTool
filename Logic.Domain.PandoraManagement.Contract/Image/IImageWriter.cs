using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;

namespace Logic.Domain.PandoraManagement.Contract.Image;

public interface IImageWriter
{
    void Write(ImageData data, Stream output);
}