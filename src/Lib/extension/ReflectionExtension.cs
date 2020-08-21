using FluentAssertions;
using Lib.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lib.extension
{
    /// <summary>
    /// BaseType是类
    /// GetInterfaces是接口
    /// IsGenericType是泛型
    /// GetGenericTypeDefinition()获取泛型类型比如Consumer《string》
    /// </summary>
    public static class ReflectionExtension
    {
        /*
         * 获取类型的字符串表示，适用于泛型。
         
            string ParseType(Type t)
            {
                if (t.IsGenericType)
                {
                    var args = t.GetGenericArguments();

                    if (t.IsGenericType_(typeof(Task<>)))
                        return ParseType(args.FirstOrDefault() ?? throw new ArgumentNullException("获取不到值"));

                    return $"{t.Name.Replace("`1", string.Empty)}<{string.Join(",", args.Select(x => ParseType(x)))}>";
                }
                else
                {
                    return t.Name;
                }
            }
             */

        /// <summary>
        /// 找到依赖的所有程序集
        /// </summary>
        public static IEnumerable<Assembly> FindAllReferencedAssemblies(this Assembly entry, Func<Assembly, bool> filter = null)
        {
            filter ??= x => true;
            var finded = new List<Assembly>();

            IEnumerable<Assembly> __find__(Assembly ass)
            {
                ass.Should().NotBeNull();

                if (!finded.Contains(ass))
                {
                    yield return ass;
                    finded.Add(ass);
                    //找到依赖
                    var referenced_ass = ass.GetReferencedAssemblies().Select(x => Assembly.Load(x)).ToArray();
                    foreach (var m in referenced_ass)
                    {
                        foreach (var finded in __find__(m))
                        {
                            yield return finded;
                        }
                    }
                }
            }

            var res = __find__(entry);
            return res;
        }

        public static bool IsNullable(this Type t)
        {
            var res = t.IsGenericType_(typeof(Nullable<>));
            return res;
        }

        public static bool IsDatabaseTable(this Type t)
        {
            var res = t.IsNormalClass() && t.IsAssignableTo_<IDBTable>();
            return res;
        }

        /// <summary>
        /// 找到表对象
        /// </summary>
        public static IEnumerable<Type> FindEntity_(this Assembly ass)
        {
            var res = ass.GetTypes().Where(x => IsDatabaseTable(x));
            return res;
        }

        /// <summary>
        /// 获取所有类型
        /// </summary>
        /// <param name="ass"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllTypes(this IEnumerable<Assembly> ass)
        {
            var res = ass.SelectMany(x => x.GetTypes());
            return res;
        }

        /// <summary>
        /// 生成表实例，用来生成json给前端使用
        /// </summary>
        public static Dictionary<string, object> FindEntityDefaultInstance(this Assembly ass)
        {
            var res = ass.FindEntity_().ToDictionary_(x => x.FullName, x => Activator.CreateInstance(x));
            return res;
        }

        public static bool IsAsync(this MethodInfo method)
        {
            var res = method.ReturnType == typeof(Task) || method.ReturnType.IsGenericType_(typeof(Task<>));
            return res;
        }

        public static IEnumerable<Type> FindTypesByBaseType<T>(this Assembly a)
        {
            var tp = typeof(T);
            var res = a.GetTypes().Where(x => x.BaseType == tp);
            return res;
        }

        /// <summary>
        /// 判断是否是泛型的子类
        /// </summary>
        /// <param name="t"></param>
        /// <param name="generic_type"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool IsAssignableToGeneric_(this Type t, Type generic_type)
        {
            if (!generic_type.IsGenericType)
                throw new ArgumentException("必须是泛型");

            if (generic_type.IsInterface)
            {
                var res = t.GetAllInterfaces_().Any(x => x.IsGenericType_(generic_type));
                return res;
            }
            else
            {
                var repeat = new List<Type>();
                bool __is_generic_class__(Type a)
                {
                    if (repeat.Contains(a))
                    {
                        return false;
                    }
                    repeat.Add(a);
                    //-------------------------------------
                    if (IsGenericType_(a, generic_type))
                    {
                        return true;
                    }
                    var base_type = a.BaseType;
                    if (base_type == null || base_type == typeof(object))
                    {
                        return false;
                    }

                    return __is_generic_class__(base_type);
                }

                return __is_generic_class__(t);
            }
        }

        /// <summary>
        /// 是否可以赋值给
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsAssignableTo_<T>(this Type t)
        {
            var res = t.IsAssignableTo_(typeof(T));
            return res;
        }

        /// <summary>
        /// 是否可以赋值给
        /// </summary>
        /// <param name="t"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAssignableTo_(this Type t, Type type)
        {
            var res = type.IsAssignableFrom(t);
            return res;
        }

        /// <summary>
        /// 非抽象类，不是抽象类，不是接口
        /// </summary>
        public static bool IsNormalClass(this Type t)
        {
            var res = t.IsClass && !t.IsAbstract && !t.IsInterface;
            return res;
        }

        public static IEnumerable<Type> GetAllNormalClass(this Assembly ass)
        {
            var res = ass.GetTypes().Where(x => x.IsNormalClass());
            return res;
        }

        /// <summary>
        /// 是指定的泛型
        /// </summary>
        /// <param name="t"></param>
        /// <param name="tt"></param>
        /// <returns></returns>
        public static bool IsGenericType_(this Type t, Type tt)
        {
            if (!tt.IsGenericType)
                throw new ArgumentException("传入参数必须是泛型");

            var res = t.IsGenericType && t.GetGenericTypeDefinition() == tt;
            return res;
        }

        /// <summary>
        /// 获取所有实现接口，包括继承的
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<Type> GetAllInterfaces_(this Type t)
        {
            var res = t.GetInterfaces().ToList();
            return res;
        }

        /// <summary>
        /// 获取可以赋值给T的属性
        /// </summary>
        public static IEnumerable<T> GetCustomAttributes_<T>(this MemberInfo prop, bool inherit = true)
            where T : Attribute
        {
            var attrs = CustomAttributeExtensions.GetCustomAttributes(prop, inherit);
            var res = attrs.Where(x => x.GetType().IsAssignableTo_<T>()).Select(x => (T)x).ToArray();
            return res;
        }

        /// <summary>
        /// 有属性
        /// </summary>
        public static bool HasCustomAttributes_<T>(this MemberInfo prop, bool inherit = true)
            where T : Attribute
        {
            var res = prop.GetCustomAttributes_<T>(inherit).Any();
            return res;
        }
    }
}
