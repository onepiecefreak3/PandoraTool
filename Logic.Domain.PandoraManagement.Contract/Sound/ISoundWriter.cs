using Logic.Domain.PandoraManagement.Contract.DataClasses.Sound;

namespace Logic.Domain.PandoraManagement.Contract.Sound;

public interface ISoundWriter
{
    void Write(SoundData data, Stream output);
}