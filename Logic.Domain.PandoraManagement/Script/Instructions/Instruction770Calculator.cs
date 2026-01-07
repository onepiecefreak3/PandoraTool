using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction770Calculator
{
    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateByte(instruction, 0, ref length, out byte value);

        switch (value)
        {
            case 0:
                CalculateVariableData(instruction, 1, ref length);
                CalculateValueData(instruction, 2, ref length);
                CalculateVariableData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                break;

            case 1:
                CalculateValueData(instruction, 1, ref length);
                CalculateVariableData(instruction, 2, ref length);
                CalculateVariableData(instruction, 3, ref length);
                CalculateValueData(instruction, 4, ref length);
                break;
        }
    }
}