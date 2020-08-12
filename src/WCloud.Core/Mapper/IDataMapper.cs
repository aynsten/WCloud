using System;

namespace WCloud.Core.Mapper
{
    public interface IDataMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}
