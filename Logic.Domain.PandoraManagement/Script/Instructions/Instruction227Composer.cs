using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction227Composer
{
    protected override void ComposeArguments(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset)
    {
        ComposeByte(instruction, 0, arguments, ref offset, out byte value);

        switch (value)
        {
            case 0:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                ComposeStringData(instruction, 4, arguments, ref offset);
                break;

            case 1:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeVariableData(instruction, 2, arguments, ref offset);
                ComposeVariableData(instruction, 3, arguments, ref offset);
                ComposeVariableData(instruction, 4, arguments, ref offset);
                ComposeVariableData(instruction, 5, arguments, ref offset);
                ComposeVariableData(instruction, 6, arguments, ref offset);
                ComposeVariableData(instruction, 7, arguments, ref offset);
                ComposeVariableData(instruction, 8, arguments, ref offset);
                ComposeStringData(instruction, 9, arguments, ref offset);
                break;

            case 2:
            case 15:
            case 16:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                ComposeValueData(instruction, 4, arguments, ref offset);
                ComposeValueData(instruction, 5, arguments, ref offset);
                ComposeValueData(instruction, 6, arguments, ref offset);
                ComposeValueData(instruction, 7, arguments, ref offset);
                ComposeValueData(instruction, 8, arguments, ref offset);
                ComposeStringData(instruction, 9, arguments, ref offset);
                break;

            case 3:
            case 13:
            case 14:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeStringData(instruction, 2, arguments, ref offset);
                break;

            case 4:
            case 6:
            case 22:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                break;

            case 7:
            case 8:
            case 9:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeVariableData(instruction, 2, arguments, ref offset);
                ComposeVariableData(instruction, 3, arguments, ref offset);
                ComposeVariableData(instruction, 4, arguments, ref offset);
                ComposeVariableData(instruction, 5, arguments, ref offset);
                ComposeVariableData(instruction, 6, arguments, ref offset);
                ComposeVariableData(instruction, 7, arguments, ref offset);
                ComposeVariableData(instruction, 8, arguments, ref offset);
                break;

            case 10:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeVariableData(instruction, 2, arguments, ref offset);
                break;

            case 11:
            case 12:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                break;

            case 17:
            case 18:
                ComposeStringData(instruction, 1, arguments, ref offset);
                ComposeStringData(instruction, 2, arguments, ref offset);
                break;

            case 20:
                ComposeStringData(instruction, 1, arguments, ref offset);
                break;

            case 23:
            case 26:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                break;

            case 24:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                ComposeValueData(instruction, 4, arguments, ref offset);
                break;

            case 25:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                ComposeValueData(instruction, 4, arguments, ref offset);
                ComposeValueData(instruction, 5, arguments, ref offset);
                break;
        }
    }
}