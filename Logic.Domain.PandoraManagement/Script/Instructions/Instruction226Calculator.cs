using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction226Calculator
{
    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateByte(instruction, 0, ref length, out byte value);

        switch (value)
        {
            case 0:
                CalculateVariableData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                break;

            case 1:
            case 12:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                break;

            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 8:
                CalculateValueData(instruction, 1, ref length);
                break;

            case 7:
            case 9:
            case 10:
                CalculateVariableData(instruction, 1, ref length);
                break;

            case 11:
                CalculateVariableData(instruction, 1, ref length);
                CalculateVariableData(instruction, 2, ref length);
                CalculateVariableData(instruction, 3, ref length);
                CalculateVariableData(instruction, 4, ref length);
                break;
        }
    }
}