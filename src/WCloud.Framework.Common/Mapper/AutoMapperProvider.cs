using AutoMapper;
using WCloud.Core.Mapper;

namespace WCloud.Framework.Common.Mapper
{
    public class AutoMapperProvider : IDataMapper
    {
        private readonly IMapper mapper;
        public AutoMapperProvider(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            var res = this.mapper.Map<TSource, TDestination>(source);
            return res;
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            var res = this.mapper.Map<TSource, TDestination>(source, destination);
            return res;
        }
    }
}
