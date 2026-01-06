using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction230Composer
{
    protected override void ComposeArguments(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset)
    {
        ComposeByte(instruction, 0, arguments, ref offset, out byte value);

        switch (value)
        {
            case 2:
            case 5:
                ComposeVariableData(instruction, 1, arguments, ref offset);
                break;

            case 8:
            case 9:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                ComposeValueData(instruction, 4, arguments, ref offset);
                break;
        }
    }
}