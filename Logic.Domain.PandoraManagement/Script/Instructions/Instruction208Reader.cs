using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction208Reader
{
    protected override void ReadArguments(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, int endOffset)
    {
        ReadInt32(arguments, data, ref offset, out int value);

        if (value != 0)
            return;

        ReadValueData(arguments, data, ref offset);
        ReadValueData(arguments, data, ref offset);
        ReadValueData(arguments, data, ref offset);
        ReadValueData(arguments, data, ref offset);
        ReadValueData(arguments, data, ref offset);
        ReadStringData(arguments, data, ref offset);

        if (offset < endOffset)
        {
            ReadVariableData(arguments, data, ref offset);
        }
    }
}