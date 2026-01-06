using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction226Composer
{
    protected override void ComposeArguments(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset)
    {
        ComposeByte(instruction, 0, arguments, ref offset, out byte value);

        switch (value)
        {
            case 0:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                break;

            case 1:
            case 12:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                break;

            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 8:
                ComposeValueData(instruction, 1, arguments, ref offset);
                break;

            case 7:
            case 9:
            case 10:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                break;

            case 11:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeVariableData(instruction, 2, arguments, ref offset);
                ComposeVariableData(instruction, 3, arguments, ref offset);
                ComposeVariableData(instruction, 4, arguments, ref offset);
                break;
        }
    }
}