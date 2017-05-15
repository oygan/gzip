using System.Collections.Generic;
using System.IO;

namespace GZipLib.Common
{
    /// <summary>
    /// Этот класс реализует механизм работы с потоками. 
    /// Предназначен чтобы распараллеливать и синхронизировать задачи в многопроцессорной среде
    /// А также этот класс должен уметь обрабатывать файлы, размер которых превышает объем доступной оперативной памяти.
    /// </summary>
    class ThreadSchema
    {
        /// <summary>
        /// Поставщик работы
        /// </summary>
        private readonly IJobIterator _iterator;

        /// <summary>
        /// Создает экземпляр класса <see cref="ThreadSchema"/>
        /// </summary>
        /// <param name="iterator">Поставщик работы</param>
        public ThreadSchema(IJobIterator iterator)
        {
            _iterator = iterator;
        }

        /// <summary>
        /// Запускает потоки.
        /// </summary>
        /// <param name="outFilename">входной файл</param>
        /// <param name="workerCount">количество потоков</param>
        public void Run(string outFilename, int workerCount)
        {
            List<WorkerResult> completeWorks = new List<WorkerResult>();

            // создали N потоков
            List<ThreadWorker> workers = new List<ThreadWorker>();
            for (int i = 0; i < workerCount; i++)
            {
                ThreadWorker worker = new ThreadWorker();
                workers.Add(worker);
            }

            using (FileStream outStream = new FileStream(outFilename, FileMode.Create, FileAccess.Write))
            {
                while (true)
                {
                    for (int i = 0; i < workerCount; i++)
                    {
                        BaseJob job = _iterator.NextJob();

                        if (job != null)
                            workers[i].Start(job);
                        else
                            workers[i].Stop();
                    }

                    // пока работают потоки, пишем результаты в файл 
                    for (int i = 0; i < completeWorks.Count; i++)
                    {
                        byte[] data = completeWorks[i].JobData;
                        outStream.Write(data, 0, data.Length);
                    }

                    // ожидаем пока рабочии выполнят задачи
                    for (int i = 0; i < workers.Count; i++)
                    {
                        workers[i].WaitOne();
                    }

                    // собрали результаты работы
                    completeWorks = new List<WorkerResult>();
                    for (int i = 0; i < workers.Count; i++)
                    {
                        if (workers[i].Job != null && workers[i].Job.JobResult.JobData.Length > 0)
                            completeWorks.Add(workers[i].Job.JobResult);
                    }

                    if (completeWorks.Count == 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}