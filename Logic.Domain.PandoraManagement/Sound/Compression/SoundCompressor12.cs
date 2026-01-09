using Komponent.Contract.Enums;
using Komponent.IO;
using Logic.Domain.PandoraManagement.InternalContract.Sound;
using System.Buffers.Binary;

namespace Logic.Domain.PandoraManagement.Sound.Compression;

internal class SoundCompressor12 : ISoundCompressor12
{
    private static readonly short[] Multiplier = [0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99, 0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99];

    public byte[] Compress(byte[] data)
    {
        var output = new MemoryStream();
        output.Write(data.AsSpan(0, 0x2C));

        using var bitWriter = new BinaryBitWriter(output, BitOrder.LeastSignificantBitFirst, 1, ByteOrder.LittleEndian);

        for (var channel = 0; channel < 2; channel++)
        {
            var factor = 0x7F;
            ushort sample = 0;
            short carry = 0;

            for (int s = 0x2C + channel * 2; s < data.Length; s += 4)
            {
                ushort target = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(s));

                int nibble = ChooseBestNibble(target, factor, sample, carry);
                StepDecodeNibble(nibble, ref factor, ref sample, ref carry);

                WriteEncodedNibble(bitWriter, nibble);
            }
        }

        return output.ToArray();
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

    private static void WriteEncodedNibble(BinaryBitWriter writer, int nibble)
    {
        switch (nibble)
        {
            case 0: writer.WriteBits(2, 2); break;
            case 1: writer.WriteBits(5, 3); break;
            case 2: writer.WriteBits(11, 4); break;
            case 3: writer.WriteBits(0x17, 5); break;
            case 4: writer.WriteBits(0x2F, 6); break;
            case 5: writer.WriteBits(0x5F, 7); break;
            case 6: writer.WriteBits(0x7F, 8); break;
            case 7: writer.WriteBits(0xBF, 8); break;
            case 8: writer.WriteBits(0, 2); break;
            case 9: writer.WriteBits(1, 3); break;
            case 10: writer.WriteBits(3, 4); break;
            case 11: writer.WriteBits(7, 5); break;
            case 12: writer.WriteBits(0xF, 6); break;
            case 13: writer.WriteBits(0x1F, 7); break;
            case 14: writer.WriteBits(0xFF, 8); break;
            case 15: writer.WriteBits(0x3F, 8); break;
        }
    }
}