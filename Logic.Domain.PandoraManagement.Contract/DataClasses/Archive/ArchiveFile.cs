using Logic.Domain.PandoraManagement.Contract.Enums;

namespace Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;

public class ArchiveFile
{
    public required FileCompression Compression { get; init; }
    public required FileType Type { get; init; }
    public required string Name { get; init; }
    public required Stream Data { get; set; }
    public int? Attributes { get; set; }
    public DateTime? DateTime { get; init; }
}