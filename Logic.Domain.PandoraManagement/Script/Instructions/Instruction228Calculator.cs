using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction228Calculator
{
    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateByte(instruction, 0, ref length, out byte value);

        switch (value)
        {
            case 0:
            case 1:
            case 7:
                CalculateVariableData(instruction, 1, ref length);
                break;

            case 2:
                CalculateVariableData(instruction, 1, ref length);
                CalculateVariableData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                break;

            case 3:
                CalculateVariableData(instruction, 1, ref length);
                CalculateStringData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                CalculateValueData(instruction, 5, ref length);
                CalculateValueData(instruction, 6, ref length);
                CalculateValueData(instruction, 7, ref length);
                CalculateValueData(instruction, 8, ref length);
                CalculateValueData(instruction, 9, ref length);
                CalculateValueData(instruction, 10, ref length);
                break;

            case 4:
                CalculateVariableData(instruction, 1, ref length);
                CalculateStringData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                break;

            case 8:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                break;
        }
    }
}