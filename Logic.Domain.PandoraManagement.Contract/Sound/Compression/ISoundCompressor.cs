namespace Logic.Domain.PandoraManagement.Contract.Sound.Compression;

public interface ISoundCompressor
{
    byte[] Compress(byte[] data);
}