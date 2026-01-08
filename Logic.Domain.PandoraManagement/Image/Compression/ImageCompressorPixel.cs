using Komponent.Contract.Enums;
using Komponent.IO;
using Logic.Domain.PandoraManagement.InternalContract.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.PandoraManagement.Image.Compression;

internal class ImageCompressorPixel : IImageCompressorPixel
{
    public byte[] Compress(Image<Bgr24> image)
    {
        var output = new MemoryStream { Position = 8 };

        output.WriteByte(image[0, 0].B);
        output.WriteByte(image[0, 0].G);
        output.WriteByte(image[0, 0].R);

        var bitWriter = new BinaryBitWriter(output, BitOrder.LeastSignificantBitFirst, 1, Komponent.Contract.Enums.ByteOrder.LittleEndian);

        for (var y = 0; y < image.Height; y++)
        {
            for (int x = y == 0 ? 1 : 0; x < image.Width; x++)
            {
                if (y == 0 || x == 0)
                    WriteSimpleContextColor(bitWriter, image, x, y);
                else
                    WriteExtendedContextColor(bitWriter, image, x, y);
            }
        }

        bitWriter.Flush();

        using var writer = new BinaryWriterX(output, true);

        output.Position = 0;
        writer.Write((int)output.Length - 8);
        writer.Write(image.Width * image.Height * 3);

        return output.ToArray();
    }

    private static void WriteSimpleContextColor(BinaryBitWriter writer, Image<Bgr24> image, int x, int y)
    {
        Bgr24 pixel = image[x, y];
        Bgr24 prevPixel = y == 0 ? image[x - 1, y] : image[x, y - 1];

        if (prevPixel == pixel)
        {
            writer.WriteBit(1);
            return;
        }

        int bDiff = pixel.B - prevPixel.B;
        int gDiff = pixel.G - prevPixel.G;
        int rDiff = pixel.R - prevPixel.R;

        if (bDiff is >= -13 and <= 12 && gDiff is >= -13 and <= 12 && rDiff is >= -13 and <= 12)
        {
            int bLength = GetColorChannelDeltaLength(bDiff);
            int gLength = GetColorChannelDeltaLength(gDiff);
            int rLength = GetColorChannelDeltaLength(rDiff);

            if (bLength + gLength + rLength < 24)
            {
                // Write color delta's
                writer.WriteBits(2, 2);
                WriteColorChannelDelta(writer, bDiff);
                WriteColorChannelDelta(writer, gDiff);
                WriteColorChannelDelta(writer, rDiff);
                return;
            }
        }

        // Write raw pixel
        writer.WriteBits(0, 2);
        writer.WriteByte(pixel.B);
        writer.WriteByte(pixel.G);
        writer.WriteByte(pixel.R);
    }

    private static void WriteExtendedContextColor(BinaryBitWriter writer, Image<Bgr24> image, int x, int y)
    {
        Bgr24 pixel = image[x, y];
        Bgr24 leftPixel = image[x - 1, y];
        Bgr24 topLeftPixel = image[x - 1, y - 1];
        Bgr24 topPixel = image[x, y - 1];

        if (pixel == leftPixel)
        {
            writer.WriteBits(0xC, 4);
            return;
        }

        if (pixel == topLeftPixel)
        {
            writer.WriteBits(0, 6);
            return;
        }

        if (pixel == topPixel)
        {
            writer.WriteBits(0x20, 6);
            return;
        }

        if (leftPixel.B - topLeftPixel.B + topPixel.B == pixel.B &&
            leftPixel.G - topLeftPixel.G + topPixel.G == pixel.G &&
            leftPixel.R - topLeftPixel.R + topPixel.R == pixel.R)
        {
            writer.WriteBits(2, 2);
            return;
        }

        var bMinDiff = 24;
        var gMinDiff = 24;
        var rMinDiff = 24;
        var minDiffLength = 24;
        var mode = 0;
        var modeLength = 0;

        // Delta with left pixel
        int bDiff = pixel.B - leftPixel.B;
        int gDiff = pixel.G - leftPixel.G;
        int rDiff = pixel.R - leftPixel.R;

        if (bDiff is >= -13 and <= 12 && gDiff is >= -13 and <= 12 && rDiff is >= -13 and <= 12)
        {
            int bLength = GetColorChannelDeltaLength(bDiff);
            int gLength = GetColorChannelDeltaLength(gDiff);
            int rLength = GetColorChannelDeltaLength(rDiff);

            if (bLength + gLength + rLength < minDiffLength)
            {
                bMinDiff = bDiff;
                gMinDiff = gDiff;
                rMinDiff = rDiff;
                minDiffLength = bLength + gLength + rLength;
                mode = 4;
                modeLength = 4;
            }
        }

        // Delta top pixel
        bDiff = pixel.B - topPixel.B;
        gDiff = pixel.G - topPixel.G;
        rDiff = pixel.R - topPixel.R;

        if (bDiff is >= -13 and <= 12 && gDiff is >= -13 and <= 12 && rDiff is >= -13 and <= 12)
        {
            int bLength = GetColorChannelDeltaLength(bDiff);
            int gLength = GetColorChannelDeltaLength(gDiff);
            int rLength = GetColorChannelDeltaLength(rDiff);

            if (bLength + gLength + rLength < minDiffLength)
            {
                bMinDiff = bDiff;
                gMinDiff = gDiff;
                rMinDiff = rDiff;
                minDiffLength = bLength + gLength + rLength;
                mode = 0x30;
                modeLength = 6;
            }
        }

        // Delta top left pixel
        bDiff = pixel.B - topLeftPixel.B;
        gDiff = pixel.G - topLeftPixel.G;
        rDiff = pixel.R - topLeftPixel.R;

        if (bDiff is >= -13 and <= 12 && gDiff is >= -13 and <= 12 && rDiff is >= -13 and <= 12)
        {
            int bLength = GetColorChannelDeltaLength(bDiff);
            int gLength = GetColorChannelDeltaLength(gDiff);
            int rLength = GetColorChannelDeltaLength(rDiff);

            if (bLength + gLength + rLength < minDiffLength)
            {
                bMinDiff = bDiff;
                gMinDiff = gDiff;
                rMinDiff = rDiff;
                minDiffLength = bLength + gLength + rLength;
                mode = 0x10;
                modeLength = 6;
            }
        }

        // Delta top, top left, and left pixel
        bDiff = pixel.B - (leftPixel.B - topLeftPixel.B + topPixel.B);
        gDiff = pixel.G - (leftPixel.G - topLeftPixel.G + topPixel.G);
        rDiff = pixel.R - (leftPixel.R - topLeftPixel.R + topPixel.R);

        if (bDiff is >= -13 and <= 12 && gDiff is >= -13 and <= 12 && rDiff is >= -13 and <= 12)
        {
            int bLength = GetColorChannelDeltaLength(bDiff);
            int gLength = GetColorChannelDeltaLength(gDiff);
            int rLength = GetColorChannelDeltaLength(rDiff);

            if (bLength + gLength + rLength < minDiffLength)
            {
                bMinDiff = bDiff;
                gMinDiff = gDiff;
                rMinDiff = rDiff;
                minDiffLength = bLength + gLength + rLength;
                mode = 1;
                modeLength = 1;
            }
        }

        if (minDiffLength < 24)
        {
            writer.WriteBits(mode, modeLength);
            WriteColorChannelDelta(writer, bMinDiff);
            WriteColorChannelDelta(writer, gMinDiff);
            WriteColorChannelDelta(writer, rMinDiff);
            return;
        }

        // Write raw pixel
        writer.WriteBits(8, 4);
        writer.WriteByte(pixel.B);
        writer.WriteByte(pixel.G);
        writer.WriteByte(pixel.R);
    }

    private static int GetColorChannelDeltaLength(int diff)
    {
        return diff switch
        {
            0 => 1,
            -1 or 1 => 3,
            -2 or 2 or -3 => 4,
            3 or -4 or 4 or -5 or 5 or -6 or 6 => 7,
            -7 or 7 or -8 => 9,
            8 or -9 or 9 => 11,
            -10 or 10 or -11 => 13,
            _ => 15
        };
    }

    private static void WriteColorChannelDelta(BinaryBitWriter writer, int diff)
    {
        switch (diff)
        {
            case 0:
                writer.WriteBit(1);
                return;

            case -1:
                writer.WriteBits(0x4, 3);
                return;

            case 1:
                writer.WriteBits(0x2, 3);
                return;

            case -2:
                writer.WriteBits(0xE, 4);
                return;

            case 2:
                writer.WriteBits(0x6, 4);
                return;

            case -3:
                writer.WriteBits(0x8, 4);
                return;

            case 3:
                writer.WriteBits(0x70, 7);
                return;

            case -4:
                writer.WriteBits(0x30, 7);
                return;

            case 4:
                writer.WriteBits(0x50, 7);
                return;

            case -5:
                writer.WriteBits(0x10, 7);
                return;

            case 5:
                writer.WriteBits(0x60, 7);
                return;

            case -6:
                writer.WriteBits(0x20, 7);
                return;

            case 6:
                writer.WriteBits(0x40, 7);
                return;

            case -7:
                writer.WriteBits(0x180, 9);
                return;

            case 7:
                writer.WriteBits(0x80, 9);
                return;

            case -8:
                writer.WriteBits(0x100, 9);
                return;

            case 8:
                writer.WriteBits(0x600, 11);
                return;

            case -9:
                writer.WriteBits(0x200, 11);
                return;

            case 9:
                writer.WriteBits(0x400, 11);
                return;

            case -10:
                writer.WriteBits(0x1800, 13);
                return;

            case 10:
                writer.WriteBits(0x800, 13);
                return;

            case -11:
                writer.WriteBits(0x1000, 13);
                return;

            case 11:
                writer.WriteBits(0x6000, 15);
                return;

            case -12:
                writer.WriteBits(0x2000, 15);
                return;

            case 12:
                writer.WriteBits(0x4000, 15);
                return;

            case -13:
                writer.WriteBits(0, 15);
                return;
        }
    }
}