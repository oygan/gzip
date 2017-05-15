using System;

namespace GZipTest.Decompressor
{
    /// <summary>
    /// Спецификация Gzip заголовока
    /// </summary>
    public class GzipHeader
    {
        private const int Id1 = 0x1F;
        private const int Id2 = 0x8B;
        private const int DeflateCompression = 0x8;
        private const int MaxGzipFlag = 32;

        /// <summary>
        /// Проверяет буфер, начиная с указанной позиции, на соответствие Спецификация Gzip заголовока
        /// </summary>
        /// <param name="buffer">Буфер</param>
        /// <param name="location">Позиция</param>
        /// <returns></returns>
        public static bool IsHeaderCandidate(byte[] buffer, int location)
        {
            if (buffer.Length < location + 10)
            {
                return false;
            }

            // берем первые 10 элементов
            byte[] header = new byte[10];
            Array.Copy(buffer, location, header, 0, 10);

            // Check the id tokens and compression algorithm
            if (header[0] != Id1 || header[1] != Id2 || header[2] != DeflateCompression)
            {
                return false;
            }

            // Extract the GZIP flags, of which only 5 are allowed (2 pow. 5 = 32)
            if (header[3] > MaxGzipFlag)
            {
                return false;
            }

            // Check the extra compression flags, which is either 2 or 4 with the Deflate algorithm
            if (header[8] != 0x0 && header[8] != 0x2 && header[8] != 0x4)
            {
                return false;
            }

            return true;
        }
    }
}