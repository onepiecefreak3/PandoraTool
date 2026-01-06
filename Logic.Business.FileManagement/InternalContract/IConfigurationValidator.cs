namespace Logic.Business.FileManagement.InternalContract;

internal interface IConfigurationValidator
{
    void Validate(FileManagementConfiguration config);
}