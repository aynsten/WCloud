using FluentAssertions;
using Lib.data;
using Lib.extension;
using Lib.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.Abstractions.Extension
{
    public static class TreeEntityExtension
    {
        #region 非递归实现
        /// <summary>
        /// 深度优先
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IEnumerable<KeyValuePair<T, T[]>> FindNodeChildrenRecursivelyDeepFirst__<T>(this IEnumerable<T> data_source, T first_node) where T : TreeEntityBase
        {
            var list = new List<string>();

            var stack = new Stack<T>();
            stack.Push(first_node);

            while (true)
            {
                if (!stack.TryPop(out var data))
                {
                    break;
                }
                list.AddOnceOrThrow(data.UID);

                var children = data_source.Where(x => x.ParentUID == data.UID).ToArray();
                foreach (var child in children)
                {
                    stack.Push(child);
                }

                yield return new KeyValuePair<T, T[]>(data, children);
            }
        }

        /// <summary>
        /// 广度优先
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IEnumerable<KeyValuePair<T, T[]>> FindNodeChildrenRecursivelyWideFirst__<T>(this IEnumerable<T> data_source, T first_node) where T : TreeEntityBase
        {
            var list = new List<string>();

            var stack = new Queue<T>();
            stack.Enqueue(first_node);

            while (true)
            {
                if (!stack.TryDequeue(out var data))
                {
                    break;
                }
                list.AddOnceOrThrow(data.UID);

                var children = data_source.Where(x => x.ParentUID == data.UID).ToArray();
                foreach (var child in children)
                {
                    stack.Enqueue(child);
                }

                yield return new KeyValuePair<T, T[]>(data, children);
            }
        }
        #endregion


        #region 递归相关逻辑

        public static void FindNodeChildrenRecursively__<T>(this IEnumerable<T> data_source, T first_node, Action<T, T[]> callback)
            where T : TreeEntityBase
        {
            var repeat_check = new List<string>();

            void __find_children__(ref T[] nodes)
            {
                foreach (var m in nodes)
                {
                    repeat_check.AddOnceOrThrow(m.UID, error_msg: "树存在无限递归");

                    var children = data_source.Where(x => x.ParentUID == m.UID).ToArray();
                    __find_children__(ref children);

                    callback.Invoke(m, children);
                }
            }

            var start = new T[] { first_node };
            __find_children__(ref start);
        }

        public static List<T> FindNodeChildrenRecursively_<T>(this IEnumerable<T> data_source, T first_node)
            where T : TreeEntityBase
        {
            var list = new List<T>();
            data_source.FindNodeChildrenRecursively__(first_node, (node, children) => list.Add(node));
            return list;
        }

        public static T[] FindNodePath<T>(this IEnumerable<T> list, T node) where T : TreeEntityBase
        {
            node.Should().NotBeNull();
            node.UID.Should().NotBeNullOrEmpty();

            var path = new List<T>();
            var visit_path = new List<string>();

            var current_uid = node.UID;

            while (true)
            {
                visit_path.AddOnceOrThrow(current_uid);
                var current_node = list.FirstOrDefault(x => x.UID == current_uid);
                if (current_node == null)
                    break;
                path.Insert(0, current_node);

                //next
                current_uid = current_node.ParentUID;
            }

            return path.ToArray();
        }

        public static List<T> FindTreeBadNodes<T>(this IEnumerable<T> data_source) where T : TreeEntityBase
        {
            var list = data_source.ToList();
            var error_list = new List<string>();

            foreach (var node in list.OrderByDescending(x => x.Level))
            {
                if (error_list.Contains(node.UID))
                {
                    //防止中间节点被重复计算
                    //node1->node2->node3->node4->node5
                    //计算了node1到node5为错误节点之后将跳过1,2,3,4的检查
                    continue;
                }
                var node_path = data_source.FindNodePath(node);
                if (node_path.Select(x => x.ParentUID).FirstOrDefault() != TreeEntityBase.FIRST_PARENT_UID)
                {
                    error_list.AddRange(node_path.Select(x => x.UID));
                }
            }

            return list.Where(x => error_list.Distinct().Contains(x.UID)).ToList();
        }

        #endregion

        public static async Task<_<T>> AddTreeNode<T>(this IRepository<T> repo, T model, string model_flag = null) where T : TreeEntityBase
        {
            var res = new _<T>();
            model.InitSelf();

            if (model.IsFirstLevel())
            {
                model.AsFirstLevel();
            }
            else
            {
                var parent = await repo.QueryOneAsync(x => x.UID == model.ParentUID);
                parent.Should().NotBeNull("父节点为空");
                model.Level = parent.Level + 1;
            }

            if (!model.IsValid(out var msg))
            {
                res.SetErrorMsg(msg);
                return res;
            }

            await repo.InsertAsync(model);
            return res.SetSuccessData(model);
        }

        public static async Task DeleteTreeNodeRecursively<T>(this IRepository<T> repo, string node_uid) where T : TreeEntityBase
        {
            var node = await repo.QueryOneAsync(x => x.UID == node_uid);
            node.Should().NotBeNull();

            var list = await repo.QueryManyAsync(x => x.GroupKey == node.GroupKey);
            list.Count.Should().BeLessOrEqualTo(5000);

            var dead_nodes = list.FindNodeChildrenRecursively_(node).Select(x => x.UID);

            if (dead_nodes.Any())
                await repo.DeleteWhereAsync(x => dead_nodes.Contains(x.UID));
        }

        public static async Task<bool> DeleteSingleNodeWhenNoChildren_<T>(this IRepository<T> repo, string node_uid) where T : TreeEntityBase
        {
            if (await repo.ExistAsync(x => x.ParentUID == node_uid))
            {
                return false;
            }

            var count = await repo.DeleteWhereAsync(x => x.UID == node_uid);

            return count > 0;
        }

        /// <summary>
        /// 判断是父级节点
        /// </summary>
        /// <returns></returns>
        public static bool IsFirstLevel<T>(this T model) where T : TreeEntityBase
        {
            return ValidateHelper.IsEmpty(model.ParentUID?.Trim()) ||
                model.ParentUID == TreeEntityBase.FIRST_PARENT_UID ||
                model.Level == TreeEntityBase.FIRST_LEVEL;
        }

        /// <summary>
        /// 修改节点层级和父级为第一级
        /// </summary>
        public static T AsFirstLevel<T>(this T model) where T : TreeEntityBase
        {
            model.ParentUID = TreeEntityBase.FIRST_PARENT_UID;
            model.Level = TreeEntityBase.FIRST_LEVEL;
            return model;
        }

        /// <summary>
        /// 如果节点的层级和父级错误，就修改为第一级
        /// </summary>
        public static T AsFirstLevelIfParentIsNotValid<T>(this T model) where T : TreeEntityBase
        {
            if (model.IsFirstLevel())
            {
                model.AsFirstLevel();
            }
            return model;
        }

        public static IEnumerable<T> GetChildrenOf<T>(this IEnumerable<T> list, T node) where T : TreeEntityBase
        {
            var children = list.Where(x => x.ParentUID == node.UID && x.Level == node.Level + 1).ToArray();
            return children;
        }
    }
}
