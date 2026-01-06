using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Script;
using Logic.Domain.PandoraManagement.InternalContract.Script.Instructions;
using System.Buffers.Binary;
using Logic.Domain.PandoraManagement.Contract.Enums.Script;
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
            {
                int length = BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(offset + 2));

                result.Add(new ScriptInstructionData
                {
                    Offset = offset,
                    Instruction = instruction,
                    Arguments = length <= 4 ? [] : [
                            new ScriptArgumentData
                            {
                                Offset = offset + 4,
                                Type = ArgumentType.Data,
                                Data = data[(offset + 4)..(offset + length)]
                            }]
                });

                offset += length;
            }
            else
            {
                result.Add(instructionParser.Read(data, ref offset));
            }
        }

        return [.. result];
    }
}