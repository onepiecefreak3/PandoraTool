using System.Buffers.Binary;
using Logic.Domain.PandoraManagement.InternalContract.Sound;

namespace Logic.Domain.PandoraManagement.Sound.Compression;

internal class SoundCompressor8 : ISoundCompressor8
{
    private static readonly short[] Multiplier = [0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99, 0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99];

    public byte[] Compress(byte[] data)
    {
        int sampleCount = (data.Length - 0x2C) >> 1;
        int channelSampleCount = sampleCount >> 1;

        int outputLength = 0x2C + channelSampleCount;
        var output = new byte[outputLength];

        var outputIndex = 0x2C;

        for (var channel = 0; channel < 2; channel++)
        {
            var factor = 0x7F;
            ushort sample = 0;
            short carry = 0;

            var hasHigh = false;

            for (int s = 0x2C + channel * 2; s < data.Length; s += 4)
            {
                ushort target = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(s));

                int nibble = ChooseBestNibble(target, factor, sample, carry);
                StepDecodeNibble(nibble, ref factor, ref sample, ref carry);

                // HINT: On odd sample count in each channel, the outputIndex is not correctly updated and writes one nibble twice
                // HINT: This is the behaviour as researched in the original decompression code and will therefore not be fixed here to retain original functionality
                if (hasHigh)
                {
                    output[outputIndex++] |= (byte)nibble;
                    hasHigh = false;
                }
                else
                {
                    output[outputIndex] |= (byte)(nibble << 4);
                    hasHigh = true;
                }
            }
        }

        Array.Copy(data, output, 0x2C);

        return output;
    }

    private static int ChooseBestNibble(ushort target, int factor, ushort sample, short carry)
    {
        var bestValue = 0;
        var bestError = int.MaxValue;

        for (var nibble = 0; nibble < 16; nibble++)
        {
            int f = factor;
            ushort s = sample;
            short c = carry;

            StepDecodeNibble(nibble, ref f, ref s, ref c);

            int error = Math.Abs((short)s - (short)target);

            if (error < bestError)
            {
                bestError = error;
                bestValue = nibble;

                if (error is 0)
                    break;
            }
        }

        return bestValue;
    }

    private static void StepDecodeNibble(int nibble, ref int factor, ref ushort sample, ref short carry)
    {
        var delta = (ushort)((((nibble & 7) * 2 + 1) * factor) >> 3);

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

        factor = (Multiplier[nibble & 0xF] * factor) >> 6;
        factor = Math.Clamp(factor, 0x7F, 0x6000);
    }
}