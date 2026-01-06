using Logic.Domain.PandoraManagement.Contract.DataClasses.Script;

namespace Logic.Domain.PandoraManagement.Script.Instructions;

internal partial class Instruction209Reader
{
    private static readonly int[] ValueCount = [9, 9, 11, 11, 10, 10, 12, 12, 11, 11, 13, 13, 14, 14, 16, 16, 11, 11, 12, 12, 14, 14, 14, 14, 15, 15, 14, 14, 10, 10, 8, 11, 11, 8, 8];

    protected override void ReadArguments(IList<ScriptArgumentData> arguments, byte[] data, ref int offset, int endOffset)
    {
        ReadInt32(arguments, data, ref offset, out int value);

        switch (value)
        {
            case >= 0 and < 35:
                int valueCount = ValueCount[value];

                for (var i = 0; i < valueCount; i++)
                    ReadValueData(arguments, data, ref offset);
                break;

            case 35:
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadValueData(arguments, data, ref offset);
                ReadStringData(arguments, data, ref offset);

                if (offset < endOffset)
                {
                    ReadValueData(arguments, data, ref offset);
                    ReadValueData(arguments, data, ref offset);
                    ReadValueData(arguments, data, ref offset);
                    ReadValueData(arguments, data, ref offset);
                }

                break;
        }
    }
}