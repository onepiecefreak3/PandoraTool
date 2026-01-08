using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.EventBrokerage;
using CrossCutting.Core.Contract.Messages;
using Logic.Business.FileManagement.Contract;
using System.Text;
using PandoraTool;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

KernelLoader loader = new();
ICoCoKernel kernel = loader.Initialize();

var eventBroker = kernel.Get<IEventBroker>();
eventBroker.Raise(new InitializeApplicationMessage());

var mainLogic = kernel.Get<IFileManagementWorkflow>();
return mainLogic.Execute();
