using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction224Composer
{
    protected override void ComposeArguments(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset)
    {
        ComposeByte(instruction, 0, arguments, ref offset, out byte value);

        switch (value)
        {
            case 0:
            case 6:
            case 7:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                break;

            case 1:
            case 13:
                ComposeValueData(instruction, 1, arguments, ref offset);
                break;

            case 2:
                ComposeValueData(instruction, 1, arguments, ref offset);

                for (var i = 2; i < instruction.Arguments.Length; i++)
                    ComposeValueData(instruction, i, arguments, ref offset);
                break;

            case 3:
                ComposeVariableData(instruction, 1, arguments, ref offset);

                for (var i = 2; i < instruction.Arguments.Length; i++)
                    ComposeValueData(instruction, i, arguments, ref offset);
                break;

            case 4:
            case 5:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                break;

            case 8:
            case 9:
            case 12:
            case 15:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                break;

            case 11:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeVariableData(instruction, 2, arguments, ref offset);
                ComposeVariableData(instruction, 3, arguments, ref offset);
                ComposeVariableData(instruction, 4, arguments, ref offset);
                ComposeVariableData(instruction, 5, arguments, ref offset);
                break;
        }
    }
}