using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.PandoraManagement.Contract.Image;

public interface IImageCompressor
{
    byte[] Compress(Image<Bgr24> image);
}