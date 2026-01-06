using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction224Calculator
{
    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateByte(instruction, 0, ref length, out byte value);

        switch (value)
        {
            case 0:
            case 6:
            case 7:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                break;

            case 1:
            case 13:
                CalculateValueData(instruction, 1, ref length);
                break;

            case 2:
                CalculateValueData(instruction, 1, ref length);

                for (var i = 2; i < instruction.Arguments.Length; i++)
                    CalculateValueData(instruction, i, ref length);
                break;

            case 3:
                CalculateVariableData(instruction, 1, ref length);

                for (var i = 2; i < instruction.Arguments.Length; i++)
                    CalculateValueData(instruction, i, ref length);
                break;

            case 4:
            case 5:
                CalculateVariableData(instruction, 1, ref length);
                break;

            case 8:
            case 9:
            case 12:
            case 15:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                break;

            case 11:
                CalculateValueData(instruction, 1, ref length);
                CalculateVariableData(instruction, 2, ref length);
                CalculateVariableData(instruction, 3, ref length);
                CalculateVariableData(instruction, 4, ref length);
                CalculateVariableData(instruction, 5, ref length);
                break;
        }
    }
}