namespace GZipLib.Common
{
    /// <summary>
    /// Выполняемая задача\работа. Потомки конкретезируют задачу.
    /// </summary>
    public class BaseJob
    {
        /// <summary>
        /// Результат работы потока.
        /// </summary>
        public WorkerResult JobResult { get; private set; }

        /// <summary>
        /// Этот метод выполняется в созданом потоке. 
        /// Чтобы уточнить работу, потомки должны переопределять этот метод. 
        /// </summary>
        public virtual void ProcessBlock()
        {

        }

        /// <summary>
        /// Сохранить результаты работы
        /// </summary>
        /// <param name="result"></param>
        public void WriteResult(WorkerResult result)
        {
            JobResult = result;
        }
    }
}