using FluentAssertions;
using Lib.core;
using Lib.helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lib.extension
{
    public static class EnumerableExtension
    {
        [Obsolete]
        public static void BindData<Model, Data, UnionKeyType>(this IEnumerable<Model> models, IEnumerable<Data> datas,
            Expression<Func<Model, Data>> field_selector,
            Func<Model, UnionKeyType> model_key,
            Func<Data, UnionKeyType> data_key)
        {
            var p = ((field_selector.Body as MemberExpression)?.Member) as PropertyInfo;
            if (p == null)
            {
                throw new NotSupportedException("无法绑定数据，不支持的表达式");
            }

            foreach (var m in models)
            {
                var data_query = datas.Where(x =>
                object.Equals(data_key.Invoke(x), model_key.Invoke(m)) ||
                object.ReferenceEquals(data_key.Invoke(x), model_key.Invoke(m)));

                p.SetValue(m, value: data_query.FirstOrDefault());
            }
        }

        /// <summary>
        /// 打乱一个集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            list.Should().NotBeNull();

            return list.OrderBy(m => Guid.NewGuid()).ToArray();
        }

        public static List<T> AddList_<T>(this List<T> list, IEnumerable<T> data)
        {
            if (ValidateHelper.IsNotEmpty(data))
            {
                list.AddRange(data);
            }
            return list;
        }

        public static List<T> AddItem_<T>(this List<T> list, params T[] data)
        {
            var res = AddList_(list, data: data);

            return res;
        }

        public static bool IsEmpty_<T>(this IEnumerable<T> list)
        {
            list.Should().NotBeNull();

            var res = !list.Any();
            return res;
        }

        /// <summary>
        /// 取第一个非空
        /// </summary>
        public static string FirstNotEmpty_(this IEnumerable<string> list)
        {
            var res = list.WhereNotEmpty().FirstOrDefault();
            return res;
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> list)
        {
            var res = list.Where(x => x != null).ToArray();
            return res;
        }

        public static IEnumerable<string> WhereNotEmpty(this IEnumerable<string> list)
        {
            var res = list.Where(x => ValidateHelper.IsNotEmpty(x)).ToArray();
            return res;
        }

        /// <summary>
        /// 排除空字符串并去重
        /// </summary>
        public static IEnumerable<string> SelectNotEmptyAndDistinct<T>(this IEnumerable<T> list, Func<T, string> target)
        {
            var res = list.Select(x => target.Invoke(x)).WhereNotEmpty().Distinct().ToArray();
            return res;
        }

        /// <summary>
        /// 更新集合，多删少补
        /// </summary>
        public static (IEnumerable<T> WaitForDelete, IEnumerable<T> WaitForAdd) UpdateList<T>(this IEnumerable<T> old_list,
            IEnumerable<T> new_list, IEqualityComparer<T> comparer = null)
        {
            new_list = new_list ?? throw new ArgumentNullException(nameof(new_list));

            var delete_list = old_list.Except_(new_list, comparer).ToList();
            var create_list = new_list.Except_(old_list, comparer).ToList();

            return (delete_list, create_list);
        }

        /// <summary>
        /// 更新集合，多删少补
        /// </summary>
        public static (IEnumerable<T> WaitForDelete, IEnumerable<T> WaitForAdd) UpdateList<T, Target>(this IEnumerable<T> old_list,
            IEnumerable<T> new_list, Func<T, Target> selector, IEqualityComparer<Target> comparer = null)
        {
            var change = old_list.Select(selector).UpdateList(new_list.Select(selector), comparer);

            var delete_list = old_list.Where(x => change.WaitForDelete.Contains(selector.Invoke(x))).ToList();
            var create_list = new_list.Where(x => change.WaitForAdd.Contains(selector.Invoke(x))).ToList();

            return (delete_list, create_list);
        }

        /// <summary>
        /// 转为可迭代实体
        /// </summary>
        public static IEnumerable<T> AsEnumerable_<T>(this IEnumerable collection)
        {
            foreach (T item in collection)
            {
                yield return item;
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        public static void RemoveWhere_<T>(this List<T> list, Func<T, bool> where)
        {
            for (var i = list.Count - 1; i >= 0; --i)
            {
                var item = list[i];
                if (where.Invoke(item))
                {
                    list.Remove(item);
                }
            }
        }

        /// <summary>
        /// 包含
        /// </summary>
        static bool __Contains<T>(this IEnumerable<T> list, T data, IEqualityComparer<T> comparer = null)
        {
            list.Should().NotBeNull();

            return comparer == null ?
                list.Contains(data) :
                list.Contains(data, comparer);
        }

        /// <summary>
        /// 全部包含，data为空则返回false
        /// </summary>
        public static bool ContainsAll_<T>(this IEnumerable<T> list, IEnumerable<T> data, IEqualityComparer<T> comparer = null)
        {
            list.Should().NotBeNull();
            data.Should().NotBeNull();

            if (!data.Any())
            {
                return false;
            }

            var res = data.All(x => list.__Contains(x, comparer));
            return res;
        }

        /// <summary>
        /// 包含部分，data为空则返回false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool ContainsAny_<T>(this IEnumerable<T> list, IEnumerable<T> data, IEqualityComparer<T> comparer = null)
        {
            data.Should().NotBeNull();

            if (!data.Any())
            {
                return false;
            }

            var res = data.Any(x => list.__Contains(x, comparer));
            return res;
        }

        /// <summary>
        /// 获取两个集合的交集
        /// </summary>
        public static IEnumerable<T> GetInterSection<T>(this IEnumerable<T> list, IEnumerable<T> data, IEqualityComparer<T> comparer = null)
        {
            var res = list.Where(x => data.__Contains(x, comparer: comparer)).ToArray();
            return res;
        }

        /// <summary>
        /// 两个集合有部分item相等，两个空集合不相等
        /// </summary>
        public static bool AnyEqual<T>(this IEnumerable<T> list, IEnumerable<T> data, IEqualityComparer<T> comparer = null)
        {
            list.Should().NotBeNull();
            data.Should().NotBeNull();

            var res = list.GetInterSection(data, comparer).Any();
            return res;
        }

        /// <summary>
        /// 两个集合item相等，两个空集合也相等
        /// </summary>
        public static bool AllEqual<T>(this IEnumerable<T> list, IEnumerable<T> data, IEqualityComparer<T> comparer = null)
        {
            list.Should().NotBeNull();
            data.Should().NotBeNull();

            if (list.Count() != data.Count())
            {
                return false;
            }

            foreach (var m in list)
            {
                if (!data.__Contains(m, comparer))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 除了（排除）
        /// </summary>
        public static IEnumerable<T> Except_<T>(this IEnumerable<T> list, IEnumerable<T> data, IEqualityComparer<T> comparer = null)
        {
            data.Should().NotBeNull();

            return list.Where(x => !data.__Contains(x, comparer)).ToArray();
        }

        /// <summary>
        /// 在list中添加item，遇到重复就抛异常
        /// </summary>
        public static List<T> AddOnceOrThrow<T>(this List<T> list, T flag, string error_msg = null, IEqualityComparer<T> comparer = null)
        {
            if (list.__Contains(flag, comparer: comparer))
            {
                throw new MsgException(error_msg ?? $"{flag}已经存在");
            }

            list.Add(flag);
            return list;
        }

        /// <summary>
        /// 空集合就抛异常
        /// </summary>
        public static IEnumerable<T> ThrowIfEmpty<T>(this IEnumerable<T> list, string msg = null)
        {
            if (!list.Any())
            {
                throw new MsgException(msg ?? "不允许的空集合");
            }
            return list;
        }

        /// <summary>
        /// 集合数量超出最大值就抛出异常
        /// </summary>
        public static IEnumerable<T> EnsureMaxCount<T>(this IEnumerable<T> list, int max, string msg = null)
        {
            var count = list.Count();
            if (count > max)
            {
                throw new MsgException(msg ?? $"集合数量({count})超过了允许最大值：{max}");
            }
            return list;
        }

        /// <summary>
        /// 链接字符串
        /// </summary>
        public static string ConcatString<T>(this IEnumerable<T> list)
        {
            var data = new StringBuilder();
            foreach (var item in list.Where(x => x != null))
            {
                switch ((object)item)
                {
                    case char c:
                        data.Append(c); break;
                    case string s when ValidateHelper.IsNotEmpty(s):
                        data.Append(s); break;
                    case int i:
                        data.Append(i); break;
                    case uint ui:
                        data.Append(ui); break;
                    case float f:
                        data.Append(f); break;
                    case long l:
                        data.Append(l); break;
                    case double d:
                        data.Append(d); break;
                    default:
                        {
                            object obj = item;
                            if (obj != null)
                            {
                                data.Append(obj);
                            }
                            break;
                        }
                }
            }
            return data.ToString();
        }

        /// <summary>
        /// 解决ilist没有foreach的问题
        /// </summary>
        public static void ForEach_<T>(this IEnumerable<T> list, Action<int, T> action)
        {
            var index = 0;
            foreach (var m in list)
            {
                action.Invoke(index++, m);
            }
        }

        /// <summary>
        /// 解决ilist没有foreach的问题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void ForEach_<T>(this IEnumerable<T> list, Action<T> action)
        {
            list.ForEach_((index, x) => action.Invoke(x));
        }

        /// <summary>
        /// 解决ilist没有foreach的问题
        /// </summary>
        public static async Task ForEachAsync_<T>(this IEnumerable<T> list, Func<int, T, Task> action)
        {
            var index = 0;
            foreach (var m in list)
            {
                await action.Invoke(index++, m);
            }
        }

        /// <summary>
        /// 解决ilist没有foreach的问题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static async Task ForEachAsync_<T>(this IEnumerable<T> list, Func<T, Task> action)
        {
            await list.ForEachAsync_(async (index, x) => await action.Invoke(x));
        }

        /// <summary>
        /// 反转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Reverse_<T>(this IEnumerable<T> list)
        {
            var data = list.ToList();
            if (data.Count < 2)
                return data;

            for (int i = 0; i < data.Count / 2; ++i)
            {
                var temp = data[i];
                var second_index = data.Count - 1 - i;

                data[i] = data[second_index];
                data[second_index] = temp;
            }

            return data;
        }

        /// <summary>
        /// NameValueCollection转为字典
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDict(this NameValueCollection col)
        {
            var dict = col.AllKeys.Where(x => x?.Length > 0).ToDictionary_(x => x, x => col[x]);

            return dict;
        }

        /// <summary>
        /// 不会因为重复key报错，后面的key会覆盖前面的key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="list"></param>
        /// <param name="key_selector"></param>
        /// <param name="value_selector"></param>
        /// <returns></returns>
        public static Dictionary<K, V> ToDictionary_<T, K, V>(this IEnumerable<T> list, Func<T, K> key_selector, Func<T, V> value_selector)
        {
            var dict = new Dictionary<K, V>();

            foreach (var m in list)
            {
                dict[key_selector.Invoke(m)] = value_selector.Invoke(m);
            }

            return dict;
        }

        /// <summary>
        /// 获取成员，超过索引返回默认值
        /// </summary>
        public static T GetItem<T>(this IList<T> list, int index, T deft = default)
        {
            if (index >= 0 && index <= list.Count - 1)
            {
                return list[index];
            }
            return deft;
        }

        /// <summary>
        /// 执行Reduce（逻辑和python一样），集合至少2个item
        /// </summary>
        public static T Reduce<T>(this IList<T> list, Func<T, T, T> func)
        {
            if (list.Count < 2)
                throw new ArgumentException($"item少于2的list无法执行{nameof(Reduce)}操作");
            if (func == null)
                throw new ArgumentNullException($"{nameof(Reduce)}.{nameof(func)}");

            var res = func.Invoke(list[0], list[1]);
            /*
             * not sure list.skip works before sort those data
            foreach (var m in list.Skip(2))
            {
                res = func.Invoke(res, m);
            }*/
            for (var i = 2; i < list.Count; ++i)
            {
                res = func.Invoke(res, list[i]);
            }
            return res;
        }

        /// <summary>
        /// 迭代相邻两个item
        /// </summary>
        public static void IterateItems<T>(this IList<T> list, Action<T, T, int, int> func)
        {
            if (list.Count < 2)
                throw new ArgumentException($"item少于2的list无法执行{nameof(Reduce)}操作");
            if (func == null)
                throw new ArgumentNullException($"{nameof(Reduce)}.{nameof(func)}");

            for (var i = 0; i <= list.Count - 1 - 1; ++i)
            {
                func.Invoke(list[i], list[i + 1], i, i + 1);
            }
        }

        /// <summary>
        /// 集合分批处理
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Batch_<T>(this IEnumerable<T> source, int batchSize)
        {
            if (batchSize <= 0)
                throw new ArgumentException("batch size必须大于0");

            IEnumerable<T> _batch(IEnumerator<T> enumerator)
            {
                var size = batchSize;

                do
                {
                    yield return enumerator.Current;
                    //you can change the size,because int is not reference type
                    //every batch you get new number of batch size
                } while (--size > 0 && enumerator.MoveNext());
            }

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return _batch(enumerator);
                }
            }

            //老方法
            IEnumerable<IEnumerable<T>> __Batch__()
            {
                var temp = new List<T>();
                foreach (var m in source)
                {
                    temp.Add(m);
                    if (temp.Count >= batchSize)
                    {
                        yield return temp;
                        //清空，开始下一个batch
                        temp = new List<T>();
                    }
                }
                if (temp.Any())
                {
                    yield return temp;
                }
            }
        }

        /// <summary>
        /// 集合分批处理,使用分页
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IOrderedEnumerable<T> list, int batchSize)
        {
            if (batchSize <= 0)
                throw new ArgumentException("batch size必须大于0");
            var page = 0;
            while (true)
            {
                ++page;

                var pager = PagerHelper.GetQueryRange(page, batchSize);
                var data = list.Skip(pager.skip).Take(pager.take).ToList();
                if (!data.Any())
                    break;

                yield return data;
            }
        }
    }

    /// <summary>
    /// 可以计算的list《int》
    /// </summary>
    public class ComputableList : List<int>
    {
        public ComputableList() { }

        public ComputableList(IEnumerable<int> list) : base(list) { }

        public static ComputableList operator +(ComputableList a, ComputableList b)
        {
            var list = new ComputableList();
            for (var i = 0; i < a.Count || i < b.Count; ++i)
            {
                list.Add(a.GetItem(i, 0) + b.GetItem(i, 0));
            }
            return list;
        }

        public static List<bool> operator >(ComputableList a, int b)
        {
            return a.Select(x => x > b).ToList();
        }

        public static List<bool> operator <(ComputableList a, int b)
        {
            return a.Select(x => x < b).ToList();
        }

        public static List<bool> operator >=(ComputableList a, int b)
        {
            return a.Select(x => x >= b).ToList();
        }

        public static List<bool> operator <=(ComputableList a, int b)
        {
            return a.Select(x => x <= b).ToList();
        }

        public static ComputableList operator *(ComputableList a, int b)
        {
            return new ComputableList(a.Select(x => x * b));
        }

        public static ComputableList operator /(ComputableList a, int b)
        {
            return new ComputableList(a.Select(x => x / b));
        }

        public static ComputableList operator +(ComputableList a, int b)
        {
            return new ComputableList(a.Select(x => x + b));
        }

        public static ComputableList operator -(ComputableList a, int b)
        {
            return new ComputableList(a.Select(x => x - b));
        }
    }
}
