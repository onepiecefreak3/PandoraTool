using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Script;
using Logic.Domain.PandoraManagement.InternalContract.Script.Instructions;
using System.Buffers.Binary;
using Logic.Domain.PandoraManagement.Script.Instructions;

namespace Logic.Domain.PandoraManagement.Script;

internal class ScriptReader : IScriptReader
{
    public ScriptInstructionData[] Read(byte[] data)
    {
        var result = new List<ScriptInstructionData>();

        var offset = 0;
        while (offset < data.Length)
        {
            int instruction = BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(offset));
            IScriptInstructionReader? instructionParser = ScriptInstructionReaderFactory.Instance.Get(instruction);

            if (instructionParser is null)
                throw new InvalidOperationException($"Could not read unknown instruction {instruction}.");

            result.Add(instructionParser.Read(data, ref offset));
        }

        return [.. result];
    }
}