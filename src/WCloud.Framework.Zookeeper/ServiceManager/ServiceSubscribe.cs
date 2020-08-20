using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using org.apache.zookeeper;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.DataSerializer;
using WCloud.Framework.Zookeeper.watcher;

namespace WCloud.Framework.Zookeeper.ServiceManager
{
    /// <summary>
    /// 应该作为静态类
    /// </summary>
    public class ServiceSubscribe : ServiceSubscribeBase
    {
        private readonly Watcher _node_watcher;

        public event Func<Task> OnServiceChangedAsync;
        public event Func<Task> OnSubscribeFinishedAsync;

        private readonly IDataSerializer _serializer;

        public ServiceSubscribe(IServiceProvider provider, ILogger logger, string host) : base(logger, host)
        {
            this._serializer = provider.ResolveSerializer();
            this._node_watcher = new CallBackWatcher(e => this.WatchNodeChanges(e));

            //链接上了就获取服务信息
            this.OnConnectedAsync += this.Init;
            //打开链接
            this.CreateClient();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public async Task Init()
        {
            try
            {
                //清理无用节点
                await this.RetryAsync(async () => await this.ClearDeadNodes());
            }
            catch (Exception e)
            {
                var err = new Exception("清理无用节点失败", e);
                this.logger.AddErrorLog(err.Message, err);
            }

            try
            {
                //读取节点并添加监视
                await this.RetryAsync(async () => await this.DiscoverPathOrNodeThenWatch(this._base_path));
            }
            catch (Exception e)
            {
                var err = new Exception("订阅服务节点失败", e);
                this.logger.AddErrorLog(err.Message, err);
            }

            //订阅完成
            if (this.OnSubscribeFinishedAsync != null)
                await this.OnSubscribeFinishedAsync.Invoke();

            //订阅完成
            this._client_ready.Set();
        }

        /// <summary>
        /// 启动的时候清理一下无用节点
        /// 这个方法里不要watch
        /// </summary>
        private async Task ClearDeadNodes()
        {
            var services = await this.Client.GetNodeChildren(this._base_path);
            foreach (var service in services)
            {
                try
                {
                    var service_path = this._base_path + "/" + service;
                    var endpoints = await this.Client.GetNodeChildren(service_path);
                    if (ValidateHelper.IsEmpty(endpoints))
                        await this.Client.deleteAsync(service_path);
                }
                catch (Exception e)
                {
                    this.logger.AddErrorLog(e.Message, e);
                }
            }
        }

        /// <summary>
        /// 递归找到路径和节点，并做相应的监听处理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task DiscoverPathOrNodeThenWatch(string path)
        {
            try
            {
                if (this.IsServiceRootLevel(path))
                {
                    //qpl/wcf
                    //读取所有服务
                    var services = await this.Client.GetNodeChildren(path, this._node_watcher);
                    foreach (var service in services.Where(x => ValidateHelper.IsNotEmpty(x)))
                    {
                        //递归查找所有服务
                        var service_path = path + "/" + service;
                        await this.DiscoverPathOrNodeThenWatch(service_path);
                    }
                }
                else if (this.IsServiceLevel(path))
                {
                    //qpl/wcf/order
                    //读取服务的注册服务器
                    var endpoints = await this.Client.GetNodeChildren(path, this._node_watcher);
                    foreach (var endpoint in endpoints)
                        //处理节点
                        await this.GetEndpointData(path + "/" + endpoint);
                }
                else
                {
                    this.logger.LogInformation($"不能处理的节点{path}");
                }
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog($"订阅节点{path}失败", e);
            }
        }

        private async Task GetEndpointData(string path)
        {
            if (!this.IsEndpointLevel(path))
                return;
            try
            {
                var bs = await this.Client.GetDataOrThrow_(path, this._node_watcher);
                if (ValidateHelper.IsEmpty(bs))
                {
                    await this.Client.DeleteNodeRecursively_(path);
                    return;
                }
                var data = this._serializer.DeserializeFromBytes<AddressModel>(bs) ??
                    throw new ArgumentNullException("序列化address model错误");
                if (!ValidateHelper.IsAllNotEmpty(data.ServiceNodeName, data.EndpointNodeName, data.Url))
                    throw new ArgumentException($"address model数据错误:{data.ToJson()}");

                var service_info = this.GetServiceAndEndpointNodeName(path);
                data.ServiceNodeName = service_info.service_name;
                data.EndpointNodeName = service_info.endpoint_name;

                this._endpoints.RemoveWhere_(x => x.FullPathName == data.FullPathName);
                this._endpoints.Add(data);

                if (this.OnServiceChangedAsync != null)
                    await this.OnServiceChangedAsync.Invoke();
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog($"读取节点数据失败：{path}", e);
            }
        }

        private async Task DeleteEndpoint(string path)
        {
            if (!this.IsEndpointLevel(path))
                return;
            var data = this.GetServiceAndEndpointNodeName(path);

            this._endpoints.RemoveWhere_(x =>
            x.ServiceNodeName == data.service_name &&
            x.EndpointNodeName == data.endpoint_name);

            if (this.OnServiceChangedAsync != null)
            {
                await this.OnServiceChangedAsync.Invoke();
            }
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task WatchNodeChanges(WatchedEvent e)
        {
            var event_type = e.get_Type();
            var path = e.getPath();

            var path_level = path.SplitZookeeperPath().Count();
            if (path_level < this._base_path_level || path_level > this._endpoint_path_level)
            {
                this.logger.AddWarningLog($"节点无法被处理{path}");
                return;
            }

            //Console.WriteLine($"节点事件：{path}:{event_type}");

            switch (event_type)
            {
                case Watcher.Event.EventType.NodeChildrenChanged:
                    //子节点发生更改
                    await this.DiscoverPathOrNodeThenWatch(path);
                    break;

                case Watcher.Event.EventType.NodeDataChanged:
                    //单个节点数据发生修改
                    await this.GetEndpointData(path);
                    break;

                case Watcher.Event.EventType.NodeDeleted:
                    //单个节点被删除
                    await this.DeleteEndpoint(path);
                    break;

                case Watcher.Event.EventType.NodeCreated:
                //这里新增节点不用处理，因为已经watch了children change。
                //如果新增节点，上层就可以观测到
                case Watcher.Event.EventType.None:

                default:
                    break;
            }
        }
    }
}
