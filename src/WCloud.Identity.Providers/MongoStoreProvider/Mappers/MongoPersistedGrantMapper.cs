using System;
using AutoMapper;
using WCloud.Identity.Providers.MongoStoreProvider.Entity;
using IdentityServer4.Models;

namespace WCloud.Identity.Providers.MongoStoreProvider.Mappers
{
    public class MongoPersistedGrantMapper:Profile
    {
        public MongoPersistedGrantMapper()
        {
            this.CreateMap<MongoPersistedGrantEntity, PersistedGrant>()
                .ForMember(x=>x.Key,s=>s.MapFrom(x=>x.Key))
                .ForMember(x=>x.Type,s=>s.MapFrom(x=>x.Type))
                .ForMember(x => x.SubjectId, s => s.MapFrom(x => x.SubjectId))
                .ForMember(x => x.SessionId, s => s.MapFrom(x => x.SessionId))
                .ForMember(x => x.ClientId, s => s.MapFrom(x => x.ClientId))
                .ForMember(x => x.CreationTime, s => s.MapFrom(x => x.CreationTime))
                .ForMember(x => x.Expiration, s => s.MapFrom(x => x.Expiration))
                .ForMember(x => x.ConsumedTime, s => s.MapFrom(x => x.ConsumedTime))
                .ForMember(x => x.Data, s => s.MapFrom(x => x.Data));

            this.CreateMap<PersistedGrant, MongoPersistedGrantEntity>()
                .ForMember(x => x.Key, s => s.MapFrom(x => x.Key))
                .ForMember(x => x.Type, s => s.MapFrom(x => x.Type))
                .ForMember(x => x.SubjectId, s => s.MapFrom(x => x.SubjectId))
                .ForMember(x => x.SessionId, s => s.MapFrom(x => x.SessionId))
                .ForMember(x => x.ClientId, s => s.MapFrom(x => x.ClientId))
                .ForMember(x => x.CreationTime, s => s.MapFrom(x => x.CreationTime))
                .ForMember(x => x.Expiration, s => s.MapFrom(x => x.Expiration))
                .ForMember(x => x.ConsumedTime, s => s.MapFrom(x => x.ConsumedTime))
                .ForMember(x => x.Data, s => s.MapFrom(x => x.Data));
        }
    }
}
