using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WCloud.Framework.Zookeeper
{
    public class AlwaysOnZooKeeperClient : ZooKeeperClient
    {
        /// <summary>
        /// 尝试再次链接，也许还没连上
        /// </summary>
        public event Func<Task> OnRecconectingAsync;

        public AlwaysOnZooKeeperClient(ILogger logger, string host) : base(logger, host)
        {
            //只有session过期才重新创建client，否则等待client自动尝试重连
            this.OnSessionExpiredAsync += this.ReConnect;
        }

        protected async Task ReConnect()
        {
            if (this.IsDisposing)
            {
                //销毁的时候取消重试链接
                return;
            }

            this.CloseClient();
            this.CreateClient();

            if (this.OnRecconectingAsync != null)
                await this.OnRecconectingAsync.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
