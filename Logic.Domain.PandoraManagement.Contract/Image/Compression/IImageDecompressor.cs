using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.PandoraManagement.Contract.Image.Compression;

public interface IImageDecompressor
{
    Image<Bgr24> Decompress(byte[] data, int width, int height);
}