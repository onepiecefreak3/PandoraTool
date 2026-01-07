using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction770Reader
{
    protected override void ReadArguments(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, int endOffset)
    {
        ReadByte(arguments, data, ref offset, out int value);

        switch (value)
        {
            case 0:
                ReadVariableData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 1:
                ReadValueData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;
        }
    }
}