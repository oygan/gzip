using System.IO;
using System.IO.Compression;
using GZipLib.Common;

namespace GZipLib.Decompressor
{
    /// <summary>
    /// Распаковывает отдельный кусочек.
    /// </summary>
    class GzipDecompressWorker : BaseJob
    {
        /// <summary>
        /// Имя архива файла.
        /// </summary>
        private readonly string _archiveFilename;

        /// <summary>
        /// Позиция с которой начать конвертацию.
        /// </summary>
        private readonly long _blockPosition;

        /// <summary>
        /// Создает экземпляр класса.
        /// </summary>
        /// <param name="archiveFilename">Имя архива файла.</param>
        /// <param name="blockPosition">Позиция с которой начать конвертацию.</param>
        public GzipDecompressWorker(string archiveFilename, long blockPosition)
        {
            _archiveFilename = archiveFilename;
            _blockPosition = blockPosition;
        }

        /// <summary>
        /// Распаковывает отдельный кусочек.
        /// </summary>
        public override void ProcessBlock()
        {
            // открываем поток на чтение
            using (FileStream inputStream = new FileStream(_archiveFilename, FileMode.Open, FileAccess.Read))
            using (MemoryStream outputStream = new MemoryStream())
            {
                // устанавливаем позицию и начинаем распоковывать блок
                inputStream.Position = _blockPosition;
                using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    byte[] outBuffer = new byte[4096];
                    while (true)
                    {
                        int bytesRead = zipStream.Read(outBuffer, 0, outBuffer.Length);
                        if (bytesRead > 0)
                            outputStream.Write(outBuffer, 0, bytesRead);
                        else
                            break;
                    }
                }

                // создать и записать результат
                var result = new WorkerResult();
                result.JobData = outputStream.ToArray();
                WriteResult(result);
            }
        }
    }
}