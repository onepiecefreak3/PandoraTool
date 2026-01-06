using Logic.Business.FileManagement.DataClasses;

namespace Logic.Business.FileManagement.InternalContract;

internal interface IInputFileProvider
{
    InputFilePathData GetPaths(string inputPath);
}