using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Enums.Script;
using Logic.Domain.PandoraManagement.InternalContract.Script.Instructions;
using System.Buffers.Binary;
using System.Text;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal abstract class ScriptInstructionComposer : IScriptInstructionComposer
{
    private static readonly Encoding Sjis = Encoding.GetEncoding("Shift-JIS");

    public ScriptInstructionData Compose(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, ref int offset)
    {
        int startOffset = offset;

        var arguments = new List<ScriptArgumentData>();

        offset += 4;
        ComposeArguments(instruction, jumpLookup, arguments, ref offset);

        return new ScriptInstructionData
        {
            Offset = startOffset,
            Instruction = instruction.Instruction,
            Arguments = [.. arguments]
        };
    }

    protected abstract void ComposeArguments(ScriptInstruction instruction, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset);

    protected static void ComposeByte(ScriptInstruction instruction, int argumentIndex, List<ScriptArgumentData> arguments, ref int offset, out byte value)
    {
        if (argumentIndex >= instruction.Arguments.Length || instruction.Arguments[argumentIndex] is not ScriptArgumentInt argumentInt)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a number argument at position {argumentIndex}.");

        value = (byte)argumentInt.Value;

        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.Byte,
            Data = [value]
        });

        offset += 1;
    }

    protected static void ComposeInt32(ScriptInstruction instruction, int argumentIndex, List<ScriptArgumentData> arguments, ref int offset, out int value)
    {
        if (argumentIndex >= instruction.Arguments.Length || instruction.Arguments[argumentIndex] is not ScriptArgumentInt argumentInt)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a number argument at position {argumentIndex}.");

        value = argumentInt.Value;

        var buffer = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, argumentInt.Value);

        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.Int,
            Data = buffer
        });

        offset += 4;
    }

    protected static void ComposeJumpData(ScriptInstruction instruction, int argumentIndex, Dictionary<string, int> jumpLookup, List<ScriptArgumentData> arguments, ref int offset)
    {
        if (argumentIndex >= instruction.Arguments.Length || instruction.Arguments[argumentIndex] is not ScriptArgumentJump jump)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a jump argument at position {argumentIndex}.");

        if (!jumpLookup.TryGetValue(jump.Label, out int jumpOffset))
            throw new InvalidOperationException($"Could not resolve jump label {jump.Label}.");

        var buffer = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, jumpOffset);

        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.Jump,
            Data = buffer
        });

        offset += 4;
    }

    protected static void ComposeValueData(ScriptInstruction instruction, int argumentIndex, List<ScriptArgumentData> arguments, ref int offset)
    {
        if (argumentIndex >= instruction.Arguments.Length)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a data or numeric argument at position {argumentIndex}.");

        ScriptArgumentData argument;
        if (instruction.Arguments[argumentIndex] is ScriptArgumentExpression expression)
        {
            argument = new ScriptArgumentData
            {
                Offset = offset,
                Type = ArgumentType.Value,
                Data = ComposeExpressionData(expression)
            };
        }
        else if (instruction.Arguments[argumentIndex] is ScriptArgumentInt value)
        {
            var buffer = new byte[6];
            buffer[0] = 0x01;

            BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(1), value.Value);

            argument = new ScriptArgumentData
            {
                Offset = offset,
                Type = ArgumentType.Value,
                Data = buffer
            };
        }
        else if (instruction.Arguments[argumentIndex] is ScriptArgumentVariable variable)
        {
            var buffer = new byte[6];
            buffer[0] = 0x02;

            BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(1), variable.Value);

            argument = new ScriptArgumentData
            {
                Offset = offset,
                Type = ArgumentType.Value,
                Data = buffer
            };
        }
        else
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a data or numeric argument at position {argumentIndex}.");

        arguments.Add(argument);
        offset += argument.Data.Length;
    }

    protected static void ComposeVariableData(ScriptInstruction instruction, int argumentIndex, List<ScriptArgumentData> arguments, ref int offset)
    {
        if (argumentIndex >= instruction.Arguments.Length)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a data or variable argument at position {argumentIndex}.");

        ScriptArgumentData argument;
        if (instruction.Arguments[argumentIndex] is ScriptArgumentExpression expression)
        {
            argument = new ScriptArgumentData
            {
                Offset = offset,
                Type = ArgumentType.Value,
                Data = ComposeExpressionData(expression)
            };
        }
        else if (instruction.Arguments[argumentIndex] is ScriptArgumentVariable variable)
        {
            var buffer = new byte[6];
            buffer[0] = 0x02;

            BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(1), variable.Value);

            argument = new ScriptArgumentData
            {
                Offset = offset,
                Type = ArgumentType.Variable,
                Data = buffer
            };
        }
        else
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a data or variable argument at position {argumentIndex}.");

        arguments.Add(argument);
        offset += argument.Data.Length;
    }

    protected static void ComposeStringData(ScriptInstruction instruction, int argumentIndex, List<ScriptArgumentData> arguments, ref int offset)
    {
        if (argumentIndex >= instruction.Arguments.Length || instruction.Arguments[argumentIndex] is not ScriptArgumentString text)
            throw new InvalidOperationException($"Instruction {instruction.Instruction} requires a string argument at position {argumentIndex}.");

        byte[] data = Sjis.GetBytes(text.Text + '\0');

        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.Value,
            Data = data
        });

        offset += data.Length;
    }

    private static byte[] ComposeExpressionData(ScriptArgumentExpression expression)
    {
        int length = CalculateExpression(expression);
        var data = new byte[length];

        int offset = 0;
        foreach (var operation in expression.Operations)
        {
            data[offset++] = (byte)operation.Operation;

            if (operation.Operation is Operation.LoadInt or Operation.LoadVariable)
            {
                BinaryPrimitives.WriteInt32LittleEndian(data.AsSpan(offset), operation.Value);
                offset += 4;
            }
        }

        return data;
    }

    private static int CalculateExpression(ScriptArgumentExpression expression)
    {
        var length = 1;

        foreach (var operation in expression.Operations)
        {
            if (operation.Operation is Operation.LoadInt or Operation.LoadVariable)
                length += 4;

            length++;
        }

        return length;
    }
}