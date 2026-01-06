namespace Logic.Domain.PandoraManagement.Contract.Sound;

public interface ISoundParser
{
    byte[] Parse(byte[] data);
}