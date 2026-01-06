using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction208Calculator
{
    protected override void CalculateArgumentsLength(ScriptInstruction instruction, ref int length)
    {
        CalculateInt32(instruction, 0, ref length, out int value);

        if (value != 0)
            return;

        CalculateValueData(instruction, 1, ref length);
        CalculateValueData(instruction, 2, ref length);
        CalculateValueData(instruction, 3, ref length);
        CalculateValueData(instruction, 4, ref length);
        CalculateValueData(instruction, 5, ref length);
        CalculateStringData(instruction, 6, ref length);

        if (instruction.Arguments.Length > 7)
        {
            CalculateVariableData(instruction, 7, ref length);
        }
    }
}