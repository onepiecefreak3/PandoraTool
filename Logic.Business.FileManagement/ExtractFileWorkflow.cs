using Logic.Business.FileManagement.DataClasses;
using Logic.Business.FileManagement.InternalContract;
using Logic.Business.FileManagement.InternalContract.Script;
using Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;
using Logic.Domain.CodeAnalysis.Contract.Pandora;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Archive;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Enums;
using Logic.Domain.PandoraManagement.Contract.Image;
using Logic.Domain.PandoraManagement.Contract.Script;
using Logic.Domain.PandoraManagement.Contract.Sound;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Business.FileManagement;

internal class ExtractFileWorkflow(
    FileManagementConfiguration config,
    IArchiveParser archiveParser,
    IImageParser imageParser,
    ISoundParser soundParser,
    IScriptParser scriptParser,
    IPandoraScriptFileConverter scriptConverter,
    IPandoraScriptWhitespaceNormalizer whitespaceNormalizer,
    IPandoraScriptComposer scriptComposer,
    IFileDecompressor fileDecompressor,
    IInputFileProvider pathProvider) : IExtractFileWorkflow
{
    public void Extract()
    {
        InputFilePathData paths = pathProvider.GetPaths(config.InputPath!);

        using Stream dataStream = File.OpenRead(paths.DatFilePath);
        using Stream listStream = File.OpenRead(paths.LstFilePath);

        Stream? tagStream = null;
        if (File.Exists(paths.TagFilePath))
            tagStream = File.OpenRead(paths.TagFilePath);

        string? outputDirectory = Path.GetDirectoryName(paths.DatFilePath);
        outputDirectory = Path.Combine(outputDirectory ?? string.Empty, "extracted");

        Directory.CreateDirectory(outputDirectory);

        ArchiveFile[] files = archiveParser.Parse(dataStream, listStream, tagStream);

        for (var i = 0; i < files.Length; i++)
        {
            Console.Write($"Extracted files {i}/{files.Length}...\r");

            ArchiveFile file = files[i];
            byte[] fileData = fileDecompressor.DecompressBytes(file.Data, file.Compression);

            string newFilePath = Path.Combine(outputDirectory, file.Name);

            switch (file.Type)
            {
                case FileType.Image:
                    newFilePath = Path.ChangeExtension(newFilePath, ".png");

                    Image<Rgb24> image = imageParser.Parse(fileData);
                    image.SaveAsPng(newFilePath);
                    break;

                case FileType.Sound:
                    newFilePath = Path.ChangeExtension(newFilePath, ".wav");

                    byte[] waveData = soundParser.Parse(fileData);
                    File.WriteAllBytes(newFilePath, waveData);
                    break;

                case FileType.Script:
                    File.WriteAllBytes(newFilePath, fileData);

                    newFilePath = Path.ChangeExtension(newFilePath, ".txt");

                    ScriptInstruction[] instructions = scriptParser.Parse(fileData);
                    CodeUnitSyntax codeUnit = scriptConverter.CreateCodeUnit(instructions);

                    whitespaceNormalizer.NormalizeCodeUnit(codeUnit);
                    string scriptText = scriptComposer.ComposeCodeUnit(codeUnit);

                    File.WriteAllText(newFilePath, scriptText);
                    break;

                case FileType.Binary:
                    File.WriteAllBytes(newFilePath, fileData);
                    break;
            }
        }

        Console.WriteLine($"Extracted files {files.Length}/{files.Length}... Ok");
    }
}