using Logic.Domain.PandoraManagement.Contract.Enums.Image;

namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Image;

public class ImageData
{
    public required ImageCompression CompressionType { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required byte[] Data { get; init; }
}