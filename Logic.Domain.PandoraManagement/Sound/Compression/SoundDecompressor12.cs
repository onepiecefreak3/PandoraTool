using Komponent.Contract.Enums;
using Komponent.IO;
using Logic.Domain.PandoraManagement.InternalContract.Sound;
using System.Buffers.Binary;

namespace Logic.Domain.PandoraManagement.Sound.Compression;

internal class SoundDecompressor12 : ISoundDecompressor12
{
    private static readonly short[] Multiplier = [0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99, 0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99];

    public byte[] Decompress(byte[] data)
    {
        int dataSize = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(0x28));

        var result = new byte[0x2C + dataSize];

        Array.Copy(data, result, 0x2C);

        using var bitReader = new BinaryBitReader(new MemoryStream(data[0x2C..]), BitOrder.LeastSignificantBitFirst, 1, ByteOrder.LittleEndian);
        for (var i = 0; i < 2; i++)
        {
            var factor = 0x7F;
            ushort sample = 0;
            short carry = 0;

            int outputIndex = 0x2C + i * 2;

            for (var s = 0; s < dataSize >> 2; s++)
            {
                int nibble = ReadEncodedNibble(bitReader);

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

    private static int ReadEncodedNibble(BinaryBitReader reader)
    {
        var part = reader.ReadBits<int>(2);
        if (part == 2)
            return 0;
        if (part == 0)
            return 8;

        part |= reader.ReadBits<int>(1) << 2;
        if (part == 5)
            return 1;
        if (part == 1)
            return 9;

        part |= reader.ReadBits<int>(1) << 3;
        if (part == 11)
            return 2;
        if (part == 3)
            return 10;

        part |= reader.ReadBits<int>(1) << 4;
        if (part == 0x17)
            return 3;
        if (part == 7)
            return 11;

        part |= reader.ReadBits<int>(1) << 5;
        if (part == 0x2f)
            return 4;
        if (part == 15)
            return 12;

        part |= reader.ReadBits<int>(1) << 6;
        if (part == 0x5f)
            return 5;
        if (part == 0x1f)
            return 13;

        part |= reader.ReadBits<int>(1) << 7;
        if (part == 0x7f)
            return 6;
        if (part == 0xff)
            return 14;
        if (part == 0xbf)
            return 7;

        return 15;
    }
}