using System;
using System.IO;
using GZipLib.Common;
using GZipLib.Compressor;
using GZipLib.Decompressor;

namespace GZipLib
{
    /// <summary>
    /// Реализем консольный интерфейс.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Параметры программы.</param>
        static void Main(string[] args)
        {
            // Установим обработчики события при остановке программы по Ctrl-C.
            Console.CancelKeyPress += HandleCancelKeyPress;

            // Анализируем аргументы и запускаем соответствующий алгоритм.
            int result = 1;
            if (args.Length == 3)
            {
                // первый параметр приводим к нижнему регистру, будем более гибкими к параметру.
                string cmd = args[0];
                cmd = cmd.ToLower();

                switch (cmd)
                {
                    case "compress":
                    {
                        // компрессия файла
                        string sourceFilename = args[1];
                        string archiveFilename = args[2];
                        archiveFilename = Path.GetFullPath(archiveFilename);
                        sourceFilename = Path.GetFullPath(sourceFilename);
                            result = ExecuteCompress(sourceFilename, archiveFilename);
                        break;
                    }
                    case "decompress":
                    {
                        // декомпрессия файла
                        string archiveFilename = args[1];
                        string unpackedFilename = args[2];
                        archiveFilename = Path.GetFullPath(archiveFilename);
                        unpackedFilename = Path.GetFullPath(unpackedFilename);
                        result = ExecuteDecompress(archiveFilename, unpackedFilename);
                        break;
                    }
                    default:
                        Console.WriteLine("Unsupported command.");
                        break;
                }
            }
            else
            {
                // выведем набор ожидаемых аргументов
                Console.WriteLine("You must specify the parameters, please.");
                Console.WriteLine("Compression parameters: compress [source-file] [archive-file]");
                Console.WriteLine("Decompression parameters: decompress [archive-file] [unpacked-file]");
            }

            // вывод результата, в случае успеха программа возвращает 0, при ошибке 1.
            Console.WriteLine();
            Console.WriteLine("Result:" + result);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Сжать файл.
        /// </summary>
        /// <param name="sourceFilename">Имя исходного файла.</param>
        /// <param name="archiveFilename">Имя файла архива.</param>
        /// <returns>В случае успеха функция возвращает 0, при ошибке  1.</returns>
        private static int ExecuteCompress(string sourceFilename, string archiveFilename)
        {
            if (!File.Exists(sourceFilename))
            {
                Console.WriteLine("File not found:" + sourceFilename);
                return 1;
            }

            int errorType;
            using (GzipCompressor compressor = new GzipCompressor())
            {
                errorType = compressor.PackFile(new ArchiveSettings(sourceFilename, archiveFilename));
            }
            return errorType;
        }

        /// <summary>
        /// Извлечь файл из архива.
        /// </summary>
        /// <param name="archiveFilename">Имя файла архива.</param>
        /// <param name="unpackedFilename">Имя файла для распоковки</param>
        /// <returns>В случае успеха функция возвращает 0, при ошибке  1.</returns>
        public static int ExecuteDecompress(string archiveFilename, string unpackedFilename)
        {
            if (!File.Exists(archiveFilename))
            {
                Console.WriteLine("File not found:" + archiveFilename);
                return 1;
            }


            int errorType;
            using (GzipDecompressor decompressor = new GzipDecompressor())
            {
                errorType = decompressor.ExtractFile(new ArchiveSettings(archiveFilename, unpackedFilename));
            }
            return errorType;
        }

        /// <summary>
        /// Реакция на Ctrl-C.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="eventArgs">eventArgs</param>
        private static void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs eventArgs)
        {
            Console.WriteLine();
            Console.WriteLine("Cancel key was pressed");
            eventArgs.Cancel = false;
        }
    }
}
