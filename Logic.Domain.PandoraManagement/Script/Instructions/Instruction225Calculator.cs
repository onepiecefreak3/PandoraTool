using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction225Calculator
{
    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateByte(instruction, 0, ref length, out byte value);

        switch (value)
        {
            case 0:
            case 2:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                break;

            case 1:
            case 3:
                CalculateVariableData(instruction, 1, ref length);
                CalculateVariableData(instruction, 2, ref length);
                break;

            case 4:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                break;

            case 5:
                CalculateValueData(instruction, 1, ref length);
                break;

            case 6:
                CalculateVariableData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                break;
        }
    }
}