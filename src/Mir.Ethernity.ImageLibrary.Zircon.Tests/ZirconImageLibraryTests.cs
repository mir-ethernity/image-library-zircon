using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Mir.Ethernity.ImageLibrary.Zircon.Tests
{
    [TestClass]
    public class ZirconImageLibraryTests
    {
        [TestMethod]
        public void LoadIndexLibrary()
        {
            using (var fs = File.OpenRead("./Resources/Equip.Zl"))
            {
                var lib = new ZirconImageLibrary("Equip", fs);
                Assert.AreEqual(6250, lib.Length);
                Assert.AreEqual("Equip", lib.Name);
            }
        }

        [TestMethod]
        public void LoadImageCompressed()
        {
            using (var fs = File.OpenRead("./Resources/Interface1c.Zl"))
            {
                var lib = new ZirconImageLibrary("Interface1c", fs);
                var image = lib[2200];
                var data = image.GetData(ImageType.Image);

                Assert.AreEqual(ImageDataType.Dxt1, data.Type);
                Assert.AreEqual((image.Image.Width * image.Image.Height) / 2, data.Buffer.Length);
            }
        }
    }
}
