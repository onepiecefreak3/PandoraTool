using Logic.Domain.PandoraManagement.InternalContract.Sound;
using System.Buffers.Binary;

namespace Logic.Domain.PandoraManagement.Sound.Compression;

internal class SoundDecompressor8 : ISoundDecompressor8
{
    private static readonly short[] Multiplier = [0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99, 0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99];

    public byte[] Decompress(byte[] data)
    {
        int dataSize = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(0x28));

        var result = new byte[0x2C + dataSize];

        Array.Copy(data, result, 0x2C);

        var dataIndex = 0x2C;
        for (var channel = 0; channel < 2; channel++)
        {
            var factor = 0x7F;
            ushort sample = 0;
            short carry = 0;

            int outputIndex = 0x2C + channel * 2;

            for (var s = 0; s < dataSize >> 2; s++)
            {
                // HINT: On odd sample count in each channel, the dataIndex is not correctly updated and reads one nibble twice
                // HINT: This is the behaviour as researched in the original code and will therefore not be fixed here to retain original functionality
                int nibble = (s & 1) == 0 ? (data[dataIndex] >> 4) & 0xF : data[dataIndex++] & 0xF;
                
                int delta = (ushort)((((nibble & 7) * 2 + 1) * factor) >> 3);

                if ((nibble & 8) == 0)
                {
                    carry += (short)(((sample + delta) >> 16) & 1);
                    sample = (ushort)(sample + delta);

                    if (carry > -1 && sample > 0x7FFF)
                    {
                        sample = 0x7FFF;
                        carry = 0;
                    }
                }
                else
                {
                    carry = (short)(carry - (sample < delta ? 1 : 0));
                    sample = (ushort)(sample - delta);

                    if (carry < 0 && sample < 0x8000)
                    {
                        sample = 0x8000;
                        carry = -1;
                    }
                }

                factor = Math.Clamp((factor * Multiplier[nibble]) >> 6, 0x7F, 0x6000);

                BinaryPrimitives.WriteUInt16LittleEndian(result.AsSpan(outputIndex), sample);
                outputIndex += 4;
            }
        }

        return result;
    }
}