using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Lib.extension;
using Lib.helper;
using org.apache.zookeeper;
using static org.apache.zookeeper.ZooDefs;

namespace WCloud.Framework.Zookeeper
{
    /// <summary>
    /// 删除部分无用扩展
    /// </summary>
    public static class ZooKeeperClientExtension
    {
        static void __check_path__(string path)
        {
            path.Should().NotBeNullOrEmpty("zookeeper路径为空");
            path.StartsWith("/").Should().BeTrue("zookeeper路径必须斜杠开头");
        }

        public static string[] SplitZookeeperPath(this string path)
        {
            var res = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            return res;
        }

        public static string AsZookeeperPath(this IEnumerable<string> path)
        {
            var valid_path = path.Where(x => x?.Length > 0).ToArray();
            var res = "/";
            if (valid_path.Any())
            {
                res += string.Join("/", path.Where(x => x?.Length > 0));
            }
            return res;
        }

        public static async Task<string> CreateNode_(this ZooKeeper client, string path, CreateMode mode, byte[] data = null)
        {
            __check_path__(path);
            var res = await client.createAsync(path, data, Ids.OPEN_ACL_UNSAFE, mode);
            if (ValidateHelper.IsEmpty(res))
            {
                throw new Exception("创建节点失败");
            }
            return res;
        }

        public static async Task<bool> ExistAsync_(this ZooKeeper client, string path, Watcher watcher = null)
        {
            if (watcher == null)
            {
                var res = await client.existsAsync(path, watch: false);
                return res != null;
            }
            else
            {
                var res = await client.existsAsync(path, watcher);
                return res != null;
            }
        }

        public static async Task EnsurePersistentPath(this ZooKeeper client, string path)
        {
            __check_path__(path);

            if (await client.ExistAsync_(path))
                return;

            var sp = path.SplitZookeeperPath();
            var p = string.Empty;
            foreach (var itm in sp)
            {
                p += $"/{itm}";
                if (!await client.ExistAsync_(p))
                {
                    await client.CreateNode_(p, CreateMode.PERSISTENT);
                }
            }
        }

        public static async Task<string> CreateSequentialPath_(this ZooKeeper client, string path,
            byte[] data = null, bool persistent = true)
        {
            __check_path__(path);

            var mode = persistent ?
                CreateMode.PERSISTENT_SEQUENTIAL :
                CreateMode.EPHEMERAL_SEQUENTIAL;

            //创建序列节点
            var p = await client.CreateNode_(path, mode, data);
            //取path最后一个路径
            var no = p.SplitZookeeperPath().LastOrDefault();
            return no ?? throw new Exception("未能创建序列节点");
            //return p.Substring(path.Length);
        }

        public static async Task<byte[]> GetDataOrThrow_(this ZooKeeper client, string path, Watcher watcher = null)
        {
            __check_path__(path);

            var res = watcher == null ?
                await client.getDataAsync(path, watch: false) :
                await client.getDataAsync(path, watcher);

            if (res.Stat == null)
                throw new Exception(res?.ToJson() ?? "数据不存在");
            return res.Data;
        }

        public static async Task DeleteNodeRecursively_(this ZooKeeper client, string path)
        {
            __check_path__(path);

            var handlered_list = new List<string>();

            async Task __delete_node__(string pre_path, string p)
            {
                var pre_node = pre_path.SplitZookeeperPath();
                var cur_node = p.SplitZookeeperPath();

                if (ValidateHelper.IsEmpty(cur_node))
                    throw new Exception($"不能删除：{p}");

                var current_node = new List<string>().AddList_(pre_node).AddList_(cur_node).AsZookeeperPath();
                //检查死循环
                handlered_list.AddOnceOrThrow(current_node, $"递归发生错误");

                if (!await client.ExistAsync_(current_node))
                    return;
                var children = await client.GetNodeChildren(current_node);
                if (children.Any())
                {
                    foreach (var child in children)
                    {
                        //递归
                        await __delete_node__(current_node, child);
                    }
                }
                await client.deleteAsync(current_node);
            }

            //入口
            await __delete_node__(string.Empty, path);
        }

        public static async Task<List<string>> GetNodeChildren(this ZooKeeper client, string path, Watcher watcher = null)
        {
            __check_path__(path);

            var res = watcher == null ?
                await client.getChildrenAsync(path, watch: false) :
                await client.getChildrenAsync(path, watcher);

            if (res.Stat == null)
                throw new Exception("读取子节点状态为空");

            return res.Children ?? new List<string>() { };
        }
    }
}
