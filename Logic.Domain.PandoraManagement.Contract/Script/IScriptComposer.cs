using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Contract.Script;

public interface IScriptComposer
{
    byte[] Compose(ScriptInstruction[] instructions);
}