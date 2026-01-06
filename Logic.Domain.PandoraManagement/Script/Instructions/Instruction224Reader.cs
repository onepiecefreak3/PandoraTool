using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction224Reader
{
    protected override void ReadArguments(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, int endOffset)
    {
        ReadByte(arguments, data, ref offset, out int value);

        switch (value)
        {
            case 0:
            case 6:
            case 7:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 1:
            case 13:
                ReadValueData(arguments, data, ref offset);
                break;

            case 2:
                ReadValueData(arguments, data, ref offset);

                while (offset < endOffset)
                    ReadValueData(arguments, data, ref offset);
                break;

            case 3:
                ReadVariableData(arguments, data, ref offset);

                while (offset < endOffset)
                    ReadValueData(arguments, data, ref offset);
                break;

            case 4:
            case 5:
                ReadVariableData(arguments, data, ref offset);
                break;

            case 8:
            case 9:
            case 12:
            case 15:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 11:
                ReadValueData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                break;
        }
    }
}