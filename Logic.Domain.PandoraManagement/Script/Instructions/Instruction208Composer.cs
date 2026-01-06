using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction208Composer
{
    protected override void ComposeArguments(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset)
    {
        ComposeInt32(instruction, 0, arguments, ref offset, out int value);

        if (value != 0)
            return;

        ComposeValueData(instruction, 1, arguments, ref offset);
        ComposeValueData(instruction, 2, arguments, ref offset);
        ComposeValueData(instruction, 3, arguments, ref offset);
        ComposeValueData(instruction, 4, arguments, ref offset);
        ComposeValueData(instruction, 5, arguments, ref offset);
        ComposeStringData(instruction, 6, arguments, ref offset);

        if (instruction.Arguments.Length > 7)
        {
            ComposeVariableData(instruction, 7, arguments, ref offset);
        }
    }
}