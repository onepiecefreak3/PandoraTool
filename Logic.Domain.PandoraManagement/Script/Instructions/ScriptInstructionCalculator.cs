using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.InternalContract.Script.Instructions;
using System.Text;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal abstract class ScriptInstructionCalculator : IScriptInstructionCalculator
{
    private static readonly Encoding Sjis = Encoding.GetEncoding("Shift-JIS");

    public int CalculateLength(ScriptInstruction instruction)
    {
        var length = 4;

        CalculateArgumentsLength(instruction, ref length);

        return length;
    }

    protected abstract void CalculateArgumentsLength(ScriptInstruction instruction, ref int length);

    protected static void CalculateByte(ScriptInstruction instruction, int argumentIndex, ref int length, out byte value)
    {
        if (argumentIndex >= instruction.Arguments.Length || instruction.Arguments[argumentIndex] is not ScriptArgumentInt argumentInt)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a number argument at position {argumentIndex}.");

        value = (byte)argumentInt.Value;

        length++;
    }

    protected static void CalculateInt32(ScriptInstruction instruction, int argumentIndex, ref int length, out int value)
    {
        if (argumentIndex >= instruction.Arguments.Length || instruction.Arguments[argumentIndex] is not ScriptArgumentInt argumentInt)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a number argument at position {argumentIndex}.");

        value = argumentInt.Value;

        length += 4;
    }

    protected static void CalculateJumpData(ScriptInstruction instruction, int argumentIndex, ref int length)
    {
        if (argumentIndex >= instruction.Arguments.Length || instruction.Arguments[argumentIndex] is not ScriptArgumentJump)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a jump argument at position {argumentIndex}.");

        length += 4;
    }

    protected static void CalculateValueData(ScriptInstruction instruction, int argumentIndex, ref int length)
    {
        if (argumentIndex >= instruction.Arguments.Length)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a data or numeric argument at position {argumentIndex}.");

        if (instruction.Arguments[argumentIndex] is ScriptArgumentBytes bytes)
            length += bytes.Data.Length;
        else if (instruction.Arguments[argumentIndex] is ScriptArgumentInt)
            length += 6;
        else
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a data or numeric argument at position {argumentIndex}.");
    }

    protected static void CalculateVariableData(ScriptInstruction instruction, int argumentIndex, ref int length)
    {
        if (argumentIndex >= instruction.Arguments.Length)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a data or variable argument at position {argumentIndex}.");

        if (instruction.Arguments[argumentIndex] is ScriptArgumentBytes bytes)
            length += bytes.Data.Length;
        else if (instruction.Arguments[argumentIndex] is ScriptArgumentVariable)
            length += 6;
        else
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a data or variable argument at position {argumentIndex}.");
    }

    protected static void CalculateStringData(ScriptInstruction instruction, int argumentIndex, ref int length)
    {
        if (argumentIndex >= instruction.Arguments.Length || instruction.Arguments[argumentIndex] is not ScriptArgumentString text)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a string argument at position {argumentIndex}.");

        length += Sjis.GetByteCount(text.Text + '\0');
    }
}