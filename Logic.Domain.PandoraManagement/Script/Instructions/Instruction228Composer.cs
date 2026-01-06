using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction228Composer
{
    protected override void ComposeArguments(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset)
    {
        ComposeByte(instruction, 0, arguments, ref offset, out byte value);

        switch (value)
        {
            case 0:
            case 1:
            case 7:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                break;

            case 2:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeVariableData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                break;

            case 3:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeStringData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                ComposeValueData(instruction, 4, arguments, ref offset);
                ComposeValueData(instruction, 5, arguments, ref offset);
                ComposeValueData(instruction, 6, arguments, ref offset);
                ComposeValueData(instruction, 7, arguments, ref offset);
                ComposeValueData(instruction, 8, arguments, ref offset);
                ComposeValueData(instruction, 9, arguments, ref offset);
                ComposeValueData(instruction, 10, arguments, ref offset);
                break;

            case 4:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                ComposeStringData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                ComposeValueData(instruction, 4, arguments, ref offset);
                break;

            case 8:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                ComposeValueData(instruction, 4, arguments, ref offset);
                break;
        }
    }
}