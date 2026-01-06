using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction228Reader
{
    protected override void ReadArguments(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, int endOffset)
    {
        ReadByte(arguments, data, ref offset, out int value);

        switch (value)
        {
            case 0:
            case 1:
            case 7:
                ReadVariableData(arguments, data, ref offset);
                break;

            case 2:
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 3:
                ReadVariableData(arguments, data, ref offset);
                ReadStringData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 4:
                ReadVariableData(arguments, data, ref offset);
                ReadStringData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 8:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;
        }
    }
}