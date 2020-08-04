using Lib.helper;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;

namespace WCloud.Framework.Database.MongoDB
{
    public static class MongoExtension
    {
        public static SortDefinition<T> Sort__<T, SortType>(this SortDefinitionBuilder<T> builder, Expression<Func<T, SortType>> field, bool desc)
        {
            if (field.Body is MemberExpression exp)
            {
                var parameter_type = field.Parameters.FirstOrDefault()?.Type;
                parameter_type.Should().NotBeNull();

                var parameter = Expression.Parameter(parameter_type, "x");
                var member = Expression.Property(parameter, propertyName: exp.Member.Name);

                Expression body = member;

                var sort_field = Expression.Lambda<Func<T, object>>(body: body, parameters: new[] { parameter });

                if (desc)
                {
                    return builder.Descending(sort_field);
                }
                else
                {
                    return builder.Ascending(sort_field);
                }
            }
            else
            {
                throw new NotSupportedException("不支持的排序lambda表达式");
            }
        }

        public static SortDefinition<T> Sort_<T, SortType>(this SortDefinitionBuilder<T> builder, Expression<Func<T, SortType>> field, bool desc)
        {
            if (field.Body is MemberExpression exp)
            {
                var name = exp.Member.Name;
                if (desc)
                {
                    return builder.Descending(name);
                }
                else
                {
                    return builder.Ascending(name);
                }
            }
            else
            {
                throw new NotSupportedException("不支持的排序lambda表达式");
            }
        }

        public static IFindFluent<T, T> Take<T>(this IFindFluent<T, T> finder, int take)
        {
            var res = finder.Limit(take);
            return res;
        }

        public static IFindFluent<T, T> QueryPage<T>(this IFindFluent<T, T> finder, int page, int pagesize)
        {
            var range = PagerHelper.GetQueryRange(page, pagesize);
            var res = finder.Skip(range.skip).Take(range.take);
            return res;
        }
    }
}
