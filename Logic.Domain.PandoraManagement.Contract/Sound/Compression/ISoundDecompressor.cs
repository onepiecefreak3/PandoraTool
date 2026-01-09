namespace Logic.Domain.PandoraManagement.Contract.Sound.Compression;

public interface ISoundDecompressor
{
    byte[] Decompress(byte[] data);
}