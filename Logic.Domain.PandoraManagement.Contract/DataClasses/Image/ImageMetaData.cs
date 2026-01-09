using Logic.Domain.PandoraManagement.Contract.Enums.Image;

namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Image;

public class ImageMetaData
{
    public required ImageCompression Compression { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
}