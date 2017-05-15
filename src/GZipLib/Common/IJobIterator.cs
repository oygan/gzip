namespace GZipLib.Common
{
    /// <summary>
    /// Позволяет получить следующую порцию работы. 
    /// </summary>
    interface IJobIterator
    {
        /// <summary>
        /// Следующая порция работы
        /// </summary>
        /// <returns></returns>
        BaseJob NextJob();
    }
}