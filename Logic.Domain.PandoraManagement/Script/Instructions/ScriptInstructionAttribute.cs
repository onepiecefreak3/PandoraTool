namespace Logic.Domain.PandoraManagement.Script.Instructions;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ScriptInstructionAttribute(int instruction, string? expression = null, bool isManual = false) : Attribute
{
    public int Instruction => instruction;
    public string? Expression => expression;
    public bool IsManual => isManual;
}