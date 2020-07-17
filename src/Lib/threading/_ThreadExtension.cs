using System.Threading;

namespace Lib.threading
{
    public static class _ThreadExtension
    {
        /// <summary>
        /// 取消Thread.Sleep状态，继续线程
        /// </summary>
        public static void CancelSleep(this Thread thread)
        {
            if (thread.ThreadState != ThreadState.WaitSleepJoin)
                return;

            thread.Interrupt();
        }
    }
}
