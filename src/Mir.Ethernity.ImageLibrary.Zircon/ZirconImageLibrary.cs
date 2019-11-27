using System;
using System.IO;

namespace Mir.Ethernity.ImageLibrary.Zircon
{
    public class ZirconImageLibrary : IImageLibrary
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;
        private ZirconImage[] _images;

        public IImage this[int number] => _images[number];

        public string Name { get; private set; }

        public int Length { get; private set; }

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
                for (var i = 0; i < _images.Length; i++)
                {
                    if (!br.ReadBoolean()) continue;
                    _images[i] = new ZirconImage(i, br, _reader);
                }
            }
        }
    }
}
