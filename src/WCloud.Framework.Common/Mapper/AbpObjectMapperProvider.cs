using WCloud.Core.Mapper;

namespace WCloud.Framework.Common.Mapper
{
    public class AbpObjectMapperProvider : IDataMapper
    {
        private readonly Volo.Abp.ObjectMapping.IObjectMapper mapper;
        public AbpObjectMapperProvider(Volo.Abp.ObjectMapping.IObjectMapper mapper)
        {
            this.mapper = mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            var res = this.mapper.Map<TSource, TDestination>(source);
            return res;
        }
    }
}
