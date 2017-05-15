using System.IO;
using System.IO.Compression;
using GZipLib.Common;

namespace GZipLib.Decompressor
{
    /// <summary>
    /// ������������� ��������� �������.
    /// </summary>
    class GzipDecompressWorker : BaseJob
    {
        /// <summary>
        /// ��� ������ �����.
        /// </summary>
        private readonly string _archiveFilename;

        /// <summary>
        /// ������� � ������� ������ �����������.
        /// </summary>
        private readonly long _blockPosition;

        /// <summary>
        /// ������� ��������� ������.
        /// </summary>
        /// <param name="archiveFilename">��� ������ �����.</param>
        /// <param name="blockPosition">������� � ������� ������ �����������.</param>
        public GzipDecompressWorker(string archiveFilename, long blockPosition)
        {
            _archiveFilename = archiveFilename;
            _blockPosition = blockPosition;
        }

        /// <summary>
        /// ������������� ��������� �������.
        /// </summary>
        public override void ProcessBlock()
        {
            // ��������� ����� �� ������
            using (FileStream inputStream = new FileStream(_archiveFilename, FileMode.Open, FileAccess.Read))
            using (MemoryStream outputStream = new MemoryStream())
            {
                // ������������� ������� � �������� ������������� ����
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

                // ������� � �������� ���������
                var result = new WorkerResult();
                result.JobData = outputStream.ToArray();
                WriteResult(result);
            }
        }
    }
}