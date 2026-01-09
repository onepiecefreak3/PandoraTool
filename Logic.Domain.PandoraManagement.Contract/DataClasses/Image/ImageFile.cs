using Logic.Domain.PandoraManagement.Contract.Enums.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Image;

public class ImageFile
{
    public required ImageCompression Compression { get; init; }
    public required Image<Bgr24> Image { get; init; }
}