using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Mir.Ethernity.ImageLibrary.Zircon
{
    public class ZirconImage : IImage
    {
        private readonly int _offset;
        private readonly BinaryReader _fileReader;
        public ModificatorType Modificator { get; }

        public int Offset { get; set; }

        public ushort Width { get; }

        public ushort Height { get; }

        public short OffsetX { get; }

        public short OffsetY { get; }

       
        public ZirconImage(int offset, ushort width, ushort height, short offsetX, short offsetY, ModificatorType modificator, BinaryReader fileReader)
        {

            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));

            Offset = offset;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Modificator = modificator;
        }

        public ImageData GetData()
        {
            var size = CalculateImageDataSize(Width, Height);
            _fileReader.BaseStream.Seek(Offset, SeekOrigin.Begin);
            var buffer = _fileReader.ReadBytes(size);
            return new ImageData { Buffer = buffer, Type = ImageDataType.Dxt1 };
        }

        internal static int CalculateImageDataSize(ushort width, ushort height)
        {
            int w = width + (4 - width % 4) % 4;
            int h = height + (4 - height % 4) % 4;
            return (w * h) / 2;
        }
    }
}
