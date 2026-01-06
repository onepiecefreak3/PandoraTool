using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction229Calculator
{
    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateByte(instruction, 0, ref length, out byte value);

        switch (value)
        {
            case 3:
            case 4:
                CalculateValueData(instruction, 1, ref length);
                break;

            case 5:
                CalculateValueData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                break;
        }
    }
}