using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.PandoraManagement.Contract.Image.Compression;

public interface IImageCompressor
{
    byte[] Compress(Image<Bgr24> image);
}