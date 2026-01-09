namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Image;

public class ImageData
{
    public required ImageMetaData MetaData { get; init; }
    public required byte[] Data { get; init; }
}