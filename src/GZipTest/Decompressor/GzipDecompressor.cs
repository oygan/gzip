using System;
using System.Collections.Generic;
using System.IO;
using GZipTest.Common;

namespace GZipTest.Decompressor
{
    /// <summary>
    /// Точка входа для алгоритмов извлечения файла из архива Gzip. Работает в многопоточном режиме. 
    /// Делит файл на части. И каждую часть отдает потокам на декомпрессию.
    /// Чтобы получить разделить файл на части, ищется специальная последовательность битов.
    /// </summary>
    public class GzipDecompressor : IJobIterator, IDisposable
    {
        /// <summary>
        /// Список текущих позиций, где начинются новые блоки, подгружаются по мере надобности.
        /// </summary>
        private List<long> _blockPositions;

        /// <summary>
        /// Текущий номер блока, локальная нумерация.
        /// </summary>
        private int _blockId;

        /// <summary>
        /// Имя файла архива.
        /// </summary>
        private string _archiveFilename;

        /// <summary>
        /// Поток файла архива.
        /// </summary>
        private FileStream _archiveStream;

        /// <summary>
        /// Используется для корректной реализации интерфейса <see cref="IDisposable"/>.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Извлекает файл из архива Gzip.
        /// </summary>
        /// <param name="setting">Входные параметры, для удобства собраны в класс</param>
        /// <returns>В случае успеха возвращает 0, при ошибке 1</returns>
        public int ExtractFile(ArchiveSettings setting)
        {
            try
            {
                _archiveFilename = setting.InputFilename;
                
                _archiveStream = new FileStream(_archiveFilename, FileMode.Open, FileAccess.Read);
                
                ThreadSchema schema = new ThreadSchema(this);
                schema.Run(setting.OutpuFilename, setting.ThreadCount);

                return 0;
            }
            catch (Exception ex)
            {
#if DEBUG
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
            // находим начало блока
            if (_blockPositions == null || _blockPositions.Count <= _blockId)
            {
                _blockPositions = ExtractBlocksPositions(_archiveStream, true);
                _blockId = 0;
            }

            if (_blockPositions.Count > _blockId)
            {
                // выделяем порцию работы
                GzipDecompressWorker worker = new GzipDecompressWorker(_archiveFilename, _blockPositions[_blockId]);
                _blockId++;
                return worker;
            }

            // когда файл дочитали до конца, выходим
            return null;
        }

        /// <summary>
        /// Определяет список позиций, где начинются новые блоки. 
        /// </summary>
        /// <param name="archiveStream">архив с уже установленной позицией с котрой искать позиции.</param>
        /// <param name="firstOnly">индикатор того, что нужно искать только несколько первых позиций.</param>
        /// <returns></returns>
        private List<long> ExtractBlocksPositions(FileStream archiveStream, bool firstOnly)
        {
            List<long> blockPositions = new List<long>();
            int headerBufferSize = 1024*8;
            while (archiveStream.Position < archiveStream.Length)
            {
                // читаем порцию данных
                byte[] headerBuffer = new byte[headerBufferSize];
                long prevPosition = archiveStream.Position;
                int readCount = archiveStream.Read(headerBuffer, 0, headerBufferSize);
                if (readCount > 0)
                {
                    int location = 0;
                    int offset = 0;
                    while (location >= 0)
                    {
                        // проверяем совпадение первых трех байтов
                        location = ByteUtils.SearchByte(headerBuffer, new byte[] {0x1F, 0x8B, 0x8}, offset);
                        if (location >= 0)
                        {
                            // уточняем заголовок, используя более полную спецификацию индикатора заголовка
                            bool isHeader = GzipHeader.IsHeaderCandidate(headerBuffer, location);
                            if (isHeader)
                            {
                                long startBlock = prevPosition + location;
                                blockPositions.Add(startBlock);
                            }
                            offset = location + 1;
                        }
                    }
                }
                else
                {
                    // выход, полностью дочитали
                    break;
                }

                // блоки с наложением, чтобы найти разделитель который попал на границу
                if (archiveStream.Position != archiveStream.Length && archiveStream.Position > 10)
                    archiveStream.Seek(-9, SeekOrigin.Current);

                // находим только несколько первых и выходим
                if (firstOnly && blockPositions.Count > 0)
                    break;
            }

            return blockPositions;
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
                _archiveStream.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~GzipDecompressor()
        {
            Dispose(false);
        }
    }
}