using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction209Calculator
{
    private static readonly int[] ValueCount = [9, 9, 11, 11, 10, 10, 12, 12, 11, 11, 13, 13, 14, 14, 16, 16, 11, 11, 12, 12, 14, 14, 14, 14, 15, 15, 14, 14, 10, 10, 8, 11, 11, 8, 8];

    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateInt32(instruction, 0, ref length, out int value);

        switch (value)
        {
            case >= 0 and < 35:
                int valueCount = ValueCount[value];

                for (var i = 0; i < valueCount; i++)
                    CalculateValueData(instruction, i + 1, ref length);
                break;

            case 35:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                CalculateValueData(instruction, 5, ref length);
                CalculateStringData(instruction, 6, ref length);

                if (instruction.Arguments.Length > 7)
                {
                    CalculateValueData(instruction, 7, ref length);
                    CalculateValueData(instruction, 8, ref length);
                    CalculateValueData(instruction, 9, ref length);
                    CalculateValueData(instruction, 10, ref length);
                }

                break;
        }
    }
}