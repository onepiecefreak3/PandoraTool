using Logic.Domain.PandoraManagement.Contract.DataClasses.Sound;

namespace Logic.Domain.PandoraManagement.Contract.Sound;

public interface ISoundComposer
{
    void Compose(SoundFile file, Stream output);
}