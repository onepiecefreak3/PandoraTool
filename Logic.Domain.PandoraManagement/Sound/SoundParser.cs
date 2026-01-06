using Komponent.Contract.Enums;
using Komponent.IO;
using Logic.Domain.PandoraManagement.Contract.Sound;
using System.Buffers.Binary;

namespace Logic.Domain.PandoraManagement.Sound;

internal class SoundParser : ISoundParser
{
    public byte[] Parse(byte[] data)
    {
        var format = BinaryPrimitives.ReadInt32LittleEndian(data);
        if (format == 0x46464952)
            return data;

        switch (format)
        {
            case 8:
                return DecompressSound8(data[4..]);

            case 12:
                return DecompressSound12(data[4..]);

            default:
                throw new InvalidOperationException("Unknown sound encoding.");
        }
    }

    private byte[] DecompressSound8(byte[] data)
    {
        var dataSize = BinaryPrimitives.ReadInt32LittleEndian(data[0x28..]);

        var result = new byte[0x2C + dataSize];

        Array.Copy(data, result, 0x2C);

        var dataIndex = 0x2C;
        for (var i = 0; i < 2; i++)
        {
            var factor = 0x7F;
            ushort unk1 = 0;
            short unk2 = 0;

            var outputIndex = 0x2C + i * 2;

            for (var s = 0; s < dataSize >> 2; s++)
            {
                var nibble = (s & 1) == 0 ? data[dataIndex] >> 4 : data[dataIndex++];

                var preSample = (byte)((nibble & 7) * 2 + 1) * factor;
                preSample = (ushort)(preSample >> 3);

                if ((nibble & 8) == 0)
                {
                    unk2 += (short)(((unk1 + preSample) >> 16) & 1);
                    unk1 = (ushort)(unk1 + preSample);

                    if (unk2 > -1 && unk1 > 0x7FFF)
                    {
                        unk1 = 0x7FFF;
                        unk2 = 0;
                    }
                }
                else
                {
                    unk2 = (short)(unk2 - (unk1 < preSample ? 1 : 0));
                    unk1 = (ushort)(unk1 - preSample);

                    if (unk2 < 0 && unk1 < 0x8000)
                    {
                        unk1 = 0x8000;
                        unk2 = -1;
                    }
                }

                var preSample1 = Disp[nibble & 0xF] * factor;

                var b1 = (byte)((byte)(((short)(preSample1 >> 16) << 1 | ((short)preSample1 < 0 ? 1 : 0)) << 1) | ((short)(preSample1 << 1) < 0 ? 1 : 0));
                var b2 = (byte)((ushort)((short)preSample1 << 2) >> 8);

                factor = b1 << 8 | b2;
                if (factor < 0x7F)
                    factor = 0x7F;
                else if (factor > 0x6000)
                    factor = 0x6000;

                result[outputIndex++] = (byte)unk1;
                result[outputIndex++] = (byte)(unk1 >> 8);

                outputIndex += 2;
            }
        }

        return result;
    }

    private byte[] DecompressSound12(byte[] data)
    {
        var dataSize = BinaryPrimitives.ReadInt32LittleEndian(data[0x28..]);

        var result = new byte[0x2C + dataSize];

        Array.Copy(data, result, 0x2C);

        using var bitReader = new BinaryBitReader(new MemoryStream(data[0x2C..]), BitOrder.LeastSignificantBitFirst, 1, ByteOrder.LittleEndian);
        for (var i = 0; i < 2; i++)
        {
            var factor = 0x7F;
            ushort unk1 = 0;
            short unk2 = 0;

            var outputIndex = 0x2C + i * 2;

            for (var s = 0; s < dataSize >> 2; s++)
            {
                var nibble = ReadSound12Value(bitReader);

                var preSample = (byte)((nibble & 7) * 2 + 1) * factor;
                preSample = (ushort)(preSample >> 3);

                if ((nibble & 8) == 0)
                {
                    unk2 += (short)(((unk1 + preSample) >> 16) & 1);
                    unk1 = (ushort)(unk1 + preSample);

                    if (unk2 > -1 && unk1 > 0x7FFF)
                    {
                        unk1 = 0x7FFF;
                        unk2 = 0;
                    }
                }
                else
                {
                    unk2 = (short)(unk2 - (unk1 < preSample ? 1 : 0));
                    unk1 = (ushort)(unk1 - preSample);

                    if (unk2 < 0 && unk1 < 0x8000)
                    {
                        unk1 = 0x8000;
                        unk2 = -1;
                    }
                }

                var preSample1 = Disp[nibble & 0xF] * factor;

                var b1 = (byte)((byte)(((short)(preSample1 >> 16) << 1 | ((short)preSample1 < 0 ? 1 : 0)) << 1) | ((short)(preSample1 << 1) < 0 ? 1 : 0));
                var b2 = (byte)((ushort)((short)preSample1 << 2) >> 8);

                factor = b1 << 8 | b2;
                if (factor < 0x7F)
                    factor = 0x7F;
                else if (factor > 0x6000)
                    factor = 0x6000;

                result[outputIndex++] = (byte)unk1;
                result[outputIndex++] = (byte)(unk1 >> 8);

                outputIndex += 2;
            }
        }

        return result;
    }

    private int ReadSound12Value(BinaryBitReader reader)
    {
        var part = reader.ReadBits<int>(2);
        if (part == 2)
            return 0;
        if (part == 0)
            return 8;

        part = part | (reader.ReadBits<int>(1) << 2);
        if (part == 5)
            return 1;
        if (part == 1)
            return 9;

        part = part | (reader.ReadBits<int>(1) << 3);
        if (part == 11)
            return 2;
        if (part == 3)
            return 10;

        part = part | (reader.ReadBits<int>(1) << 4);
        if (part == 0x17)
            return 3;
        if (part == 7)
            return 11;

        part = part | (reader.ReadBits<int>(1) << 5);
        if (part == 0x2f)
            return 4;
        if (part == 15)
            return 12;

        part = part | (reader.ReadBits<int>(1) << 6);
        if (part == 0x5f)
            return 5;
        if (part == 0x1f)
            return 13;

        part = part | (reader.ReadBits<int>(1) << 7);
        if (part == 0x7f)
            return 6;
        if (part == 0xff)
            return 14;
        if (part == 0xbf)
            return 7;

        return 15;
    }

    private static readonly short[] Disp = [0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99, 0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99];

}