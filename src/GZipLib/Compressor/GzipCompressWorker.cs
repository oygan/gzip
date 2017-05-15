using System.IO;
using System.IO.Compression;
using GZipLib.Common;

namespace GZipLib.Compressor
{
    /// <summary>
    /// Сжимает отдельную часть файла.
    /// </summary>
    class GzipCompressWorker : BaseJob
    {
        /// <summary>
        /// Длина кусочка для сжатия.
        /// </summary>
        private readonly int _count;

        /// <summary>
        /// Буфер данных
        /// </summary>
        private readonly byte[] _buffer;

        /// <summary>
        /// Создает экземпляр класса.
        /// </summary>
        /// <param name="buffer">буфер данных.</param>
        /// <param name="count"> Длинна для сжатия.</param>
        public GzipCompressWorker(byte[] buffer, int count)
        {
            _count = count;
            _buffer = buffer;
        }

        /// <summary>
        /// Сжимает отдельную часть файла
        /// </summary>
        public override void ProcessBlock()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream inStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    inStream.Write(_buffer, 0, _count);
                }

                // создать и записать результат
                var result = new WorkerResult();
                result.JobData = stream.ToArray();
                WriteResult(result);
            }
        }
    }
}