using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;

namespace Logic.Domain.PandoraManagement.Contract.Image;

public interface IImageComposer
{
    void Compose(ImageFile file, Stream output);
}