using System;
using System.IO;
using GZipTest.Common;
using GZipTest.Decompressor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Тестирует архивы маленьких размеров
    /// </summary>
    [TestClass]
    public class ShortFilesDecompressorTests
    {
        [TestMethod]
        public void MultyThreadDecompressorTest()
        {
            string archiveFilename = TestFolder.File("Emgu.gz");
            string unpackedFilename = "!Emgu.8_thread.chm";
            GzipDecompressor compressor = new GzipDecompressor();
            int errorType = compressor.ExtractFile(new ArchiveSettings(archiveFilename, unpackedFilename) { ThreadCount = 8 });
            Assert.AreEqual(errorType, 0);
            FileInfo fileInfo = new FileInfo(archiveFilename);
            Assert.AreEqual(fileInfo.Length, 11676162);
        }

        [TestMethod]
        public void OneThreadDecompressorTest()
        {
            string archiveFilename = TestFolder.File("Emgu.gz");
            string unpackedFilename = "!Emgu.1_thread.chm";
            GzipDecompressor compressor = new GzipDecompressor();
            int errorType = compressor.ExtractFile(new ArchiveSettings(archiveFilename, unpackedFilename) { ThreadCount = 1 });
            Assert.AreEqual(errorType, 0);
            FileInfo fileInfo = new FileInfo(archiveFilename);
            Assert.AreEqual(fileInfo.Length, 11676162);
        }
    }
}
