// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENCE in the project root for license information.

using Lib.extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCloud.Core.DataSerializer;
using WCloud.Framework.Zookeeper.watcher;

namespace WCloud.Framework.Zookeeper.configuration
{
    internal sealed class ZKConfigurationProvider : ConfigurationProvider, IConfigurationProvider
    {
        private IDictionary<string, string> PathData;

        private readonly ZKConfigurationOption _option;
        private readonly AlwaysOnZooKeeperClient _client;
        private readonly IDataSerializer _serializer;
        private readonly Encoding _encoding;
        private readonly Watcher _node_watcher;

        public ZKConfigurationProvider(ZKConfigurationOption option, AlwaysOnZooKeeperClient client,
            IDataSerializer serializeProvider = null,
            Encoding encoding = null)
        {
            this._option = option;
            this._client = client;
            this._serializer = serializeProvider;
            this._encoding = encoding ?? Encoding.UTF8;

            this._node_watcher = new CallBackWatcher(this.NodeWatchCallback);
        }

        public override void Load()
        {
            var base_path = this._option.BasePath;
            var max_deep = this._option.MaxDeep ?? 10;

            this.PathData = new Dictionary<string, string>();
            var node_history = new List<string>();

            async Task load_(string parent_path, string path, int deep)
            {
                var current_deep = deep;
                if (current_deep > max_deep)
                    return;

                var current_full_path = new string[] { parent_path, path }.AsZookeeperPath();

                if (node_history.Contains(current_full_path))
                    throw new ArgumentException("递归异常");
                node_history.Add(current_full_path);

                try
                {
                    var data = await this._client.Client.getDataAsync(current_full_path, watcher: null);
                    if (data?.Data?.Any() ?? false)
                    {
                        //当前节点有数据
                        var bs = data.Data;
                        var str_data = this._encoding.GetString(bs);
                        this.PathData[current_full_path] = str_data;
                    }

                    var children = await this._client.Client.getChildrenAsync(current_full_path);
                    if (children?.Children?.Any() ?? false)
                    {
                        var children_deep = current_deep + 1;
                        foreach (var child in children.Children)
                            //find next level
                            await load_(parent_path: current_full_path, path: child, deep: children_deep);
                    }
                }
                catch (Exception e)
                {
                    var context_instance = IocContext.Instance;
                    if (context_instance.Inited)
                    {
                        using (var s = context_instance.Scope())
                        {
                            var logger = s.ServiceProvider.ResolveLogger<ZKConfigurationProvider>();
                            logger.AddErrorLog(e.Message, e);
                        }
                    }
                }
            }

            var job = load_(parent_path: string.Empty, path: base_path, deep: 0);
            Task.Run(async () => await job).Wait();

            var dict = new Dictionary<string, string>();
            foreach (var m in this.PathData)
                dict[parse_path(m.Key)] = m.Value;

            this.Data = dict;
        }

        string parse_path(string zk_path) => string.Join(":", zk_path.Split('/', '\\').Where(x => x?.Length > 0));

        /// <summary>
        /// 节点改变会被通知到
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        async Task NodeWatchCallback(WatchedEvent @event)
        {
            var base_path = this._option.BasePath;

            var path = @event.getPath();
            var type = @event.get_Type();

            switch (type)
            {
                case Watcher.Event.EventType.NodeChildrenChanged:
                case Watcher.Event.EventType.NodeDataChanged:
                case Watcher.Event.EventType.NodeDeleted:

                default: break;
            }

            await Task.CompletedTask;
        }

    }
}