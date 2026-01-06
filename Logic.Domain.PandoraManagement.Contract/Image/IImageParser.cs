using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.PandoraManagement.Contract.Image;

public interface IImageParser
{
    Image<Rgb24> Parse(byte[] data);
}