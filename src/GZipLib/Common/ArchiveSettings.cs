namespace GZipLib.Common
{
    /// <summary>
    /// Класс с параметрами настроек для методов сжатия и разжатия файлов. 
    /// Группирует параметры, не должен содержать логики.
    /// </summary>
    public class ArchiveSettings
    {
        /// <summary>
        /// Имя входного файла.
        /// </summary>
        public string InputFilename { get; set; }

        /// <summary>
        /// Имя выходного файла.
        /// </summary>
        public string OutpuFilename { get; set; }

        /// <summary>
        /// Количество потоков, которое будет использоваться.
        /// </summary>
        public int ThreadCount { get; set; } = 4;

        /// <summary>
        /// Создает экземпляр класса <see cref="ArchiveSettings"/>
        /// </summary>
        /// <param name="inputFilename">имя входного файла</param>
        /// <param name="outpuFilename">имя выходного файла</param>
        public ArchiveSettings(string inputFilename, string outpuFilename)
        {
            InputFilename = inputFilename;
            OutpuFilename = outpuFilename;
        }
    }
}