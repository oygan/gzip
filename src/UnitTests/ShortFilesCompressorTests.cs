using System;
using System.IO;
using GZipTest.Common;
using GZipTest.Compressor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Тестирует архивы маленьких размеров
    /// </summary>
    [TestClass]
    public class ShortFilesCompressorTests
    {
        [TestMethod]
        public void MultyThreadCompressorTest()
        {
            string sourceFilename = TestFolder.File("Emgu.chm");
            string archiveFilename = "!pack.Emgu.8_thread.gz";
            int errorType;
            using (GzipCompressor compressor = new GzipCompressor())
            {
                errorType = compressor.PackFile(new ArchiveSettings(sourceFilename, archiveFilename) {ThreadCount = 8});
            }
            Assert.AreEqual(errorType, 0);
            FileInfo fileInfo = new FileInfo(archiveFilename);
            Assert.AreEqual(fileInfo.Length, 11810196);
        }

        [TestMethod]
        public void OneThreadCompressorTest()
        {
            string sourceFilename = TestFolder.File("Emgu.chm");
            string archiveFilename = "!pack.Emgu.1_thread.gz";
            int errorType;
            using (GzipCompressor compressor = new GzipCompressor())
            {
                errorType = compressor.PackFile(new ArchiveSettings(sourceFilename, archiveFilename) {ThreadCount = 1});
            }
            Assert.AreEqual(errorType, 0);
            FileInfo fileInfo = new FileInfo(archiveFilename);
            Assert.AreEqual(fileInfo.Length, 11810196);
        }
    }
}
