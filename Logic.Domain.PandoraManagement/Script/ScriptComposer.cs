using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;
using Logic.Domain.PandoraManagement.Contract.Script;
using Logic.Domain.PandoraManagement.InternalContract.Script.Instructions;
using Logic.Domain.PandoraManagement.Script.Instructions;

namespace Logic.Domain.PandoraManagement.Script;

internal class ScriptComposer(IScriptWriter writer) : IScriptComposer
{
    public byte[] Compose(ScriptInstruction[] instructions)
    {
        var result = new List<ScriptInstructionData>();

        Dictionary<string, int> jumpLookup = CollectJumps(instructions);

        var offset = 0;
        foreach (ScriptInstruction instruction in instructions)
        {
            IScriptInstructionComposer? composer = ScriptInstructionComposerFactory.Instance.Get(instruction.Instruction);

            if (composer is null)
                throw new InvalidOperationException($"Could not compose unknown instruction {instruction}.");

            result.Add(composer.Compose(instruction, jumpLookup, ref offset));
        }

        return writer.Write([.. result]);
    }

    private static Dictionary<string, int> CollectJumps(ScriptInstruction[] instructions)
    {
        var result = new Dictionary<string, int>();

        var offset = 0;
        foreach (ScriptInstruction instruction in instructions)
        {
            if (instruction.JumpLabel is not null)
                result[instruction.JumpLabel] = offset;

            IScriptInstructionCalculator? calculator = ScriptInstructionCalculatorFactory.Instance.Get(instruction.Instruction);

            if (calculator is null)
                throw new InvalidOperationException($"Could not compose unknown instruction {instruction}.");

            offset += calculator.CalculateLength(instruction);
        }

        return result;
    }
}