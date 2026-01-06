using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction225Reader
{
    protected override void ReadArguments(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, int endOffset)
    {
        ReadByte(arguments, data, ref offset, out int value);

        switch (value)
        {
            case 0:
            case 2:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 1:
            case 3:
                ReadVariableData(arguments, data, ref offset);
                ReadVariableData(arguments, data, ref offset);
                break;

            case 4:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;

            case 5:
                ReadValueData(arguments, data, ref offset);
                break;

            case 6:
                ReadVariableData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                break;
        }
    }
}