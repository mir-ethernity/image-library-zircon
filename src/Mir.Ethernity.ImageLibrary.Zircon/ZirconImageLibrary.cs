using System;
using System.IO;

namespace Mir.Ethernity.ImageLibrary.Zircon
{
    public class ZirconImageLibrary : IImageLibrary
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;

        private ZirconImage[] _images;
        private ZirconImage[] _shadows;
        private ZirconImage[] _overlays;

        public IImage[] Images { get => _images; }
        public IImage[] Shadows { get => _shadows; }
        public IImage[] Overlays { get => _overlays; }

        public string Name { get; private set; }

        public int Length { get => _images?.Length ?? 0; }

        public ZirconImageLibrary(string name, Stream stream)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _reader = new BinaryReader(_stream);

            InitializeLibrary();
        }

        public ZirconImageLibrary(string zlPath)
            : this(Path.GetFileNameWithoutExtension(zlPath), new FileStream(zlPath, FileMode.Open, FileAccess.Read))
        {
        }

        public void Dispose()
        {
            _stream.Dispose();
            _reader.Dispose();
            _images = null;
        }

        private void InitializeLibrary()
        {
            _stream.Seek(0, SeekOrigin.Begin);

            var headerBufferSize = _reader.ReadInt32();
            var headerBuffer = _reader.ReadBytes(headerBufferSize);

            using (var ms = new MemoryStream(headerBuffer))
            using (var br = new BinaryReader(ms))
            {
                var count = br.ReadInt32();

                _images = new ZirconImage[count];
                _shadows = new ZirconImage[count];
                _overlays = new ZirconImage[count];

                for (var i = 0; i < _images.Length; i++)
                {
                    if (!br.ReadBoolean()) continue;

                    var offset = br.ReadInt32();

                    var width = br.ReadUInt16();
                    var height = br.ReadUInt16();
                    var offsetX = br.ReadInt16();
                    var offsetY = br.ReadInt16();

                    var shadowTypeByte = br.ReadByte();
                    var shadowModificatorType = shadowTypeByte == 177 || shadowTypeByte == 176 || shadowTypeByte == 49
                        ? ModificatorType.Transform
                        : ModificatorType.Opacity;

                    var shadowWidth = br.ReadUInt16();
                    var shadowHeight = br.ReadUInt16();
                    var shadowOffsetX = br.ReadInt16();
                    var shadowOffsetY = br.ReadInt16();

                    var overlayWidth = br.ReadUInt16();
                    var overlayHeight = br.ReadUInt16();

                    _images[i] = new ZirconImage(offset, width, height, offsetX, offsetX, ModificatorType.None, _reader);

                    offset += ZirconImage.CalculateImageDataSize(width, height);

                    if (shadowWidth > 0 && shadowHeight > 0)
                    {
                        _shadows[i] = new ZirconImage(offset, shadowWidth, shadowHeight, shadowOffsetX, shadowOffsetY, shadowModificatorType, _reader);
                        offset += ZirconImage.CalculateImageDataSize(shadowWidth, shadowHeight);
                    }

                    if (overlayWidth > 0 && overlayHeight > 0)
                    {
                        _overlays[i] = new ZirconImage(offset, overlayWidth, overlayHeight, offsetX, offsetY, ModificatorType.None, _reader);
                    }
                }
            }
        }
    }
}
