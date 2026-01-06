using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction227Calculator
{
    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateByte(instruction, 0, ref length, out byte value);

        switch (value)
        {
            case 0:
                CalculateVariableData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateStringData(instruction, 4, ref length);
                break;

            case 1:
                CalculateVariableData(instruction, 1, ref length);
                CalculateVariableData(instruction, 2, ref length);
                CalculateVariableData(instruction, 3, ref length);
                CalculateVariableData(instruction, 4, ref length);
                CalculateVariableData(instruction, 5, ref length);
                CalculateVariableData(instruction, 6, ref length);
                CalculateVariableData(instruction, 7, ref length);
                CalculateVariableData(instruction, 8, ref length);
                CalculateStringData(instruction, 9, ref length);
                break;

            case 2:
            case 15:
            case 16:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                CalculateValueData(instruction, 5, ref length);
                CalculateValueData(instruction, 6, ref length);
                CalculateValueData(instruction, 7, ref length);
                CalculateValueData(instruction, 8, ref length);
                CalculateStringData(instruction, 9, ref length);
                break;

            case 3:
            case 13:
            case 14:
                CalculateVariableData(instruction, 1, ref length);
                CalculateStringData(instruction, 2, ref length);
                break;

            case 4:
            case 6:
            case 22:
                CalculateVariableData(instruction, 1, ref length);
                break;

            case 7:
            case 8:
            case 9:
                CalculateVariableData(instruction, 1, ref length);
                CalculateVariableData(instruction, 2, ref length);
                CalculateVariableData(instruction, 3, ref length);
                CalculateVariableData(instruction, 4, ref length);
                CalculateVariableData(instruction, 5, ref length);
                CalculateVariableData(instruction, 6, ref length);
                CalculateVariableData(instruction, 7, ref length);
                CalculateVariableData(instruction, 8, ref length);
                break;

            case 10:
                CalculateVariableData(instruction, 1, ref length);
                CalculateVariableData(instruction, 2, ref length);
                break;

            case 11:
            case 12:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                break;

            case 17:
            case 18:
                CalculateStringData(instruction, 1, ref length);
                CalculateStringData(instruction, 2, ref length);
                break;

            case 20:
                CalculateStringData(instruction, 1, ref length);
                break;

            case 23:
            case 26:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                break;

            case 24:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                break;

            case 25:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                CalculateValueData(instruction, 5, ref length);
                break;
        }
    }
}