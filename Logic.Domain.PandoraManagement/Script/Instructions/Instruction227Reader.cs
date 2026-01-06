using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction227Reader
{
    protected override void ReadArguments(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, int endOffset)
    {
        ReadByte(arguments, data, ref offset, out int value);

        switch (value)
        {
            case 0:
                ReadVariableData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadStringData(arguments, data, ref offset);
                break;

            case 1:
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadStringData(arguments, data, ref offset);
                break;

            case 2:
            case 15:
            case 16:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadStringData(arguments, data, ref offset);
                break;

            case 3:
            case 13:
            case 14:
                ReadVariableData(arguments, data, ref offset);
                ReadStringData(arguments, data, ref offset);
                break;

            case 4:
            case 6:
            case 22:
                ReadVariableData(arguments, data, ref offset);
                break;

            case 7:
            case 8:
            case 9:
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                break;

            case 10:
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                break;

            case 11:
            case 12:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 17:
            case 18:
                ReadStringData(arguments, data, ref offset);
                ReadStringData(arguments, data, ref offset);
                break;

            case 20:
                ReadStringData(arguments, data, ref offset);
                break;

            case 23:
            case 26:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 24:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 25:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;
        }
    }
}