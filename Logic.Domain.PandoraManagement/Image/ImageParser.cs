using System.Buffers.Binary;
using Komponent.Contract.Enums;
using Komponent.IO;
using Logic.Domain.PandoraManagement.Contract.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Logic.Domain.PandoraManagement.Contract.Archive;
using Logic.Domain.PandoraManagement.Contract.Enums;

namespace Logic.Domain.PandoraManagement.Image;

internal class ImageParser(IFileDecompressor decompressor) : IImageParser
{
    public Image<Rgb24> Parse(byte[] data)
    {
        int compressionFormat = BinaryPrimitives.ReadInt32LittleEndian(data);
        int flavor = BinaryPrimitives.ReadInt32LittleEndian(data[4..]);

        int width = BinaryPrimitives.ReadInt32LittleEndian(data[0x10..]);
        int height = BinaryPrimitives.ReadInt32LittleEndian(data[0x14..]);

        if (flavor != -1)
            throw new InvalidOperationException($"Unknown image compression flavor {flavor}.");

        if (compressionFormat == 2)
        {
            var colors = new List<Rgb24> { new(data[0x22], data[0x21], data[0x20]) };

            var bitReader = new BinaryBitReader(new MemoryStream(data[0x23..]), BitOrder.LeastSignificantBitFirst, 1, Komponent.Contract.Enums.ByteOrder.LittleEndian);

            for (var h = 0; h < height; h++)
            {
                for (int w = h == 0 ? 1 : 0; w < width; w++)
                {
                    if (h == 0 || w == 0)
                        colors.Add(ReadSimpleContextColor(bitReader, h, width, colors));
                    else
                        colors.Add(ReadExtendedContextColor(bitReader, width, colors));
                }
            }

            return SixLabors.ImageSharp.Image.LoadPixelData([.. colors], width, height);
        }
        else
        {
            byte[] imageData = decompressor.DecompressBytes(new MemoryStream(data[0x18..]), FileCompression.Lzss01);

            var colors = new List<Rgb24>();
            for (var i = 0; i < width * height; i++)
                colors.Add(new Rgb24(imageData[i * 3 + 2], imageData[i * 3 + 1], imageData[i * 3]));

            return SixLabors.ImageSharp.Image.LoadPixelData([.. colors], width, height);
        }
    }

    private Rgb24 ReadSimpleContextColor(BinaryBitReader bitReader, int y, int width, List<Rgb24> colors)
    {
        if (bitReader.ReadBit() == 0)
        {
            if (bitReader.ReadBit() == 0)
            {
                // Write raw pixel
                var b = (byte)bitReader.ReadByte();
                var g = (byte)bitReader.ReadByte();
                var r = (byte)bitReader.ReadByte();

                return new Rgb24(r, g, b);
            }
            else
            {
                // Delta left or above pixel
                var pixel = y == 0 ? colors[^1] : colors[^width];
                var b = (byte)(pixel.B + ReadColorChannelDelta(bitReader));
                var g = (byte)(pixel.G + ReadColorChannelDelta(bitReader));
                var r = (byte)(pixel.R + ReadColorChannelDelta(bitReader));

                return new Rgb24(r, g, b);
            }
        }
        else
        {
            // Copy left or above pixel
            var pixel = y == 0 ? colors[^1] : colors[^width];
            return pixel;
        }
    }

    private Rgb24 ReadExtendedContextColor(BinaryBitReader bitReader, int width, List<Rgb24> colors)
    {
        if (bitReader.ReadBit() == 0)
        {
            if (bitReader.ReadBit() == 0)
            {
                switch (bitReader.ReadBits<int>(2))
                {
                    case 3:
                        // Copy left pixel
                        return colors[^1];

                    case 2:
                        // Write raw pixel
                        var b = (byte)bitReader.ReadByte();
                        var g = (byte)bitReader.ReadByte();
                        var r = (byte)bitReader.ReadByte();

                        return new Rgb24(r, g, b);

                    case 1:
                        var leftPixel = colors[^1];

                        var b1 = (byte)(leftPixel.B + ReadColorChannelDelta(bitReader));
                        var g1 = (byte)(leftPixel.G + ReadColorChannelDelta(bitReader));
                        var r1 = (byte)(leftPixel.R + ReadColorChannelDelta(bitReader));

                        return new Rgb24(r1, g1, b1);

                    // 0
                    default:
                        switch (bitReader.ReadBits<int>(2))
                        {
                            case 0:
                                // Copy above left pixel
                                return colors[colors.Count - width - 1];

                            case 2:
                                // Copy above pixel
                                return colors[^width];

                            case 3:
                                // above pixel + delta
                                var topPixel = colors[^width];

                                var b3 = (byte)(topPixel.B + ReadColorChannelDelta(bitReader));
                                var g3 = (byte)(topPixel.G + ReadColorChannelDelta(bitReader));
                                var r3 = (byte)(topPixel.R + ReadColorChannelDelta(bitReader));

                                return new Rgb24(r3, g3, b3);

                            // 1
                            default:
                                // above left pixel + delta
                                var topLeftPixel = colors[colors.Count - width - 1];

                                var b2 = (byte)(topLeftPixel.B + ReadColorChannelDelta(bitReader));
                                var g2 = (byte)(topLeftPixel.G + ReadColorChannelDelta(bitReader));
                                var r2 = (byte)(topLeftPixel.R + ReadColorChannelDelta(bitReader));

                                return new Rgb24(r2, g2, b2);
                        }
                }
            }
            else
            {
                // Left pixel - AboveLeft pixel + Above pixel
                var leftPixel = colors[^1];
                var topLeftPixel = colors[colors.Count - width - 1];
                var topPixel = colors[^width];

                var b = (byte)(leftPixel.B - topLeftPixel.B + topPixel.B);
                var g = (byte)(leftPixel.G - topLeftPixel.G + topPixel.G);
                var r = (byte)(leftPixel.R - topLeftPixel.R + topPixel.R);

                return new Rgb24(r, g, b);
            }
        }
        else
        {
            // Left pixel - AboveLeft pixel + Above pixel + delta
            var leftPixel = colors[^1];
            var topLeftPixel = colors[colors.Count - width - 1];
            var topPixel = colors[^width];

            var b = (byte)(leftPixel.B - topLeftPixel.B + topPixel.B + ReadColorChannelDelta(bitReader));
            var g = (byte)(leftPixel.G - topLeftPixel.G + topPixel.G + ReadColorChannelDelta(bitReader));
            var r = (byte)(leftPixel.R - topLeftPixel.R + topPixel.R + ReadColorChannelDelta(bitReader));

            return new Rgb24(r, g, b);
        }
    }

    private int ReadColorChannelDelta(BinaryBitReader reader)
    {
        if (reader.ReadBit() != 0)
            return 0;

        var part = reader.ReadBits<int>(2);
        if (part == 2)
            return -1;
        if (part == 1)
            return 1;

        part = part | (reader.ReadBits<int>(1) << 2);
        if (part == 7)
            return -2;
        if (part == 3)
            return 2;
        if (part == 4)
            return -3;

        part = part | (reader.ReadBits<int>(3) << 3);
        if (part == 0x38)
            return 3;
        if (part == 0x18)
            return -4;
        if (part == 0x28)
            return 4;
        if (part == 8)
            return -5;
        if (part == 0x30)
            return 5;
        if (part == 0x10)
            return -6;
        if (part == 0x20)
            return 6;

        part = part | (reader.ReadBits<int>(2) << 6);
        if (part == 0xC0)
            return -7;
        if (part == 0x40)
            return 7;
        if (part == 0x80)
            return -8;

        part = part | (reader.ReadBits<int>(2) << 8);
        if (part == 0x300)
            return 8;
        if (part == 0x100)
            return -9;
        if (part == 0x200)
            return 9;

        part = part | (reader.ReadBits<int>(2) << 10);
        if (part == 0xC00)
            return -10;
        if (part == 0x400)
            return 10;
        if (part == 0x800)
            return -11;

        part = part | (reader.ReadBits<int>(2) << 12);
        if (part == 0x3000)
            return 11;
        if (part == 0x1000)
            return -12;
        if (part == 0x2000)
            return 12;

        return -13;
    }
}