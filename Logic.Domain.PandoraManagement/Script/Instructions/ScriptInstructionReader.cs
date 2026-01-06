using System.Buffers.Binary;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Enums.Script;
using Logic.Domain.PandoraManagement.InternalContract.Script.Instructions;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal abstract class ScriptInstructionReader : IScriptInstructionReader
{
    public ScriptInstructionData Read(byte[] data, ref int offset)
    {
        int startOffset = offset;
        int instruction = BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(offset));
        int length = BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(offset + 2));

        var arguments = new List<ScriptArgumentData>();

        offset += 4;
        ReadArguments(arguments, data, ref offset, startOffset + length);

        if (startOffset + length != offset)
            throw new InvalidOperationException($"Inconsistent data for instruction {instruction} at offset {startOffset}.");

        return new ScriptInstructionData
        {
            Offset = startOffset,
            Instruction = instruction,
            Arguments = [.. arguments]
        };
    }

    protected abstract void ReadArguments(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, int endOffset);

    protected static void ReadByte(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, out int value)
    {
        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.Byte,
            Data = [data[offset]]
        });

        value = data[offset];

        offset += 1;
    }

    protected static void ReadInt32(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, out int value)
    {
        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.Int,
            Data = [.. data[offset..(offset + 4)]]
        });

        value = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(offset, 4));

        offset += 4;
    }

    protected static void ReadJumpData(IList<ScriptArgumentData> arguments, byte[] data, ref int offset)
    {
        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.Jump,
            Data = [.. data[offset..(offset + 4)]]
        });

        offset += 4;
    }

    protected static void ReadValueData(IList<ScriptArgumentData> arguments, byte[] data, ref int offset)
    {
        int length = CalculateArgumentLength(data, offset);

        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.Value,
            Data = [.. data[offset..(offset + length)]]
        });

        offset += length;
    }

    protected static void ReadVariableData(IList<ScriptArgumentData> arguments, byte[] data, ref int offset)
    {
        int length = CalculateArgumentLength(data, offset);

        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.Variable,
            Data = [.. data[offset..(offset + length)]]
        });

        offset += length;
    }

    protected static void ReadStringData(IList<ScriptArgumentData> arguments, byte[] data, ref int offset)
    {
        int length = CalculateStringLength(data, offset);

        arguments.Add(new ScriptArgumentData
        {
            Offset = offset,
            Type = ArgumentType.String,
            Data = [.. data[offset..(offset + length)]]
        });

        offset += length;
    }

    protected static int CalculateArgumentLength(byte[] data, int offset)
    {
        int i = offset;

        for (; i < data.Length; i++)
        {
            if (data[i] is 0)
            {
                i++;
                break;
            }

            if (data[i] is 1 or 2)
                i += 4;
        }

        return i - offset;
    }

    protected static int CalculateStringLength(byte[] data, int offset)
    {
        int i = offset;

        for (; i < data.Length; i++)
        {
            if (data[i] is 0)
            {
                i++;
                break;
            }
        }

        return i - offset;
    }
}