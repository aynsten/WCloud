using Lib.core;
using Lib.helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lib.extension
{
    public static class CommonExtension
    {
        public static bool HasDuplicateItems(this IEnumerable<string> list)
        {
            var res = list.Count() != list.Distinct().Count();
            return res;
        }

        public static Dictionary<string, string> ConfigAsKV(this IConfiguration config)
        {
            var dict = new Dictionary<string, string>();
            foreach (var kv in config.AsEnumerable())
            {
                dict[kv.Key] = kv.Value;
            }

            return dict;
        }

        public static T ExecuteWithRetry_<T>(this Func<T> action, int retry_count, Func<int, TimeSpan> delay = null)
        {
            if (retry_count < 0)
                throw new ArgumentException($"{nameof(retry_count)}");
            delay = delay ?? (i => TimeSpan.Zero);

            var error = 0;
            while (true)
            {
                try
                {
                    var res = action.Invoke();
                    return res;
                }
                catch
                {
                    ++error;
                    if (error > retry_count)
                        throw;
                    var wait = delay.Invoke(error);
                    if (wait.TotalMilliseconds > 0)
                        System.Threading.Thread.Sleep(wait);
                }
            }
        }

        public static void ExecuteWithRetry_<T>(this Action action, int retry_count, Func<int, TimeSpan> delay = null)
        {
            var res = ExecuteWithRetry_(() =>
            {
                action.Invoke();
                return string.Empty;
            }, retry_count: retry_count, delay: delay);
        }

        /// <summary>
        /// 重试
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="retry_count"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static async Task<T> ExecuteWithRetryAsync_<T>(this Func<Task<T>> action, int retry_count, Func<int, TimeSpan> delay = null)
        {
            if (retry_count < 0)
                throw new ArgumentException($"{nameof(retry_count)}");
            delay = delay ?? (i => TimeSpan.Zero);

            var error = 0;
            while (true)
            {
                try
                {
                    //这里一定要await，不然task内部的异常将无法抓到
                    var res = await action.Invoke();
                    return res;
                }
                catch
                {
                    ++error;
                    if (error > retry_count)
                        throw;
                    var wait = delay.Invoke(error);
                    if (wait.TotalMilliseconds > 0)
                        await Task.Delay(wait);
                }
            }
        }

        /// <summary>
        /// 重试
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retry_count"></param>
        /// <param name="delay"></param>
        public static async Task ExecuteWithRetryAsync_(this Func<Task> action, int retry_count, Func<int, TimeSpan> delay = null)
        {
            var res = await ExecuteWithRetryAsync_(async () =>
            {
                await action.Invoke();
                return string.Empty;
            }, retry_count: retry_count, delay: delay);
        }

        public static List<string> GetInnerExceptionAsList(this Exception e)
        {
            var list = new List<string>();

            //暂时不适用递归（inner exception会不会有循环引用？）
            void AddMessage(Exception err)
            {
                list.Add(err.Message);
                if (err.InnerException != null)
                    AddMessage(err.InnerException);
            }

            var ex_list = new List<Exception>();

            void AddInnerExceptionMessage()
            {
                for (var inner = e; inner != null; inner = inner.InnerException)
                {
                    if (ex_list.Contains(inner))
                        break;
                    ex_list.Add(inner);

                    list.Add(inner.Message ?? string.Empty);
                }
            }

            AddInnerExceptionMessage();

            return list;
        }

        public static string GetInnerExceptionAsJson(this Exception e) => e.GetInnerExceptionAsList().ToJson();


        /// <summary>
        /// 获取过期的方法
        /// </summary>
        public static List<string> FindObsoleteFunctions(this Assembly ass)
        {
            var list = new List<string>();
            foreach (var tp in ass.GetTypes())
            {
                foreach (var func in tp.GetMethods())
                {
                    if (func.GetCustomAttributes<ObsoleteAttribute>().Any())
                    {
                        list.Add($"{tp.FullName}.{func.Name},{ass.FullName}");
                    }
                }
            }
            return list.Distinct().OrderBy(x => x).ToList();
        }

        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T CloneObject<T>(this T obj) where T : ICloneable
        {
            return (T)((ICloneable)obj).Clone();
        }

        /// <summary>
        /// 从list中随机取出一个item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ran"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Choice<T>(this Random ran, IList<T> list)
        {
            return ran.ChoiceIndexAndItem(list).item;
        }

        /// <summary>
        /// 随机抽取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ran"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static (int index, T item) ChoiceIndexAndItem<T>(this Random ran, IList<T> list)
        {
            //The maxValue for the upper-bound in the Next() method is exclusive—
            //the range includes minValue, maxValue-1, and all numbers in between.
            var index = ran.RealNext(minValue: 0, maxValue: list.Count - 1);
            return (index, list[index]);
        }

        /// <summary>
        /// 带边界的随机范围
        /// </summary>
        /// <param name="ran"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int RealNext(this Random ran, int maxValue)
        {
            //The maxValue for the upper-bound in the Next() method is exclusive—
            //the range includes minValue, maxValue-1, and all numbers in between.
            return ran.RealNext(minValue: 0, maxValue: maxValue);
        }

        /// <summary>
        /// 带边界的随机范围
        /// </summary>
        /// <param name="ran"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int RealNext(this Random ran, int minValue, int maxValue)
        {
            //The maxValue for the upper-bound in the Next() method is exclusive—
            //the range includes minValue, maxValue-1, and all numbers in between.
            return ran.Next(minValue: minValue, maxValue: maxValue + 1);
        }

        /// <summary>
        /// 随机抽取一个后从list中移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ran"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T PopChoice<T>(this Random ran, ref List<T> list)
        {
            var data = ran.ChoiceIndexAndItem(list);
            list.RemoveAt(data.index);
            return data.item;
        }

        /// <summary>
        /// 从list中随机抽取count个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ran"></param>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> Sample<T>(this Random ran, IList<T> list, int count)
        {
            return new int[count].Select(x => ran.Choice(list)).ToList();
        }

        /// <summary>
        /// 随机选取索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ran"></param>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<int> SampleIndexs<T>(this Random ran, IList<T> list, int count)
        {
            return ran.Sample(Com.Range(list.Count).ToList(), count);
        }

        /// <summary>
        /// 打乱list的顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ran"></param>
        /// <param name="list"></param>
        public static void Shuffle<T>(this Random ran, ref List<T> list)
        {
            var data = new List<T>();
            while (list.Any())
            {
                var itm = ran.PopChoice(ref list);
                data.Add(itm);
            }
            list.AddRange(data);
        }

        /// <summary>
        /// 根据权重选择
        /// </summary>
        public static T ChoiceByWeight<T>(this Random ran, IEnumerable<T> source, Func<T, int> selector)
        {
            if (ValidateHelper.IsEmpty(source))
                throw new ArgumentException(nameof(source));

            if (source.Count() == 1)
                return source.First();

            if (source.Any(x => selector.Invoke(x) < 1))
                throw new ArgumentException("权重不能小于1");

            var total_weight = source.Sum(x => selector.Invoke(x));
            //这次命中的权重
            var weight = ran.RealNext(maxValue: total_weight);

            var cur = 0;

            foreach (var s in source)
            {
                //单个权重
                var w = selector.Invoke(s);

                var start = cur;
                var end = start + w;

                //假如在此区间
                if (weight >= start && weight <= end)
                {
                    return s;
                }

                cur = end;
            }

            throw new MsgException("权重取值异常");
        }
    }
}
