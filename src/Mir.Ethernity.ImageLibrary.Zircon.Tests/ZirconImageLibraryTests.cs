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
                lib.Initialize();
                Assert.AreEqual(6250, lib.Count);
                Assert.AreEqual("Equip", lib.Name);
            }
        }
    }
}
