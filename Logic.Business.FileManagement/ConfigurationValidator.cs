using Logic.Business.FileManagement.DataClasses;
using Logic.Business.FileManagement.InternalContract;

namespace Logic.Business.FileManagement;

internal class ConfigurationValidator(IInputFileProvider pathProvider) : IConfigurationValidator
{
    public void Validate(FileManagementConfiguration config)
    {
        if (config.ShowHelp)
            return;

        ValidateOperation(config);
        ValidateFilePath(config);
        ValidateInsertPath(config);
    }

    private void ValidateOperation(FileManagementConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config.Operation))
            throw new InvalidOperationException("No operation was specified. Specify an operation by using the -o argument.");

        if (config.Operation is not "e" and not "i")
            throw new InvalidOperationException($"Operation '{config.Operation}' is invalid. Valid operations are 'e' and 'i'.");
    }

    private void ValidateFilePath(FileManagementConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config.InputPath))
            throw new InvalidOperationException("No file to process was specified. Specify a file by using the -f argument.");

        InputFilePathData paths = pathProvider.GetPaths(config.InputPath);

        if (!File.Exists(paths.DatFilePath))
            throw new InvalidOperationException($"Data file '{Path.GetFullPath(paths.DatFilePath)}' was not found.");

        if (!File.Exists(paths.LstFilePath))
            throw new InvalidOperationException($"List file '{Path.GetFullPath(paths.LstFilePath)}' was not found.");
    }

    private void ValidateInsertPath(FileManagementConfiguration config)
    {
        if (config.Operation is not "i")
            return;

        if (string.IsNullOrWhiteSpace(config.InsertPath))
            throw new InvalidOperationException("No directory to inject from was specified. Specify a directory by using the -fi argument.");

        if (!Directory.Exists(config.InsertPath))
            throw new InvalidOperationException($"Directory '{config.InsertPath}' was not found.");
    }
}