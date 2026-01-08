using Logic.Business.FileManagement.DataClasses;
using Logic.Business.FileManagement.InternalContract;
using Logic.Business.FileManagement.InternalContract.Script;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.CodeAnalysis.Contract.Pandora;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Image;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Enums;
using Logic.Domain.PandoraManagement.Contract.Image;
using Logic.Domain.PandoraManagement.Contract.Script;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Business.FileManagement;

internal class InjectFileWorkflow(
    FileManagementConfiguration config,
    IInputFileProvider pathProvider,
    IArchiveParser archiveParser,
    IArchiveComposer archiveComposer,
    IImageComposer imageComposer,
    IImageReader imageReader,
    IScriptComposer scriptComposer,
    IPandoraCodeUnitConverter codeUnitConverter,
    IPandoraScriptParser scriptParser,
    IFileDecompressor fileDecompressor,
    IFileCompressor fileCompressor) : IInjectFileWorkflow
{
    public void Inject()
    {
        InputFilePathData paths = pathProvider.GetPaths(config.InputPath!);

        Stream dataStream = File.OpenRead(paths.DatFilePath);
        Stream listStream = File.OpenRead(paths.LstFilePath);

        Stream? tagStream = null;
        if (File.Exists(paths.TagFilePath))
            tagStream = File.OpenRead(paths.TagFilePath);

        ArchiveFile[] files = archiveParser.Parse(dataStream, listStream, tagStream);

        InjectFiles(files);

        string tempDataPath = paths.DatFilePath + ".tmp";
        string tempListPath = paths.LstFilePath + ".tmp";

        string? tempTagPath = null;
        if (tagStream is not null)
            tempTagPath = paths.TagFilePath + ".tmp";

        SaveFiles(files, tempDataPath, tempListPath, tempTagPath);

        dataStream.Close();
        listStream.Close();
        tagStream?.Close();

        File.Replace(tempDataPath, paths.DatFilePath, null);
        File.Replace(tempListPath, paths.LstFilePath, null);

        if (tempTagPath is not null)
            File.Replace(tempTagPath, paths.TagFilePath, null);
    }

    private void InjectFiles(ArchiveFile[] files)
    {
        for (var i = 0; i < files.Length; i++)
        {
            Console.Write($"Inject files {i}/{files.Length}...\r");

            ArchiveFile file = files[i];

            string filePath = GetInjectFilePath(file);

            if (!File.Exists(filePath))
                continue;

            Stream newFileStream;
            switch (file.Type)
            {
                case FileType.Script:
                    CodeUnitSyntax codeUnit = scriptParser.ParseCodeUnit(File.ReadAllText(filePath));
                    ScriptInstruction[] instructions = codeUnitConverter.CreateInstructions(codeUnit);

                    byte[] scriptData = scriptComposer.Compose(instructions);

                    newFileStream = new MemoryStream(scriptData);
                    break;

                case FileType.Image:
                    byte[] fileData = fileDecompressor.DecompressBytes(file.Data, file.Compression);
                    ImageData imageData = imageReader.Read(fileData);

                    var imageFile = new ImageFile
                    {
                        CompressionType = imageData.CompressionType,
                        Image = Image.Load<Bgr24>(filePath)
                    };

                    newFileStream = new MemoryStream();
                    imageComposer.Compose(imageFile, newFileStream);
                    break;

                case FileType.Binary:
                    newFileStream = File.OpenRead(filePath);
                    break;

                case FileType.Sound:
                default:
                    continue;
            }

            file.Data = fileCompressor.CompressStream(newFileStream, file.Compression);
        }

        Console.WriteLine($"Inject files {files.Length}/{files.Length}... Ok");
    }

    private void SaveFiles(ArchiveFile[] files, string tempDataPath, string tempListPath, string? tempTagPath)
    {
        using Stream newDataStream = File.Create(tempDataPath);
        using Stream newListStream = File.Create(tempListPath);

        Stream? newTagStream = null;
        if (tempTagPath is not null)
            newTagStream = File.Create(tempTagPath);

        Console.Write("Save files... ");

        archiveComposer.Compose(files, newDataStream, newListStream, newTagStream);

        Console.WriteLine("Ok");

        newTagStream?.Close();
    }

    private string GetInjectFilePath(ArchiveFile file)
    {
        string filePath = Path.Combine(config.InsertPath!, file.Name);

        return file.Type switch
        {
            FileType.Image => Path.ChangeExtension(filePath, ".png"),
            FileType.Sound => Path.ChangeExtension(filePath, ".wav"),
            FileType.Script => Path.ChangeExtension(filePath, ".txt"),
            _ => filePath
        };
    }
}