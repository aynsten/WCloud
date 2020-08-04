using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lib.distributed;
using Lib.extension;
using Lib.helper;
using Lib.zookeeper.watcher;
using Microsoft.Extensions.Logging;
using org.apache.zookeeper;

namespace Lib.zookeeper
{
    /// <summary>
    /// 有一个极端环境为解决
    /// 第一个节点获取锁的同时，第二个节点获取children，然后准备watch第一个节点
    /// 这时候第一个节点用完锁并删除自己的节点，第二个节点watch失败，就会导致后面的节点卡死
    /// </summary>
    public class ZooKeeperDistributedLock : IDistributedLock
    {
        private string _lock_id;

        private readonly string _lock_path;

        private readonly IServiceProvider provider;
        private readonly ILogger logger;
        private readonly ZooKeeperClient _client;
        private readonly TimeSpan _timeout;

        private readonly AutoResetEvent _reset = new AutoResetEvent(initialState: false);

        private readonly Watcher _watcher;

        private ZooKeeper Client => _client.Client;
        public string LockID => this._lock_id;

        public ZooKeeperDistributedLock(IServiceProvider provider,
            ZooKeeperClient client, string base_path, string lock_name, TimeSpan? timeout = null)
        {
            this.provider = provider;
            this.logger = this.provider.ResolveLogger<ZooKeeperDistributedLock>();
            this._client = client ?? throw new ArgumentNullException(nameof(client));
            this._lock_path = base_path.SplitZookeeperPath().ToList().AddItem_(lock_name).AsZookeeperPath();

            this._watcher = new CallBackWatcher(this.WatcherCallback);
            this._timeout = timeout ?? TimeSpan.FromSeconds(60);
        }

        public async Task LockOrThrow()
        {
            await this.Client.EnsurePersistentPath(this._lock_path);

            this._lock_id = await this.Client.CreateSequentialPath_($"{this._lock_path}/locker", persistent: false);

            await this.__get_lock_or_watch_previous_node__();

            var success = this._reset.WaitOne(timeout: this._timeout);
            if (!success)
            {
                throw new TimeoutException("等待锁超时");
            }
        }

        async Task __get_lock_or_watch_previous_node__()
        {
            //并发枪锁这里效率比较低
            var children = await this.Client.GetNodeChildren(this._lock_path);
            children = children.OrderBy(x => x).ToList();

            var index = children.IndexOf(this._lock_id);
            if (index <= 0)
            {
                //释放获取锁的信号
                this._reset.Set();
            }
            else
            {
#if DEBUG
                this.logger.LogInformation("setup watch" + DateTime.UtcNow.Ticks);
#endif
                var success = await this.Client.ExistAsync_($"{this._lock_path}/{children[index - 1]}", this._watcher);
                if (!success)
                {
                    //watch失败，有可能前序节点被删除，重新尝试watch
                    //await this.__get_lock_or_watch_previous_node__();
                    this._reset.Set();
                }
            }
        }

        async Task WatcherCallback(WatchedEvent @event)
        {
#if DEBUG
            this.logger.LogInformation("watch callback");
#endif
            if (@event.get_Type() == org.apache.zookeeper.Watcher.Event.EventType.NodeDeleted)
            {
                await this.__get_lock_or_watch_previous_node__();
            }
        }

        public async Task ReleaseLock()
        {
            try
            {
                if (ValidateHelper.IsNotEmpty(this._lock_id))
                {
                    await this.Client.deleteAsync(this._lock_path + "/" + this._lock_id);
                }
            }
            catch (Exception e)
            {
                logger.AddWarningLog(e: e, msg: e.Message);
            }
            this._lock_id = null;
        }

        public void Dispose()
        {
            try
            {
                AsyncHelper_.RunSync(() => this.ReleaseLock());
            }
            catch (Exception e)
            {
                logger.AddWarningLog(e: e, msg: e.Message);
            }

            try
            {
                this._reset.Dispose();
            }
            catch (Exception e)
            {
                logger.AddWarningLog(e: e, msg: e.Message);
            }
        }
    }
}
