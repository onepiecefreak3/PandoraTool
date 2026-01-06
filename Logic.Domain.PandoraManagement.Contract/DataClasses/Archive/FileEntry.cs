namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

public class FileEntry
{
    public required string FileName { get; init; }
    public required int Offset { get; init; }
    public required int Size { get; init; }
}