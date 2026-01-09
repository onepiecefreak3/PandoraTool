using Logic.Domain.PandoraManagement.Contract.Enums.Sound;

namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Sound;

public class SoundData
{
    public required SoundCompression Compression { get; init; }
    public required byte[] Data { get; init; }
}