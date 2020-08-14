namespace WCloud.Core.Mapper
{
    public interface IDataMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);

        //不常用，不实现
        //TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}
