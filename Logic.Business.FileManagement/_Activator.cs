using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.FileManagement.Contract;
using Logic.Business.FileManagement.InternalContract;
using Logic.Business.FileManagement.InternalContract.Script;
using Logic.Business.FileManagement.Script;

namespace Logic.Business.FileManagement;

public class FileManagementActivator : IComponentActivator
{
    public void Activating()
    {
    }

    public void Activated()
    {
    }

    public void Deactivating()
    {
    }

    public void Deactivated()
    {
    }

    public void Register(ICoCoKernel kernel)
    {
        kernel.Register<IFileManagementWorkflow, FileManagementWorkflow>(ActivationScope.Unique);
        kernel.Register<IExtractFileWorkflow, ExtractFileWorkflow>(ActivationScope.Unique);
        kernel.Register<IInjectFileWorkflow, InjectFileWorkflow>(ActivationScope.Unique);

        kernel.Register<IInputFileProvider, InputFileProvider>(ActivationScope.Unique);

        kernel.Register<IPandoraScriptFileConverter, PandoraScriptFileConverter>(ActivationScope.Unique);
        kernel.Register<IPandoraCodeUnitConverter, PandoraCodeUnitConverter>(ActivationScope.Unique);

        kernel.Register<IConfigurationValidator, ConfigurationValidator>(ActivationScope.Unique);

        kernel.RegisterConfiguration<FileManagementConfiguration>();
    }

    public void AddMessageSubscriptions(IEventBroker broker)
    {
    }

    public void Configure(IConfigurator configurator)
    {
    }
}