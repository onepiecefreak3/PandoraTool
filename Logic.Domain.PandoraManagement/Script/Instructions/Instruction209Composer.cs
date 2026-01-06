using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction209Composer
{
    private static readonly int[] ValueCount = [9, 9, 11, 11, 10, 10, 12, 12, 11, 11, 13, 13, 14, 14, 16, 16, 11, 11, 12, 12, 14, 14, 14, 14, 15, 15, 14, 14, 10, 10, 8, 11, 11, 8, 8];

    protected override void ComposeArguments(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset)
    {
        ComposeInt32(instruction, 0, arguments, ref offset, out int value);

        switch (value)
        {
            case >= 0 and < 35:
                int valueCount = ValueCount[value];

                for (var i = 0; i < valueCount; i++)
                    ComposeValueData(instruction, i + 1, arguments, ref offset);
                break;

            case 35:
                ComposeValueData(instruction, 1, arguments, ref offset);
                ComposeValueData(instruction, 2, arguments, ref offset);
                ComposeValueData(instruction, 3, arguments, ref offset);
                ComposeValueData(instruction, 4, arguments, ref offset);
                ComposeValueData(instruction, 5, arguments, ref offset);
                ComposeStringData(instruction, 6, arguments, ref offset);

                if (instruction.Arguments.Length > 7)
                {
                    ComposeValueData(instruction, 7, arguments, ref offset);
                    ComposeValueData(instruction, 8, arguments, ref offset);
                    ComposeValueData(instruction, 9, arguments, ref offset);
                    ComposeValueData(instruction, 10, arguments, ref offset);
                }

                break;
        }
    }
}