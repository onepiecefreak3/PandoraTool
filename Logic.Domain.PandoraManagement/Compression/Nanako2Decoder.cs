using Kompression.Contract.Decoder;
using Kompression.IO;
using System.Buffers.Binary;

namespace Logic.Domain.PandoraManagement.Compression;

internal class Nanako2Decoder : IDecoder
{
    private const int PreBufferSize = 0xFEE;

    public void Decode(Stream input, Stream output)
    {
        var buffer = new byte[4];

        _ = input.Read(buffer);
        _ = input.Read(buffer);
        int decompressedSize = BinaryPrimitives.ReadInt32LittleEndian(buffer);

        Decode(input, output, decompressedSize);
    }

    public void Decode(Stream input, Stream output, int decompressedSize)
    {
        var circularBuffer = new CircularBuffer(0x1000);
        circularBuffer.Write(CreatePreBuffer(), 0, PreBufferSize);

        var flags = 0;
        var flagPosition = 8;
        while (output.Length < decompressedSize)
        {
            if (flagPosition == 8)
            {
                flagPosition = 0;
                flags = input.ReadByte();
            }

            if ((flags >> flagPosition++ & 0x1) == 1)
            {
                // raw data
                var value = (byte)input.ReadByte();

                output.WriteByte(value);
                circularBuffer.WriteByte(value);
            }
            else
            {
                // compressed data
                var byte1 = input.ReadByte();
                var byte2 = input.ReadByte();

                var length = (byte2 & 0xF) + 3;
                var bufferPosition = byte1 | (byte2 & 0xF0) << 4;

                // Convert buffer position to displacement
                var displacement = (circularBuffer.Position - bufferPosition) % circularBuffer.Length;
                displacement = (displacement + circularBuffer.Length) % circularBuffer.Length;
                if (displacement == 0)
                    displacement = 0x1000;

                circularBuffer.Copy(output, displacement, length);
            }
        }
    }

    public void Dispose() { }

    private byte[] CreatePreBuffer()
    {
        var result = new byte[PreBufferSize];

        for (var i = 0; i < 256; i++)
            Array.Fill(result, (byte)i, i * 13, 13);

        for (var i = 0; i < 256; i++)
            result[0xd00 + i] = (byte)i;

        for (var i = 0; i < 256; i++)
            result[0xe00 + i] = (byte)(255 - i);

        Array.Fill(result, (byte)0x20, 0xf80, 0x6e);

        return result;
    }
}