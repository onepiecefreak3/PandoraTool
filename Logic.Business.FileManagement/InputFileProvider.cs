using Logic.Business.FileManagement.InternalContract;
using Logic.Business.FileManagement.DataClasses;

namespace Logic.Business.FileManagement;

internal class InputFileProvider : IInputFileProvider
{
    public InputFilePathData GetPaths(string inputPath)
    {
        string extension = Path.GetExtension(inputPath);

        if (string.IsNullOrWhiteSpace(extension))
            throw new InvalidOperationException($"File '{Path.GetFullPath(inputPath)}' has no extension.");

        if (extension.Equals(".dat", StringComparison.OrdinalIgnoreCase))
        {
            return new InputFilePathData
            {
                DatFilePath = inputPath,
                LstFilePath = Path.ChangeExtension(inputPath, ".LST"),
                TagFilePath = Path.ChangeExtension(inputPath, ".TAG")
            };
        }
        
        if (extension.Equals(".lst", StringComparison.OrdinalIgnoreCase))
        {
            return new InputFilePathData
            {
                DatFilePath = Path.ChangeExtension(inputPath, ".DAT"),
                LstFilePath = inputPath,
                TagFilePath = Path.ChangeExtension(inputPath, ".TAG")
            };
        }

        if (extension.Equals(".tag", StringComparison.OrdinalIgnoreCase))
        {
            return new InputFilePathData
            {
                DatFilePath = Path.ChangeExtension(inputPath, ".DAT"),
                LstFilePath = Path.ChangeExtension(inputPath, ".LST"),
                TagFilePath = inputPath
            };
        }

        throw new InvalidOperationException($"File '{Path.GetFullPath(inputPath)}' has an unknown extension.");
    }
}