using Logic.Business.FileManagement.Contract;
using Logic.Business.FileManagement.InternalContract;

namespace Logic.Business.FileManagement;

internal class FileManagementWorkflow(
    FileManagementConfiguration config,
    IConfigurationValidator configValidator,
    IExtractFileWorkflow extractWorkflow,
    IInjectFileWorkflow injectWorkflow)
    : IFileManagementWorkflow
{
    public int Execute()
    {
        if (config.ShowHelp || Environment.GetCommandLineArgs().Length <= 0)
        {
            PrintHelp();
            return 0;
        }

        if (!TryValidateConfig())
        {
            PrintHelp();
            return 0;
        }

        switch (config.Operation)
        {
            case "e":
                extractWorkflow.Extract();
                break;

            case "i":
                injectWorkflow.Inject();
                break;
        }

        return 0;
    }

    private bool TryValidateConfig()
    {
        try
        {
            configValidator.Validate(config);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine();

            return false;
        }

        return true;
    }

    private void PrintHelp()
    {
        Console.WriteLine("Following commands exist:");
        Console.WriteLine("  -h, --help\t\tShows this help message.");
        Console.WriteLine("  -o, --operation\tThe operation to process the file with.");
        Console.WriteLine("    Possible operations are: e for extraction, i for injection");
        Console.WriteLine("    Hint: Operation 'i' only injects already existing files");
        Console.WriteLine("  -f, --file\t\tThe file to extract/inject into.");
        Console.WriteLine("  -fi, --insert\t\tThe directory of files to inject. Only applies for operation 'i'.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine($"\tExtract a .DAT file from a Pandora game: {Environment.ProcessPath} -o e -f Path/To/FILE.DAT");
        Console.WriteLine($"\tExtract a .LST file from a Pandora game: {Environment.ProcessPath} -o e -f Path/To/FILE.LST");
        Console.WriteLine($"\tInject files to a Pandora game: {Environment.ProcessPath} -o i -f Path/To/FILE.DAT -fi Path/To/Edited/Files");
        Console.WriteLine($"\tInject files to a Pandora game: {Environment.ProcessPath} -o i -f Path/To/FILE.LST -fi Path/To/Edited/Files");
    }
}