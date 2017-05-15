using System.IO;

namespace UnitTests
{
    /// <summary>
    /// Пути к тестовым данным
    /// </summary>
    class TestFolder
    {
        private const string TEST_FOLDER = @"..\..\test-data\";

        /// <summary>
        /// Получить имя файла из тестовой папки.
        /// </summary>
        /// <param name="name">Короткое имя файла.</param>
        /// <returns>Полное имя.</returns>
        public static string File(string name)
        {
            return Path.GetFullPath(TEST_FOLDER + name);
        }
    }
}