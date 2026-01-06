using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction230Calculator
{
    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateByte(instruction, 0, ref length, out byte value);

        switch (value)
        {
            case 2:
            case 5:
                CalculateVariableData(instruction, 1, ref length);
                break;

            case 8:
            case 9:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateValueData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                break;
        }
    }
}