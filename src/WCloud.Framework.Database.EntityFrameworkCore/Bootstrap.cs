using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using WCloud.Framework.Database.EntityFrameworkCore.Repository;

namespace WCloud.Framework.Database.EntityFrameworkCore
{
    public static class EFBootstrap
    {
        /// <summary>
        /// 使用EF
        /// </summary>
        public static IServiceCollection AddEF<T>(this IServiceCollection collection)
            where T : DbContext
        {
            collection.AddTransient<DbContext, T>().AddTransient<T, T>();
            return collection;
        }

        /// <summary>
        /// 使用repo，type必须是泛型
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="repoType"></param>
        /// <returns></returns>
        public static IServiceCollection UseEFRepository(this IServiceCollection collection, Type repoType)
        {
            if (repoType == null)
                throw new ArgumentNullException(nameof(repoType));
            if (!repoType.IsGenericType)
                throw new ArgumentException("ef repository type must be generic type");

            collection.AddTransient(typeof(IEFRepository<>), repoType);
            return collection;
        }
    }
}
