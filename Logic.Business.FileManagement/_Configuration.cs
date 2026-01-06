using CrossCutting.Core.Contract.Configuration.DataClasses;

namespace Logic.Business.FileManagement;

public class FileManagementConfiguration
{
    [ConfigMap("CommandLine", ["h", "help"])]
    public virtual bool ShowHelp { get; set; }

    [ConfigMap("CommandLine", ["o", "operation"])]
    public virtual string? Operation { get; set; }

    [ConfigMap("CommandLine", ["f", "file"])]
    public virtual string? InputPath { get; set; }

    [ConfigMap("CommandLine", ["fi", "insert"])]
    public virtual string? InsertPath { get; set; }
}