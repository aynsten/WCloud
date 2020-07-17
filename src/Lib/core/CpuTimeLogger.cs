using System;
using System.Diagnostics;

namespace Lib.core
{
    /// <summary>
    /// 运行计时
    /// </summary>
    public class CpuTimeLogger : IDisposable
    {
        private readonly Stopwatch timer = new Stopwatch();

        public Action<long> OnStop { get; set; }

        public CpuTimeLogger(Action<long> OnStop)
        {
            this.OnStop = OnStop;

            this.timer.Start();
        }

        public void Dispose()
        {
            this.timer.Stop();
            this.OnStop?.Invoke(this.timer.ElapsedMilliseconds);
        }
    }
}
