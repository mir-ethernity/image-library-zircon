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
        public int Index { get; private set; }

        public ImageContent Image { get; private set; }
        public ShadowContent Shadow { get; private set; }
        public OverlayContent Overlay { get; private set; }

        public ImageData GetData(ImageType type)
        {
            var size = NormalizeSize(Image.Width, Image.Height);
            var offset = _offset;

            if(type == ImageType.Shadow || type == ImageType.Overlay)
            {
                offset += size;
                size = NormalizeSize(Shadow.Width, Shadow.Height);
            }

            if(type == ImageType.Overlay)
            {
                offset += size;
                size = NormalizeSize(Overlay.Width, Overlay.Height);
            }
            
            _fileReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            var buffer = _fileReader.ReadBytes(size);

            return new ImageData { Buffer = buffer, Type = ImageDataType.Dxt1 };
        }

        public ZirconImage(int index, BinaryReader headerReader, BinaryReader fileReader)
        {
            Index = index;
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));

            if (headerReader == null) throw new ArgumentNullException(nameof(fileReader));
            _offset = headerReader.ReadInt32();

            Image = new ImageContent
            {
                Width = headerReader.ReadUInt16(),
                Height = headerReader.ReadUInt16(),
                OffsetX = headerReader.ReadInt16(),
                OffsetY = headerReader.ReadInt16(),
            };

            var shadowType = headerReader.ReadByte();
            Shadow = new ShadowContent
            {
                Type = shadowType == 177 || shadowType == 176 || shadowType == 49 ? ShadowType.Transform : ShadowType.Opacity,
                Width = headerReader.ReadUInt16(),
                Height = headerReader.ReadUInt16(),
                OffsetX = headerReader.ReadInt16(),
                OffsetY = headerReader.ReadInt16(),
            };

            Overlay = new OverlayContent
            {
                Width = headerReader.ReadUInt16(),
                Height = headerReader.ReadUInt16(),
                OffsetX = Image.OffsetX,
                OffsetY = Image.OffsetY,
            };
        }


        private int NormalizeSize(ushort width, ushort height)
        {
            int w = width + (4 - width % 4) % 4;
            int h = height + (4 - height % 4) % 4;
            return w * h;
        }
    }
}
