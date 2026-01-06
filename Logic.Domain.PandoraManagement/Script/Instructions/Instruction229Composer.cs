using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction229Composer
{
    protected override void ComposeArguments(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset)
    {
        ComposeByte(instruction, 0, arguments, ref offset, out byte value);

        switch (value)
        {
            case 3:
            case 4:
                ComposeValueData(instruction, 1, arguments, ref offset);
                break;

            case 5:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                break;
        }
    }
}