using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction226Reader
{
    protected override void ReadArguments(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, int endOffset)
    {
        ReadByte(arguments, data, ref offset, out int value);

        switch (value)
        {
            case 0:
                ReadVariableData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 1:
            case 12:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 8:
                ReadValueData(arguments, data, ref offset);
                break;

            case 7:
            case 9:
            case 10:
                ReadVariableData(arguments, data, ref offset);
                break;

            case 11:
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                break;
        }
    }
}