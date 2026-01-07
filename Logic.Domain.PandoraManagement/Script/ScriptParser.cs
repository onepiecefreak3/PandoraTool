using System.Buffers.Binary;
using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Script;
using System.Text;
using Logic.Domain.PandoraManagement.Contract.Enums.Script;

namespace Logic.Domain.PandoraManagement.Script;

internal class ScriptParser(IScriptReader reader) : IScriptParser
{
    private static readonly Encoding Sjis = Encoding.GetEncoding("Shift-JIS");

    public ScriptInstruction[] Parse(byte[] data)
    {
        ScriptInstructionData[] instructions = reader.Read(data);
        Dictionary<int, string> jumpOffsets = CollectJumps(instructions);

        var result = new List<ScriptInstruction>();

        foreach (ScriptInstructionData instruction in instructions)
        {
            jumpOffsets.TryGetValue(instruction.Offset, out string? jumpLabel);

            result.Add(new ScriptInstruction
            {
                Instruction = instruction.Instruction,
                Arguments = ParseArguments(instruction.Arguments, jumpOffsets),
                JumpLabel = jumpLabel
            });
        }

        return [.. result];
    }

    private static Dictionary<int, string> CollectJumps(ScriptInstructionData[] instructions)
    {
        var jumpOffsets = new HashSet<int>();

        foreach (ScriptInstructionData instruction in instructions)
        {
            foreach (ScriptArgumentData argument in instruction.Arguments)
            {
                if (argument.Type is not ArgumentType.Jump)
                    continue;

                jumpOffsets.Add(BinaryPrimitives.ReadInt32LittleEndian(argument.Data));
            }
        }

        var result = new Dictionary<int, string>();

        var jumpIndex = 0;
        foreach (int jumpOffset in jumpOffsets.OrderBy(x => x))
        {
            result[jumpOffset] = $"@{jumpIndex++:000}@";
        }

        return result;
    }

    private static ScriptArgument[] ParseArguments(ScriptArgumentData[] arguments, Dictionary<int, string> jumpOffsets)
    {
        var result = new List<ScriptArgument>();

        foreach (ScriptArgumentData argumentData in arguments)
        {
            ScriptArgument argument;
            switch (argumentData.Type)
            {
                case ArgumentType.Data:
                    argument = new ScriptArgumentBytes { Data = argumentData.Data };
                    break;

                case ArgumentType.Byte:
                    argument = new ScriptArgumentInt { Value = argumentData.Data[0] };
                    break;

                case ArgumentType.Int:
                    argument = new ScriptArgumentInt { Value = BinaryPrimitives.ReadInt32LittleEndian(argumentData.Data) };
                    break;

                case ArgumentType.Jump:
                    int offset = BinaryPrimitives.ReadInt32LittleEndian(argumentData.Data);

                    if (!jumpOffsets.TryGetValue(offset, out string? jumpLabel))
                        throw new InvalidOperationException($"Could not resolve jump to offset {offset}.");

                    argument = new ScriptArgumentJump { Label = jumpLabel };
                    break;

                case ArgumentType.Value:
                    var operations = CreateOperations(argumentData.Data);

                    if (operations[^1].Operation is Operation.LoadInt)
                    {
                        argument = new ScriptArgumentInt { Value = operations[^1].Value };
                        break;
                    }

                    argument = new ScriptArgumentExpression { Operations = operations };
                    break;

                case ArgumentType.Variable:
                    var operations1 = CreateOperations(argumentData.Data);

                    if (operations1[^1].Operation is Operation.LoadVariable)
                    {
                        argument = new ScriptArgumentVariable { Value = operations1[^1].Value };
                        break;
                    }

                    argument = new ScriptArgumentExpression { Operations = operations1 };
                    break;

                case ArgumentType.String:
                    argument = new ScriptArgumentString { Text = Sjis.GetString(argumentData.Data).Trim('\0') };
                    break;

                default:
                    throw new InvalidOperationException($"Unknown argument {argumentData.Type}.");
            }

            result.Add(argument);
        }

        return [.. result];
    }

    private static ExpressionOperation[] CreateOperations(byte[] data)
    {
        var result = new List<ExpressionOperation>();

        for (var i = 0; i < data.Length; i++)
        {
            var operation = (Operation)data[i];
            int value = -1;

            if (operation is Operation.Exit)
                break;

            switch (operation)
            {
                case Operation.LoadInt:
                    value = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(i + 1, 4));
                    i += 4;
                    break;

                case Operation.LoadVariable:
                    value = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(i + 1, 4));
                    i += 4;
                    break;
            }

            result.Add(new ExpressionOperation(operation, value));
        }

        return [.. result];
    }
}