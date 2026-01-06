using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder;
using Kompression.Encoder.LempelZiv.PriceCalculators;
using System.Buffers.Binary;

namespace Logic.Domain.PandoraManagement.Compression;

internal class Nanako2Encoder : ILempelZivEncoder
{
    private const int WindowBufferLength_ = 0x1000;
    private const int PreBufferSize_ = 0xFEE;

    class Block
    {
        public byte[] buffer = new byte[1 + 8 * 2];
        public int bufferLength = 1;
        public int flagCount;
    }

    public void Configure(ILempelZivEncoderOptionsBuilder matchOptions)
    {
        matchOptions.CalculatePricesWith(() => new Lzss01PriceCalculator())
            .FindPatternMatches().WithinLimitations(3, 0x12, 1, 0x1000)
            .AdjustInput(input => input.Prepend(CreatePreBuffer()));
    }

    public void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
    {
        var compressedStream = new MemoryStream();
        EncodeInternal(input, compressedStream, matches);

        var buffer = new byte[4];

        BinaryPrimitives.WriteInt32LittleEndian(buffer, (int)compressedStream.Length);
        output.Write(buffer);

        BinaryPrimitives.WriteInt32LittleEndian(buffer, (int)input.Length);
        output.Write(buffer);

        compressedStream.Position = 0;
        compressedStream.CopyTo(output);

        output.Position = 0;
    }

    private void EncodeInternal(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
    {
        var block = new Block();

        foreach (var match in matches)
        {
            if (input.Position < match.Position)
                WriteRawData(input, output, block, match.Position - input.Position);

            WriteMatchData(input, output, block, match);
        }

        if (input.Position < input.Length)
            WriteRawData(input, output, block, input.Length - input.Position);

        WriteAndResetBuffer(output, block);
    }

    private void WriteRawData(Stream input, Stream output, Block block, long rawLength)
    {
        for (var i = 0; i < rawLength; i++)
        {
            if (block.flagCount == 8)
                WriteAndResetBuffer(output, block);

            block.buffer[0] |= (byte)(1 << block.flagCount++);
            block.buffer[block.bufferLength++] = (byte)input.ReadByte();
        }
    }

    private void WriteMatchData(Stream input, Stream output, Block block, LempelZivMatch lempelZivMatch)
    {
        if (block.flagCount == 8)
            WriteAndResetBuffer(output, block);

        var bufferPosition = (PreBufferSize_ + lempelZivMatch.Position - lempelZivMatch.Displacement) % WindowBufferLength_;

        var byte2 = (byte)(lempelZivMatch.Length - 3 & 0xF);
        byte2 |= (byte)(bufferPosition >> 4 & 0xF0);
        var byte1 = (byte)bufferPosition;

        block.flagCount++;
        block.buffer[block.bufferLength++] = byte1;
        block.buffer[block.bufferLength++] = byte2;
        input.Position += lempelZivMatch.Length;
    }

    private void WriteAndResetBuffer(Stream output, Block block)
    {
        output.Write(block.buffer, 0, block.bufferLength);

        Array.Clear(block.buffer, 0, block.bufferLength);
        block.bufferLength = 1;
        block.flagCount = 0;
    }

    private byte[] CreatePreBuffer()
    {
        var result = new byte[PreBufferSize_];

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