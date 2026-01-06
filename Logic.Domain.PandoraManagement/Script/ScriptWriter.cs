using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Script;
using System.Buffers.Binary;

namespace Logic.Domain.PandoraManagement.Script;

internal class ScriptWriter : IScriptWriter
{
    public byte[] Write(ScriptInstructionData[] instructions)
    {
        var buffer = new byte[4];
        var result = new List<byte>();

        foreach (ScriptInstructionData instruction in instructions)
        {
            int length = 4 + instruction.Arguments.Sum(i => i.Data.Length);

            BinaryPrimitives.WriteInt16LittleEndian(buffer, (short)instruction.Instruction);
            BinaryPrimitives.WriteInt16LittleEndian(buffer.AsSpan(2), (short)length);

            result.AddRange(buffer);

            foreach (ScriptArgumentData argument in instruction.Arguments)
                result.AddRange(argument.Data);
        }

        return [.. result];
    }
}