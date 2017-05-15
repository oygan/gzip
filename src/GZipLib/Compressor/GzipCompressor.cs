using System;
using System.IO;
using GZipLib.Common;

namespace GZipLib.Compressor
{
    /// <summary>
    /// Точка входа для алгоритмов сжатия файла с помощью System.IO.Compression.GzipStream.
    /// Файл делится на части и архивируется в многопоточно режиме.
    /// </summary>
    public class GzipCompressor : IJobIterator, IDisposable
    {
        /// <summary>
        /// Размер блоков для сжатия.
        /// Значение взято исходя их рекомендаций описанных 
        /// в этой статье https://code.logos.com/blog/2012/06/always-wrap-gzipstream-with-bufferedstream.html
        /// </summary>
        private readonly int _bufferSize = 8192;

        /// <summary>
        /// Поток из которого читаем данные.
        /// </summary>
        private FileStream _srcStream;

        /// <summary>
        /// Используется для корректной реализации интерфейса <see cref="IDisposable"/>.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Упаковывает файл алгоритмом GzipStream.
        /// </summary>
        /// <param name="settings">Входные параметры, для удобства собраны в класс</param>
        /// <returns>В случае успеха возвращает 0, при ошибке 1</returns>
        public int PackFile(ArchiveSettings settings)
        {
            try
            {
                // считываем настройки алгоритма
                string inputFilename = settings.InputFilename;
                string outpuFilename = settings.OutpuFilename;
                int workerCount = settings.ThreadCount;

                _srcStream = new FileStream(inputFilename, FileMode.Open, FileAccess.Read);

                ThreadSchema schema = new ThreadSchema(this);
                schema.Run(outpuFilename, workerCount);
                return 0;
            }
            catch (Exception ex)
            {
#if DEBUG
                // в режиме отладки, ошибку показывать можно
                Console.WriteLine("ERROR: " + ex.Message);
#endif
                return 1;
            }
        }

        /// <summary>
        /// Получить следующую порцию работы.
        /// </summary>
        /// <returns></returns>
        BaseJob IJobIterator.NextJob()
        {
            byte[] buffer = new byte[_bufferSize];
            int readCount = _srcStream.Read(buffer, 0, _bufferSize);
            BaseJob worker = null;
            if (readCount > 0)
            {
                worker = new GzipCompressWorker(buffer, readCount);
            }
            return worker;
        }

        /// <summary>
        /// Высвободить ресурс.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Корректная реализация высвобождения.
        /// </summary>
        /// <param name="disposing">значение true - означает что высвобождение из метдоа  IDisposable.Dispose </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                _srcStream.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~GzipCompressor()
        {
            Dispose(false);
        }
    }
}