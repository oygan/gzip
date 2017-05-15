using System.Threading;

namespace GZipTest.Common
{
    /// <summary>
    /// "Обертка" для потока, для выполнения поставленых задач. 
    /// </summary>
    public class ThreadWorker
    {
        /// <summary>
        /// Поток, в котором совершается работа. 
        /// </summary>
        private readonly Thread _jobThread;

        /// <summary>
        /// Позволяет выполнять только одну задачу за раз. 
        /// </summary>
        private readonly ManualResetEvent _signal;

        /// <summary>
        /// Блокирует холостую работу. 
        /// </summary>
        private ManualResetEvent _selfSignal;

        /// <summary>
        /// Задача для выполнения. 
        /// </summary>
        private BaseJob _job;

        /// <summary>
        /// Задача для выполнения(открываем доступ только на чтение). 
        /// </summary>
        public BaseJob Job
        {
            get { return _job; }
        }
        public ThreadWorker()
        {
            _jobThread = new Thread(ProcessJob);
            _signal = new ManualResetEvent(false);
            _selfSignal = new ManualResetEvent(false);
        }

        /// <summary>
        /// Дает потоку работу на исполнение.
        /// </summary>
        public void Start(BaseJob job)
        {
            // остановить другие потоки, пока выполняется работа
            _signal.Reset();

            // установить задачу
            _job = job;
            if (_jobThread.ThreadState == ThreadState.Unstarted)
                _jobThread.Start();

            // разблокировать вунтренний цикл
            _selfSignal.Set();
        }

        /// <summary>
        /// Останавливат работу
        /// </summary>
        public void Stop()
        {
            if (_selfSignal == null)
                return;

            _signal.Reset();
            _job = null;
            _selfSignal.Set();
        }

        /// <summary>
        /// Цикл ожидания и выполнения задач
        /// </summary>
        private void ProcessJob()
        {
            while (true)
            {
                // блокируем холостую работу в ожидании следующей порции работы
                _selfSignal.WaitOne();

                // работы нет, завершаем 
                if (_job == null)
                {
                    _selfSignal.Set();
                    _signal.Set();
                    break;
                }

                // Выполнить задачу
                _job.ProcessBlock();

                // исключаем холостой проход
                _selfSignal.Reset();

                // готовы к приему новых задач
                _signal.Set();
            }
            _selfSignal = null;
        }

        /// <summary>
        /// Блокирует текущий поток, пока текущий дескриптор ожидания не примет сигнал.
        /// </summary>
        public void WaitOne()
        {
            _signal.WaitOne();
        }
    }
}