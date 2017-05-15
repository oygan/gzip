namespace GZipTest.Common
{
    /// <summary>
    /// Содержит результат работы потока
    /// </summary>
    public class WorkerResult
    {
        /// <summary>
        /// Массив сконвертированных байтов
        /// </summary>
        public byte[] JobData { get; set; }
    }
}