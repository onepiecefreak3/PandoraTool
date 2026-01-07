namespace Logic.Domain.PandoraManagement.Contract.Enums.Script;

public enum Operation : byte
{
    Exit = 0,
    LoadInt = 1,
    LoadVariable = 2,
    VariableValueToValue = 0x82,
    VariableValueToValueNegate = 0x83,
    VariableValueToValueInvert = 0x84,
    VariableValueToValueBool = 0x85,
    VariableIndexToValue = 0x86,
    ValueToVariableValue = 0x87,
    Multiply = 0x88,
    Divide = 0x89,
    Modulo = 0x8A,
    Add = 0x8B,
    Subtract = 0x8C,
    ShiftLeft = 0x8D,
    ShiftRight = 0x8E,
    GreaterEquals = 0x8F,
    GreaterThan = 0x90,
    SmallerEquals = 0x91,
    SmallerThan = 0x92,
    Equals = 0x93,
    NotEquals = 0x94,
    BitwiseAnd = 0x95,
    BitwiseXor = 0x96,
    BitwiseOr = 0x97,
    LogicalAnd = 0x98,
    LogicalOr = 0x99,
    SetVariable = 0x9A
}